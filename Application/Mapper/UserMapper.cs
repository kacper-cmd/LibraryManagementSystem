using Application.DTOs;
using Infrastructure.Entities;

namespace Application.Mapper
{
    public static class UserMapper
    {
		/// <summary>
		/// Map User entity to dto User . See <see cref="User"/> class for details.
		/// </summary>
		/// <param name="user">extends user type to provide projection between  db entities and dto </param>
		/// <returns>Converted Dto </returns>
		public static UserDTO MapToDTO(this User user)
        {
            return new UserDTO
            {
                Role = user.Role,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                ID = user.ID
            };
        }
        public static IEnumerable<UserDTO> MapToDTO(this IEnumerable<User> query)
        {
            return query.Select(x => new UserDTO
            {
                ID = x.ID,
                Name = x.Name,
                Email = x.Email,
                Role = x.Role,
                Password = x.Password,
            });
        }
        public static User MapToDomainModel(this UserDTO userDTO)
        {
            return new User
            {
                ID = userDTO.ID,
                Name = userDTO.Name,
                Email = userDTO.Email,
                Role = userDTO.Role,
                Password= userDTO.Password,
            };
        }
        public static IEnumerable<User> MapToDomainModel(this IEnumerable<UserDTO> query)
        {
            return query.Select(x => new User
            {
                ID = x.ID,
                Name = x.Name,
                Email = x.Email,
                Role = x.Role,
                Password = x.Password,
            });
        }

    }
}
