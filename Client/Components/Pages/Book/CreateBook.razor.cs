using Microsoft.AspNetCore.Components;
using Application.DTOs;
using Client.Services;
using Blazored.FluentValidation;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;
namespace Client.Components.Pages.Book;

public partial class CreateBook : ComponentBase
{
	#region Properties
	[Inject]
	private NavigationManager NavigationManager { get; set; }
	[Inject]
	private IBookService bookService { get; set; }
	[Inject]
	private IExcelService ExcelService { get; set; }
	private BookDTO book = new BookDTO();
	#endregion

	private async Task HandleValidSubmit()
	{
		try
		{
			var response = await bookService.CreateBookAsync(book);
			if (response is not null)
			{
				toastService.ShowSuccess("Form Submitted Successfully!");
				NavigationManager.NavigateTo("/books");
			}
			toastService.ShowSuccess("Form Submitted Successfully!");
		}
		catch (Exception e)
		{
			toastService.ShowError(e.Message);
		}
	}
    List<string> messages = new();
    private async Task FileUploaded(InputFileChangeEventArgs e)
	{
        messages.Clear();

        if (e.File.Name.EndsWith(".xlsx"))
		{
			using (MemoryStream ms = new MemoryStream())
			{
				await e.GetMultipleFiles(1).First().OpenReadStream().CopyToAsync(ms);
				ms.Position = 0;

				List<BookDTO> uploadedList = await ExcelService.Read<BookDTO>(ms);
				await bookService.ImportBooks(uploadedList);
				NavigationManager.NavigateTo("/books");
			}
		}
		else
		{
			messages.Add($"File: {e.File.Name}, type: .{e.File.Name.Split('.')[1].ToString()} not allowed");
        }

	}
	private void Cancel()
	{
		NavigationManager.NavigateTo("/books");
	}
}
