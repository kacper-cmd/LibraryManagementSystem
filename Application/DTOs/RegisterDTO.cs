using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
	public class RegisterDTO : LoginDTO
	{
		public int Id { get; set; }
		[Required,MinLength(3, ErrorMessage = "Name should be at least 3 letters long")]
		public string Name { get; set; } = string.Empty;

		[Required, Compare(nameof(Password)), DataType(DataType.Password)]
		public string ConfirmPassword { get; set; } = string.Empty;
		[Required, MinLength(3)]
		public string Role { get; set; } = string.Empty;
	}
}
