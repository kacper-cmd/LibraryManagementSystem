using Application.DTOs;
using static Application.DTOs.CustomResponses;
using System.Net.Http.Headers;
using Client.States;
using Client.Helpers;
using Infrastructure.Entities;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Infrastructure.Constants;

namespace Client.Services
{
	public class AccountService : IAccountService
	{
		private readonly IHttpClientFactory _httpClient;
		private readonly IRequestHelper _requestHelper;
		private readonly Services.ILocalStorageService _localStorageService;

		public AccountService(IHttpClientFactory httpClientFactory, IRequestHelper request, Services.ILocalStorageService localStorageService)
		{
			_httpClient = httpClientFactory;
			_requestHelper = request;
			_localStorageService = localStorageService;
		}
		private static bool CheckIfUnauthorized(HttpResponseMessage response)
		{
			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
				return true;
			else
				return false;
		}
        private async Task GetRefreshToken()
        {
            using (var client = _httpClient.CreateClient("client"))
            {
                var jwtoken = await _localStorageService.GetItem(Constants.JWTTokenName);
                var userSession = new UserSession { JWTToken = jwtoken };

                var result = await _requestHelper.MakeHttpRequestAsync<UserSession, LoginResponse>(client, "account/refresh-token", HttpMethod.Post, userSession, true);
                await _localStorageService.SetItem(Constants.JWTTokenName, result!.JWTToken);
            }
        }
        public async Task<LoginResponse> LoginAsync(LoginDTO user)
		{ 
			using (var client = _httpClient.CreateClient("client"))
			{
				var result = await _requestHelper.MakeHttpRequestAsync<LoginDTO, LoginResponse>(client, "account/login", HttpMethod.Post, user, true);

                await _localStorageService.SetItem(Constants.JWTTokenName,result.JWTToken);
                return result;
			}
		}
        public async Task<UserDTO> GetCurrentUser()
        {
            using (var client = _httpClient.CreateClient("client"))
            {
                var result = await _requestHelper.MakeHttpRequestAsync<object, UserDTO>(client, "account/current", HttpMethod.Get, null, true);

                return result;
            }
        }
        public async Task<RegistrationResponse> RegisterAsync(RegisterDTO model)
		{
			using (var client = _httpClient.CreateClient("client"))
			{
				var result = await _requestHelper.MakeHttpRequestAsync<RegisterDTO, RegistrationResponse>(client, "account/register", HttpMethod.Post, model, true);
				return result;
			}
		}

		public async Task<LoginResponse> RefreshToken(UserSession userSession)
		{
			using (var client = _httpClient.CreateClient("client"))
			{
				var result = await _requestHelper.MakeHttpRequestAsync<UserSession, LoginResponse>(client, "account/refresh-token", HttpMethod.Post, userSession, true);
				return result;
			}
		}
	}
}
