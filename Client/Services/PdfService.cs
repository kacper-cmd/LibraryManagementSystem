using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.JSInterop;
using Infrastructure.Constants;
namespace Client.Services
{
	public interface IPdfService
	{
		Task Export<T>(IEnumerable<T> data, string fileName, bool landscape = true, List<string>? columnsToExport = null);
	}

	public class PdfService : IPdfService
	{
		private readonly IJSRuntime _jsRuntime;

		public PdfService(IJSRuntime jsRuntime)
		{
			_jsRuntime = jsRuntime;
		}
		public async Task Export<T>(IEnumerable<T> data, string fileName, bool landscape = true, List<string>? columnsToExport = null)
		{

			var pageSize = landscape ? PageSize.A4.Rotate() : PageSize.A4;

			Document doc = new(pageSize, 10, 10, 10, 10);

			var stream = new MemoryStream();

			PdfWriter.GetInstance(doc, stream);

			doc.Open();

			Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);

			Paragraph title = new(fileName + "\n", titleFont)
			{
				Alignment = Element.ALIGN_CENTER
			};

			doc.Add(title);

			doc.Add(new Paragraph("\n"));

			//var propertyNames = typeof(T).GetProperties().Select(p => p.Name).ToList();

			var propertyNames = columnsToExport != null
								? columnsToExport.Where(column => typeof(T).GetProperty(column) != null).ToList()
								: typeof(T).GetProperties().Select(p => p.Name).ToList();

			PdfPTable table = new(propertyNames.Count)
			{
				WidthPercentage = 100
			};

			foreach (var propertyName in propertyNames)
			{
				PdfPCell headerCell = new(new Phrase(propertyName.SplitCamelCase(), new Font(Font.HELVETICA, 11f)))
				{
					VerticalAlignment = Element.ALIGN_MIDDLE,
					ExtraParagraphSpace = 2
				};
				table.AddCell(headerCell);
			}

			foreach (var item in data)
			{
				foreach (var propertyName in propertyNames)
				{
					var property = typeof(T).GetProperty(propertyName);
					if (property != null)
					{
						var value = property.GetValue(item);
						PdfPCell dataCell = new(new Phrase(value == null ? "" : value.ToString(), new Font(Font.HELVETICA, 11f)))
						{
							VerticalAlignment = Element.ALIGN_MIDDLE,
							ExtraParagraphSpace = 2
						};
						table.AddCell(dataCell);
					}
				}
			}

			doc.Add(table);

			doc.Close();

			await _jsRuntime.InvokeVoidAsync("saveAsFile", $"{fileName}.pdf", stream.ToArray());
		}
	}
}
