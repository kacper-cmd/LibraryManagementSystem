using Application.DTOs;
using Application.RequestModel;
using Microsoft.AspNetCore.JsonPatch;

namespace Client.Services
{
	public interface IUserService
	{
		Task<PagedListDTO<UserDTO>> GetPagedEntities(BaseFilter filter);
		Task<List<UserDTO>> GetAll();
		Task<UserDTO> CreateUserAsync(UserDTO book);
		Task<UserDTO> UpdateUserAsync(UserDTO book);
		Task DeleteUserAsync(Guid id);
		Task<UserDTO> PatchUserAsync(Guid id, JsonPatchDocument<UserDTO> patchDocument);
		Task<UserDTO> GetUserAsync(Guid Id);
	}
}
