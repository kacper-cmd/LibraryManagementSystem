using Application.DTOs;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Client.Components.Pages.User
{
	public partial class UserForm
	{
		[Parameter] public UserDTO User { get; set; }
		[Parameter] public EventCallback OnValidSubmit { get; set; }
		[Parameter] public EventCallback Cancel { get; set; }
		[Parameter]
		public string FormTitle { get; set; }
		[Parameter]
		public string ButtonText { get; set; }

		private FluentValidationValidator? _fluentValidationValidator;


		private async Task HandleSubmit()
		{
			if (await _fluentValidationValidator!.ValidateAsync())
			{
				await OnValidSubmit.InvokeAsync();
			}
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				await JS.InvokeVoidAsync("focusById", "Name");
			}
		}
	}
}
