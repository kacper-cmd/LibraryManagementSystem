using Application.DTOs;
using Blazored.FluentValidation;
using Client.Services;
using Infrastructure.Constants;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.JsonPatch;
using System.Data;
using Microsoft.JSInterop;

namespace Client.Components.Pages.User
{
	public partial class EditUser
	{
		#region Properties
		[Parameter]
		public Guid UserId { get; set; }
		[Inject]
		private NavigationManager NavigationManager { get; set; }
		[Inject]
		private IUserService userService { get; set; }

		[Inject]
		private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
		public UserDTO user = new UserDTO();
		private UserDTO originalUser = new UserDTO();
		private bool isAuthorized;

		#endregion
		protected override async Task OnInitializedAsync()
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			var userIdentity = authState.User;
			var roles = CustomRoles.ExtractRole(CustomRoles.AdminOrLibrarian);
			foreach (var role in roles)
			{
				if (userIdentity.IsInRole(CustomRoles.Librarian))
				{
					isAuthorized = true;
					break;
				}
				else
				{
					isAuthorized = false;
				}
			}
			if (!isAuthorized)
			{
				NavigationManager.NavigateTo("/not-authorized");
			}
			try
			{
				user = await userService.GetUserAsync(UserId);
				originalUser = user.DeepCopy();
			}
			catch (Exception e)
			{
				toastService.ShowError(e.Message);
			}

		}
		// private FluentValidationValidator? _fluentValidationValidator;

		//private async Task HandleValidSubmit()
		//{
		//    if (await _fluentValidationValidator!.ValidateAsync())
		//    {
		//        try
		//        {
		//            var patchDocument = CreatePatchDocument();
		//            if (patchDocument.Operations.Count > 0)
		//            {
		//                var response = await userService.PatchUserAsync(UserId, patchDocument);
		//            }
		//            NavigationManager.NavigateTo("/users");
		//        }
		//        catch (BadHttpRequestException e)
		//        {
		//            toastService.ShowError(e.Message);
		//        }
		//        catch (Exception e)
		//        {
		//            toastService.ShowError(e.Message);

		//        }
		//    }
		//}
		private async Task HandleValidSubmit()
		{
			try
			{
				var patchDocument = CreatePatchDocument();
				if (patchDocument.Operations.Count > 0)
				{
					var response = await userService.PatchUserAsync(UserId, patchDocument);
				}
				NavigationManager.NavigateTo("/users");
			}
			catch (BadHttpRequestException e)
			{
				toastService.ShowError(e.Message);
			}
			catch (Exception e)
			{
				toastService.ShowError(e.Message);
			}
		}

		private JsonPatchDocument<UserDTO> CreatePatchDocument()
		{
			var patchDocument = new JsonPatchDocument<UserDTO>();

			if (user.Password != originalUser.Password) patchDocument.Replace(b => b.Password, user.Password);
			if (user.Name != originalUser.Name) patchDocument.Replace(b => b.Name, user.Name);
			if (user.Email != originalUser.Email) patchDocument.Replace(b => b.Email, user.Email);
			if (user.Role != originalUser.Role) patchDocument.Replace(b => b.Role, user.Role);

			return patchDocument;
		}

		public void Cancel()
		{
			NavigationManager.NavigateTo("/users");
		}
	}
}
