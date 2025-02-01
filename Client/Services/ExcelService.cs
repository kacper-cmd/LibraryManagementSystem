using ClosedXML.Excel;
using ClosedXML.Graphics;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;
namespace Client.Services
{   /// <summary>
	/// BASE ON https://medium.com/@shahriyarali08/blazor-wasm-exporting-and-importing-data-to-excel-and-pdf-438e775da1a2
	/// </summary>
	public class ExcelService : IExcelService
	{
		private readonly IJSRuntime _jsRuntime;
		private readonly HttpClient _httpClient;
		private readonly IFontLoader _fontLoader;
		public ExcelService(IJSRuntime jsRuntime, HttpClient httpClient, IFontLoader fontLoader)
		{
			_jsRuntime = jsRuntime;
			_httpClient = httpClient;
			_fontLoader = fontLoader;
		}
		public async Task Export<T>(IEnumerable<T> data, string fileName, List<string>? columnsToExport = null)
		{
			using (var workbook = new XLWorkbook())
			{

				var propertyNames = columnsToExport != null
						? columnsToExport.Where(column => typeof(T).GetProperty(column) != null).ToList()
						: typeof(T).GetProperties().Select(p => p.Name).ToList();

				var worksheet = workbook.Worksheets.Add("Sheet1");

				// Write header row
				for (int i = 0; i < propertyNames.Count; i++)
				{
					worksheet.Cell(1, i + 1).Value = propertyNames[i];
				}

				// Write data rows
				var rowData = data.ToList();
				for (int rowIndex = 0; rowIndex < rowData.Count; rowIndex++)
				{
					for (int colIndex = 0; colIndex < propertyNames.Count; colIndex++)
					{
						var propertyName = propertyNames[colIndex];
						var propertyValue = typeof(T).GetProperty(propertyName)?.GetValue(rowData[rowIndex])?.ToString();
						worksheet.Cell(rowIndex + 2, colIndex + 1).Value = propertyValue;
					}
				}

				// Create Table
				worksheet.RangeUsed().CreateTable();

				// Save the workbook to a memory stream
				using (var stream = new MemoryStream())
				{
					workbook.SaveAs(stream);
					await _jsRuntime.InvokeVoidAsync("saveAsFile", fileName, stream.ToArray());
				}
			}
		}



		public async Task<List<T>> Read<T>(MemoryStream ms)
		{
			try
			{
				List<T> items = new();

				// You can use any font that exists in your project's wwwroot/fonts directory. If there no font file just download and add one.
				//var fallbackFontStream = await _httpClient.GetStreamAsync("fonts/SourceSansPro-Regular.ttf.woff2");
				using var fallbackFontStream = _fontLoader.GetFontStream("SourceSansPro-Regular.ttf.woff2");

				var loadOptions = new ClosedXML.Excel.LoadOptions
				{
					// Create a default graphic engine that uses only fallback font and additional fonts passed as streams. It also uses system fonts.
					GraphicEngine = DefaultGraphicEngine.CreateWithFontsAndSystemFonts(fallbackFontStream)
				};

				using (var workbook = new XLWorkbook(ms, loadOptions))
				{
					var worksheet = workbook.Worksheet(1);

					var table = worksheet.Table("Table1");

					var headers = table.HeadersRow().Cells().Select(c => c.Value.ToString()).ToList();

					foreach (var row in table.DataRange.RowsUsed())
					{
						T item = Activator.CreateInstance<T>();

						for (int i = 1; i <= headers.Count(); i++)
						{
							var header = headers[i - 1].ToString();
							var cellValue = row.Cell(i).Value.ToString();

							if (!string.IsNullOrEmpty(cellValue))
							{
								var property = typeof(T).GetProperty(header);

								if (property != null)
								{
									var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
									if (targetType == typeof(Guid))
									{
										if (Guid.TryParse(cellValue, out Guid guidValue))
										{
											property.SetValue(item, guidValue);
										}
										else
										{
											property.SetValue(item, Guid.Empty);
										}
									}
									else if (targetType == typeof(TaxOn))
									{
										Enum.TryParse(cellValue, out TaxOn value);
										property.SetValue(item, value);
									}
									else if (targetType == typeof(decimal?))
									{
										if (decimal.TryParse(cellValue, out decimal parsedValue))
										{
											property.SetValue(item, parsedValue);
										}
										else
										{
											property.SetValue(item, null);
										}
									}
									else if (targetType == typeof(float?))
									{
										if (float.TryParse(cellValue, out float parsedValue))
										{
											property.SetValue(item, parsedValue);
										}
										else
										{
											property.SetValue(item, null);
										}
									}
									else if (targetType == typeof(double?))
									{
										double parsedValue;
										if (double.TryParse(cellValue, out parsedValue))
										{
											property.SetValue(item, parsedValue);
										}
										else
										{
											property.SetValue(item, null);
										}
									}
									else if (targetType == typeof(DateTime))
									{
										DateTime parsedValue;
										if (DateTime.TryParse(cellValue, out parsedValue))
										{
											property.SetValue(item, parsedValue);
										}
										else
										{
											property.SetValue(item, null);
										}
									}
									else
									{
										var value = Convert.ChangeType(cellValue, targetType);
										property.SetValue(item, value);
									}
								}
							}
						}

						items.Add(item);
					}
				}

				return items;
			}
			catch (Exception e)
			{
				throw;
			}
		}
	}
	public enum TaxOn
	{
		None,
		Sales,
		Income,
		Property,
		Corporate
	}
}
