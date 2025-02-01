using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
	public class UserSession
	{
		public string JWTToken { get; set; } = string.Empty;
	}
}
