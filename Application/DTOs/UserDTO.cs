using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserDTO : BaseDTO
    {
        public UserDTO() { }
        public UserDTO(Guid id, string name, string email, string role)
        {
            ID = id;
            Name = name;
            Email = email;
            Role = role;
        }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
		public string Password { get; set; }
        public UserDTO DeepCopy()
        {
            var clone = this.MemberwiseClone() as UserDTO;
            clone.Name = new string(Name);
            clone.Email = new string(Email);
            clone.Role = new string(Role);
            clone.Password = new string(Password);
            return clone;
        }
    }
}
