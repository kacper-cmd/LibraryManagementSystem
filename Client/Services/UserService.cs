using Application.DTOs;
using Application.RequestModel;
using Client.Helpers;
using Microsoft.AspNetCore.JsonPatch;

namespace Client.Services;
public class UserService : IUserService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IRequestHelper _requestHelper;
	public UserService(IHttpClientFactory httpClientFactory, IRequestHelper request)
	{
		_httpClientFactory = httpClientFactory;
		_requestHelper = request;
	}

	public async Task<List<UserDTO>> GetAll()
	{
		using (var client = _httpClientFactory.CreateClient("client"))
		{
			var result = await _requestHelper.MakeHttpRequestAsync<object, List<UserDTO>>(
			 client, "get-users-paged", HttpMethod.Get, null, true);
			return result;
		}
	}

	public async Task<PagedListDTO<UserDTO>> GetPagedEntities(BaseFilter filter)
	{
		using (var client = _httpClientFactory.CreateClient("client"))
		{
			var result = await _requestHelper.MakeHttpRequestAsync<BaseFilter, PagedListDTO<UserDTO>>(
			 client, "user/get-users-paged", HttpMethod.Get, filter, true);
			return result;
		}
	}

	public async Task<UserDTO> CreateUserAsync(UserDTO book)
	{
		using (var client = _httpClientFactory.CreateClient("client"))
		{
			var result = await _requestHelper.MakeHttpRequestAsync<UserDTO, UserDTO>(client, "user", HttpMethod.Post, book, true);
			return result;
		}
	}
	public async Task<UserDTO> UpdateUserAsync(UserDTO book)
	{
		using (var client = _httpClientFactory.CreateClient("client"))
		{
			var result = await _requestHelper.MakeHttpRequestAsync<UserDTO, UserDTO>(client, $"user/{book.ID}", HttpMethod.Put, book, true);
			return result;
		}
	}

	public async Task DeleteUserAsync(Guid id)
	{
		using (var client = _httpClientFactory.CreateClient("client"))
		{
			var result = await _requestHelper.MakeHttpRequestAsync<string, Guid>(client, $"user/{id}", HttpMethod.Delete, null, true);
			//return result;
		}
	}
    public async Task<UserDTO> PatchUserAsync(Guid id, JsonPatchDocument<UserDTO> patchDocument)
    {
        using (var client = _httpClientFactory.CreateClient("client"))
        {
            var result = await _requestHelper.MakeHttpRequestAsyncPatch<UserDTO>(client, $"user/{id}", patchDocument, true, null);
            return result;
        }
    }
    public async Task<UserDTO> GetUserAsync(Guid bookId)
	{
		using (var client = _httpClientFactory.CreateClient("client"))
		{
			var result = await _requestHelper.MakeHttpRequestAsync<object, UserDTO>(client, $"user/{bookId}", HttpMethod.Get, null, true);
			return result;
		}
	}

}