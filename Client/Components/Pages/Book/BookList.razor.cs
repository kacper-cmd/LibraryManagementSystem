using Application.DTOs;
using Application.RequestModel;
using Microsoft.AspNetCore.Components;
using Client.Services;
using Microsoft.JSInterop;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Infrastructure.Exceptions;
using Infrastructure.Constants;
using Microsoft.AspNetCore.Components.Forms;
using Client.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;
using CsvHelper.Configuration;
using Org.BouncyCastle.Crypto;
namespace Client.Components.Pages.Book;

public partial class BookList : ComponentBase
{
	#region Properties
	[Inject]
	private NavigationManager NavigationManager { get; set; }
	[Inject]
	private IBookService bookService { get; set; }
	[Inject]
	private IExcelService ExcelService { get; set; }
	[Inject]
	private IPdfService PdfService { get; set; }

	private BaseFilter filter = new BaseFilter();
	private List<BookDTO> books = new List<BookDTO>();
	private PagedListDTO<BookDTO> pageList;
	private int startPage = 1;
	private int endPage = 1;
	private Guid deleteBookId;
	private string userRole;
	private Guid? selectedBookId;
	private List<int> pageSizes = new List<int> { 5, 10, 20, 50 };
	private int totalEntries;
	private int startEntry;
	private int endEntry;
	private string? errorMessage;
	private List<string> columnsToExport = new List<string> { nameof(BookDTO.Title), nameof(BookDTO.Author), nameof(BookDTO.ISBN), nameof(BookDTO.PublishedDate) };
	private bool IsLoading { get; set; } = true;

	public bool DeleteDialogOpen { get; set; }

	public static  Dictionary<string, string> columns;
	private Dictionary<string, string> sortOrders;
	#endregion
	#region Methods
	public async Task Filter(BaseFilter baseFilter)
	{
		filter = baseFilter;
		filter.Page = 1;
		await LoadItems();
	}
	public void InitializeSelect()
	{
		columns = new Dictionary<string, string>
		{
			{ localizer["Title"],nameof(BookDTO.Title)},
			{ localizer["Author"] ,nameof(BookDTO.Author)},
			{ localizer["PublishedDate"], nameof(BookDTO.PublishedDate)  },
			{ nameof(BookDTO.ISBN) , nameof(BookDTO.ISBN)  },
			{ localizer["Available"], nameof(BookDTO.Available)  }
		};

		sortOrders = new Dictionary<string, string>
		{
			{ localizer["Ascending"], Constants.asc },
			{ localizer["Descending"], Constants.desc },
		};
	}
	protected override async Task OnInitializedAsync()
	{
		InitializeSelect();
		IsLoading = true;
		await LoadItems();
		IsLoading = false;
	}


	private async Task LoadItems()
	{
		try
		{

			var response = await bookService.GetPagedEntities(filter);
			pageList = response;
			books = pageList.Items;

			totalEntries = pageList.TotalCount;
			startEntry = (filter.Page - 1) * filter.PageSize + 1;
			endEntry = startEntry + books.Count - 1;


			CalculatePageRange();
			StateHasChanged();
		}
		catch (BadRequestException ex)
		{
			NavigationManager.NavigateTo("/login");
			toastService.ShowError($"You dont have access to this page : {ex.Message}");
			await Task.Delay(1000);
		}
		catch (ForbiddenException e)
		{
			errorMessage = $"Forbidden: {e.Message}";
		}
		catch (NotFoundException ex)
		{
			errorMessage = $"Not Found: {ex.Message}";
		}
		catch (InternalServerErrorException ex)
		{
			errorMessage = $"Internal Server Error: {ex.Message}";
		}
		catch (HttpRequestException e)
		{
			toastService.ShowError(e.Message);
			errorMessage = $"HTTP Request Failed: {e.Message}";
		}
		catch (Exception e)
		{
			toastService.ShowError(e.Message);
			errorMessage = $"An unexpected error occurred: {e.Message}";
		}
	}
	private async Task ExportToExcel()
	{
		var filterdto = filter;
		filterdto.Page = 1;
		filterdto.PageSize = totalEntries;
		var response = await bookService.GetPagedEntities(filterdto);
		if (response is not null)
		{

			await ExcelService.Export(response.Items, "Books.xlsx", columnsToExport);
		}
		else
		{
			await ExcelService.Export(books, "Books.xlsx", columnsToExport);
		}

	}
	private async Task ExportToPdf()
	{
		var filterdto = filter;
		filterdto.Page = 1;
		filterdto.PageSize = totalEntries;
		var response = await bookService.GetPagedEntities(filterdto);
		if (response is not null)
		{
			await PdfService.Export(response.Items, "Books", false, columnsToExport);
		}
		else
		{
			await PdfService.Export(books, "Books", false, columnsToExport);
		}

	}
	public class BookDTOMap : ClassMap<BookDTO>
	{
		public BookDTOMap()
		{
			var title = columns.ElementAt(0).Key;
			var author = columns.ElementAt(1).Key;
			var date = columns.ElementAt(2).Key;
			var isbn = columns.ElementAt(3).Key;

			Map(m => m.Title).Index(0).Name(title);
			Map(m => m.Author).Index(1).Name(author);
			Map(m => m.ISBN).Index(2).Name(isbn);
			Map(m => m.PublishedDate).Index(3).Name(date);
		}
	}
	private void ExportToCsv()
	{
		using (var memoryStream = new MemoryStream())
		{
			using (var writer = new StreamWriter(memoryStream))
			{
				var invariantFormat = CultureInfo.InvariantCulture;
				var config = new CsvHelper.Configuration.CsvConfiguration(invariantFormat);
				using (var csv = new CsvHelper.CsvWriter(writer, config))
				{
					csv.Context.RegisterClassMap<BookDTOMap>();
					csv.WriteRecords(books);
				}

				var arr = memoryStream.ToArray();
				JS.SaveAs("books.csv", arr);
			}
		}
	}
	private void CalculatePageRange()
	{
		int totalPages = (int)Math.Ceiling((double)pageList.TotalCount / filter.PageSize);

		if (totalPages <= 10)
		{
			startPage = 1;
			endPage = totalPages;
		}
		else
		{
			startPage = Math.Max(filter.Page - 2, 1);
			endPage = Math.Min(filter.Page + 2, totalPages);

			if (startPage <= 3)
			{
				endPage = 5;
			}
			else if (endPage >= totalPages - 2)
			{
				startPage = totalPages - 4;
			}
		}
	}

