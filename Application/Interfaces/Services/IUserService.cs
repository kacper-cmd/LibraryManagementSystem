using Application.DTOs;
using Application.Mapper;
using Application.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IUserService
    {
		ValueTask<IEnumerable<UserDTO>> GetUsers();
		ValueTask<UserDTO> GetUser(Guid userId);
		ValueTask<UserDTO> AddUser(UserDTO dto);
		ValueTask<UserDTO> UpdateUser(UserDTO dto);
		ValueTask DeleteUser(Guid userId);
        PagedListDTO<UserDTO> GetPagedListAsync(BaseFilter filter);
    }
}