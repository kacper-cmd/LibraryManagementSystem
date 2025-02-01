using Microsoft.AspNetCore.Components;
namespace Client.Components.Pages.Authentications;
using Application.DTOs;
using Client.Services;
using Client.States;
using Infrastructure.Constants;
using Microsoft.JSInterop;
using static Application.DTOs.CustomResponses;

public partial class LoginPage : ComponentBase
{
	public LoginDTO Login = new();
	[Inject]
	private NavigationManager NavigationManager { get; set; }
	[Inject]
	private  IAccountService accountService { get; set; }
    [Inject]
    private ILocalStorageService localStorageService { get; set; }
	public bool ShowLoadingButton { get; set; } = false;

	async Task LoginClicked()
	{
        try
        {
            ShowLoadingButton = true;
            LoginResponse response = await accountService.LoginAsync(Login);
            if (!response.Flag)
            {
                await JS.InvokeVoidAsync("alert", response.Message);
				ShowLoadingButton = false;
				return;
            }
            var customAuthStateProvider = (CustomAuthenticationStateProvider)AuthStateProvider;
            customAuthStateProvider.UpdateAuthenticationState(response.JWTToken);

            NavigationManager.NavigateTo("/dashboard", forceLoad: true);
        }
        catch (Exception e)
        {
			ShowLoadingButton = false;
			await JS.InvokeVoidAsync("alert", e.Message);

        }
    }
    protected override async Task OnInitializedAsync()
    {
        await CheckUserAuthentication();
        var token = await localStorageService.GetItem(Constants.JWTTokenName);
        if (token is not null)
        {
            NavigationManager.NavigateTo("/dashboard");
        }
    }
}
