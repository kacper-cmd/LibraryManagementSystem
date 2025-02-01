using Application.DTOs;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;

namespace Client.Components.Pages.Book
{
	public partial class BookForm
	{
		[Parameter] public BookDTO Book { get; set; }
		[Parameter] public EventCallback OnValidSubmit { get; set; }
		[Parameter] public EventCallback Cancel { get; set; }

		[Parameter]
		public string FormTitle { get; set; }
		[Parameter]
		public string ButtonText { get; set; }
		[Parameter]
		public bool DisplayFileUploaded { get; set; } = false;
		[Parameter]
		public List<string>? Errors { get; set; }

		private FluentValidationValidator? _fluentValidationValidator;

		[Parameter]
		public EventCallback<InputFileChangeEventArgs> FileUploaded { get; set; }

		private async Task HandleSubmit()
		{
			if (await _fluentValidationValidator!.ValidateAsync())
			{
				await OnValidSubmit.InvokeAsync();
			}
		}
		private async Task HandleFileChange(InputFileChangeEventArgs e)
		{
			await FileUploaded.InvokeAsync(e);
		}
	}
}
