using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
	public class CustomResponses
	{
		public record RegistrationResponse(bool Flag = false, string Message = null!);
		public record LoginResponse(bool Flag = false, string Message = null!, string JWTToken = null!);
	}
}
