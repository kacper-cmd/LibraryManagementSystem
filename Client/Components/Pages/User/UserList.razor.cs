using Application.DTOs;
using Application.RequestModel;
using Application.Services;
using Client.Helpers;
using Client.Locales;
using Client.Services;
using CsvHelper.Configuration;
using Infrastructure.Constants;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;

namespace Client.Components.Pages.User;

public partial class UserList : ComponentBase
{

    #region Properties
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private IUserService service { get; set; }
	[Inject]
	private IExcelService ExcelService { get; set; }
	[Inject]
	private IPdfService PdfService { get; set; }

	private BaseFilter filter = new BaseFilter();
    private List<UserDTO> users = new List<UserDTO>();
    private PagedListDTO<UserDTO> pageList;
    private int startPage = 1;
    private int endPage = 1;
    private Guid deleteUserId;
    private string userRole;
    private bool IsLoading { get; set; } = true;
    private Guid? selectedUserId;
    private List<int> pageSizes = new List<int> { 5, 10, 20, 50 };
	private List<string> columnsToExport = new List<string> { nameof(UserDTO.Name), nameof(UserDTO.Email), nameof(UserDTO.Role) };
	public static Dictionary<string, string> columns;

    private Dictionary<string, string> sortOrders;
    #endregion
    public bool DeleteDialogOpen { get; set; }
    public void InitializeSelect()
    {
        columns = new Dictionary<string, string>
        {
            { localizer["Name"],nameof(UserDTO.Name)},
            { localizer["Email"] ,nameof(UserDTO.Email)},
            { localizer["Role"], nameof(UserDTO.Role)  }
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
    public async Task Filter(BaseFilter baseFilter)
    {
        filter = baseFilter;
        filter.Page = 1;
        await LoadItems();
    }

    private int totalEntries;
    private int startEntry;
    private int endEntry;
    private async Task LoadItems()
    {
        try
        {
            //var response = await Http.GetFromJsonAsync<PageList<BookDTO>>($"api/books/get-books-paged?SearchTerm={filter.SearchTerm}&SortColumn={filter.SortColumn}&SortOrder={filter.SortOrder}&Page={filter.Page}&PageSize={filter.PageSize}");
            var response = await service.GetPagedEntities(filter);
            pageList = response;
            users = pageList.Items;

            totalEntries = pageList.TotalCount;
            startEntry = (filter.Page - 1) * filter.PageSize + 1;
            endEntry = startEntry + users.Count - 1;

            CalculatePageRange();
            StateHasChanged();
        }
        catch (BadRequestException ex)
        {
            NavigationManager.NavigateTo("/login");
            toastService.ShowError($"You dont have access to this page : {ex.Message}");
            await Task.Delay(1000);
        }
        catch (NotFoundException ex)
        {
            toastService.ShowError($"{ex.Message}");
        }
        catch (InternalServerErrorException ex)
        {
            toastService.ShowError($"{ex.Message}");
        }
        catch (HttpRequestException e)
        {
            toastService.ShowError($"{e.Message}");
        }
        catch (Exception e)
        {
            toastService.ShowError($"{e.Message}");
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
    private void SelectUser(Guid userId)
    {
        selectedUserId = userId;
    }
    private async Task GoToPage(int page)
    {
        filter.Page = page;
        await LoadItems();
    }
	private async Task ExportToExcel()
	{
		var filterdto = filter;
		filterdto.Page = 1;
		filterdto.PageSize = totalEntries;
		var response = await service.GetPagedEntities(filterdto);
		if (response is not null)
		{

			await ExcelService.Export(response.Items, "Users.xlsx",columnsToExport);
		}
		else
		{
			await ExcelService.Export(users, "Users.xlsx", columnsToExport);
		}

	}
	private async Task ExportToPdf()
	{
		var filterdto = filter;
		filterdto.Page = 1;
		filterdto.PageSize = totalEntries;
		var response = await service.GetPagedEntities(filterdto);
		if (response is not null)
		{
			await PdfService.Export(response.Items, "Users", false,columnsToExport);
		}
		else
		{
			await PdfService.Export(users, "Users", false, columnsToExport);
		}

	}
	private void ExportToCsv()
	{
		using (var memoryStream = new MemoryStream())
		{
			using (var writer = new StreamWriter(memoryStream))
			{
				var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture);
				using (var csv = new CsvHelper.CsvWriter(writer, config))
				{
					csv.Context.RegisterClassMap<UserDTOMap>();
					csv.WriteRecords(users);
				}

				var arr = memoryStream.ToArray();
				JS.SaveAs("users.csv", arr);
			}
		}
	}
	public class UserDTOMap : ClassMap<UserDTO>
	{
		public UserDTOMap()
		{
            
            var name = columns.ElementAt(0).Key;
			var email = columns.ElementAt(1).Key;
			var role = columns.ElementAt(2).Key;

			Map(m => m.Name).Index(0).Name(name);
			Map(m => m.Email).Index(1).Name(email);
			Map(m => m.Role).Index(2).Name(role);
		}
	}

	private void ViewDetails(Guid userId)
    {
        SelectUser(userId);
        NavigationManager.NavigateTo($"/users/details/{userId}");
    }
    private void EditUser(Guid userId)
    {
        SelectUser(userId);
        NavigationManager.NavigateTo($"/users/edit/{userId}");

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
    private async Task OnDeleteDialogClose(bool accepted)
    {
        try
        {
            if (accepted)
            {
                await service.DeleteUserAsync(deleteUserId);
                await LoadItems();
            }
        }
        catch (ForbiddenException e)
        {
            toastService.ShowError($"Forbidden access to delete");
        }
        catch (Exception e)
        {
            toastService.ShowError(e.Message);
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
        deleteUserId = bookId;
        StateHasChanged();
    }
}
