using Application.DTOs;
using Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Win32;
using static Application.DTOs.CustomResponses;

namespace Client.Components.Pages.User
{
	public partial class DetailsUser : ComponentBase
	{
		#region Properties
		[Parameter]
		public Guid UserId { get; set; }
		[Inject]
		private NavigationManager NavigationManager { get; set; }

		[Inject]
		private IUserService userService { get; set; }

        private UserDTO user = new UserDTO(); 
		#endregion
		protected override async Task OnInitializedAsync()
        {
            try
            {
                user = await userService.GetUserAsync(UserId);
            }
            catch (Exception e)
            {
                toastService.ShowError(e.Message);

            }
        }

		private void BackToList()
		{
			NavigationManager.NavigateTo("/users");
		}
	}
}
