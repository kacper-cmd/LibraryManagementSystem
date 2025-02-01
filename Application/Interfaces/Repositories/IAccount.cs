using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.DTOs.CustomResponses;

namespace Application.Interfaces.Repositories
{
	public interface IAccount
	{
		Task<RegistrationResponse> RegisterAsync(RegisterDTO model);
		Task<LoginResponse> LoginAsync(LoginDTO user);
		LoginResponse RefreshToken(UserSession userSession);

	}
}
