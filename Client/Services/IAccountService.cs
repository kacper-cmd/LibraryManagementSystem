using Application.DTOs;
using static Application.DTOs.CustomResponses;

namespace Client.Services
{
	public interface IAccountService
	{
		Task<RegistrationResponse> RegisterAsync(RegisterDTO model);
		Task<LoginResponse> LoginAsync(LoginDTO user);
		Task<LoginResponse> RefreshToken(UserSession userSession);
        Task<UserDTO> GetCurrentUser();

    }
}
