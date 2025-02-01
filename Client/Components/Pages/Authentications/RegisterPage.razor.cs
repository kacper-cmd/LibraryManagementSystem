using Application.DTOs;
using Client.Services;
using Client.States;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using static Application.DTOs.CustomResponses;

namespace Client.Components.Pages.Authentications
{
    public partial class RegisterPage : ComponentBase
    {
		[CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; }

		[Inject]
        private IAccountService accountService { get; set; }
		public bool ShowLoadingButton { get; set; } = false;
		public RegisterDTO Register = new();
        async Task RegisterClicked()
        {
            try
            {
                ShowLoadingButton = true;
                RegistrationResponse response = await accountService.RegisterAsync(Register);
                if (!response.Flag)
                {
                    await JS.InvokeVoidAsync("alert", response.Message);
					ShowLoadingButton = false;
					return;
                }
                await JS.InvokeVoidAsync("alert", response.Message);
                Register = new();
				navigationManager.NavigateTo("/login", forceLoad: true);
				return;

            }
            catch (Exception e)
            {
				ShowLoadingButton = false;
				await JS.InvokeVoidAsync("alert", e.Message);
            }
        }

		private async Task CheckUserAuthentication()
		{
			var user = (await AuthenticationState).User;
			bool isUserAuthenticated = user.Identity!.IsAuthenticated;
			if (isUserAuthenticated)
				navigationManager.NavigateTo("/dashboard");
		}
	}
}