	public async Task ApplyFilter()
	{
		filter.Page = 1;
		await LoadItems();
	}

	public async Task ClearFilter()
	{
		filter = new BaseFilter();
		await LoadItems();
	}

	private async Task SortColumn(string column)
	{
		if (filter.SortColumn == column)
		{
			filter.SortOrder = filter.SortOrder == "asc" ? "desc" : "asc";
		}
		else
		{
			filter.SortColumn = column;
			filter.SortOrder = "asc";
		}
		await LoadItems();
	}

	private string GetSortIcon(string column)
	{
		if (filter.SortColumn == column)
		{
			return filter.SortOrder == "asc" ? "fa fa-sort-up" : "fa fa-sort-down";
		}
		return "fa fa-sort";
	}

	private async Task PreviousPage()
	{
		if (pageList.HasPreviousPage)
		{
			filter.Page--;
			await LoadItems();
		}
	}

	private async Task NextPage()
	{
		if (pageList.HasNextPage)
		{
			filter.Page++;
			await LoadItems();
		}
	}
	private async Task GoToFirstPage()
	{
		filter.Page = 1;
		await LoadItems();
	}

	private async Task GoToLastPage()
	{
		filter.Page = (int)Math.Ceiling((double)pageList.TotalCount / filter.PageSize);
		await LoadItems();
	}
	private async Task GoToPage(int page)
	{
		filter.Page = page;
		await LoadItems();
	}
	private void SelectBook(Guid bookId)
	{
		selectedBookId = bookId;
	}
	private void EditBook(Guid bookId)
	{
		SelectBook(bookId);
		NavigationManager.NavigateTo($"/books/edit/{bookId}");

	}

	private void ViewDetails(Guid bookId)
	{
		NavigationManager.NavigateTo($"/books/details/{bookId}");
	}
	private void AddBook()
	{
		NavigationManager.NavigateTo("/books/create");
	}
	private async Task OnDeleteDialogClose(bool accepted)
	{
		try
		{
			if (accepted)
			{
				// await bookService.DeleteBookAsync(deleteBookId);
				var apiResponse = await bookService.DeleteBookApiResponseAsync(deleteBookId);
				await LoadItems();
			}
		}
		catch (ValidationErrorsException ex)
		{
			var validationErrors = ex.Errors;
			foreach (var error in validationErrors)
			{
				toastService.ShowError(error);

			}
		}
		catch (ForbiddenException e)
		{
			toastService.ShowError($"Forbidden access to delete");
		}
		catch (Exception e)
		{
			errorMessage = $"Exception: {e.Message}";
		}
		finally
		{
			DeleteDialogOpen = false;
			StateHasChanged();
		}
	}
	private void OpenDeleteDialog(Guid bookId)
	{
		DeleteDialogOpen = true;
		deleteBookId = bookId;
		StateHasChanged();
	}
	private async Task OnPageSizeChange(ChangeEventArgs e)
	{
		if (e.Value != null && int.TryParse(e.Value.ToString(), out int pageSize))
		{
			filter.PageSize = pageSize;
			filter.Page = 1;
			await LoadItems();
		}
		else
		{
			filter.PageSize = pageSizes.First();
			filter.Page = 1;
			await LoadItems();
		}
	}
	private async Task Enter(KeyboardEventArgs e)
	{
		if (e.Code == "Enter")
		{
			await InvokeAsync(async () =>
			{
				await ApplyFilter();
				StateHasChanged();
			});
		}
	}
	#endregion
}
