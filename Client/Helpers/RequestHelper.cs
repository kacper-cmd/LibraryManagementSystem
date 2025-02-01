using System.Text.Json;
using System.Text;
using Client.Services;
using System.Net.Http.Headers;
using Client.States;
using Infrastructure.Constants;
using Application.DTOs;
using static Application.DTOs.CustomResponses;
using System.Net.Http;
using Microsoft.AspNetCore.JsonPatch;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Entities;
using Blazored.LocalStorage;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Office2010.Excel;
namespace Client.Helpers
{

	public class RequestHelper : IRequestHelper
	{
		private readonly JsonSerializerOptions _options;
		private readonly Services.ILocalStorageService _localStorageService;
		public RequestHelper(Services.ILocalStorageService localStorageService)
		{
			_options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				PropertyNameCaseInsensitive = true,
				DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
				ReadCommentHandling = JsonCommentHandling.Skip
			};
			_localStorageService = localStorageService;
		}
		/// <summary>
		/// Generic universal method to handle http request 
		/// </summary>
		/// <typeparam name="TRequest">generic type to send </typeparam>
		/// <typeparam name="TResponse">generic type that will arrive from endpoint</typeparam>
		/// <param name="client">creteated using httpclientfactory</param>
		/// <param name="url">address to send httprequest</param>
		/// <param name="httpMethod">post, get, put, delete</param>
		/// <param name="requestBody">Data to send</param>
		/// <param name="handleErrors">flag to proccess errors and auto refersh token</param>
		/// <param name="token">optional parameter</param>
		/// <returns>response generic type from enpoint </returns>
		/// <example>
		/// <code>
		///   using (var client = _httpClient.CreateClient("client"))
		///   {
		///   var result = await _requestHelper.MakeHttpRequestAsync<object, List<BookDTO>>(
		///    client, "get-books-paged", HttpMethod.Get, null, true);
		///    return result;
		///   }
		/// </code>
		/// </example>
	public async Task<TResponse> MakeHttpRequestAsync<TRequest, TResponse>(
			HttpClient client,
			string url,
			HttpMethod httpMethod,
			TRequest requestBody = default,
			bool handleErrors = false,
			string? token = null)
		{
			token = await GetTokenAsync();
			var request = CreateHttpRequestMessage(client.BaseAddress!, url, httpMethod, requestBody, token);

			var response = await client.SendAsync(request);

			if (handleErrors && !response.IsSuccessStatusCode)
			{
				if (CheckIfUnauthorized(response))
				{
					await GetRefreshToken(client);
					token = await GetTokenAsync();
					//rodzielic bo System.InvalidOperationException: 'The request message was already sent. Cannot send the same request message multiple times.
					request = CreateHttpRequestMessage(client.BaseAddress!, url, httpMethod, requestBody, token);
					response = await client.SendAsync(request);
				}

				if (!response.IsSuccessStatusCode)
				{
				  await HandleHttpErrors(response);
				}
			}

			var responseJson = await response.Content.ReadAsStringAsync();
			var responseData = JsonSerializer.Deserialize<TResponse>(responseJson, _options);

			return responseData;
		}
		private HttpRequestMessage CreateHttpRequestMessage<TRequest>(
			Uri baseAddress,
			string url,
			HttpMethod method,
			TRequest requestBody,
			string? token)
		{
			var request = new HttpRequestMessage
			{
				Method = method,
				RequestUri = new Uri(baseAddress, url)
			};

			if (!string.IsNullOrEmpty(token))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}

			if (requestBody != null)
			{
				var json = JsonSerializer.Serialize(requestBody);
				request.Content = new StringContent(json, Encoding.UTF8, "application/json");
			}

			return request;
		}
		private async Task<string> GetTokenAsync()
		{
			return await _localStorageService.GetItem(Constants.JWTTokenName);
		}
		private async Task<string> HandleHttpErrors(HttpResponseMessage response)
		{
			var responseJson = await response.Content.ReadAsStringAsync();
			switch (response.StatusCode)
			{
				case System.Net.HttpStatusCode.BadRequest:
					return responseJson;
				case System.Net.HttpStatusCode.NotFound:
					return responseJson;
				case System.Net.HttpStatusCode.InternalServerError:
					return responseJson;
				default:
					throw new HttpRequestException($"HTTP request failed with status code {response.StatusCode}");
			}
		}
		private bool CheckIfUnauthorized(HttpResponseMessage response)
		{
			return response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
		}
		private async Task GetRefreshToken(HttpClient client)
		{
			var jwtoken = await _localStorageService.GetItem(Constants.JWTTokenName);
			var userSession = new UserSession { JWTToken = jwtoken };

			var result = await MakeHttpRequestAsync<UserSession, LoginResponse>(client, "account/refresh-token", HttpMethod.Post, userSession, true);
			await _localStorageService.SetItem(Constants.JWTTokenName, result?.JWTToken);
		}

		/// <summary>
		/// Generic universal method using ApiResponse<T> as response to handle http request 
		/// </summary>
		/// <typeparam name="TRequest">generic type to send </typeparam>
		/// <typeparam name="TResponse">generic type that will arrive from endpoint wrapped into ApiResponse </typeparam>
		/// <param name="client">creteated using httpclientfactory</param>
		/// <param name="url">address to send httprequest</param>
		/// <param name="httpMethod">post, get, put, delete</param>
		/// <param name="requestBody">Data to send</param>
		/// <param name="handleErrors">flag to proccess errors and auto refersh token</param>
		/// <param name="token">optional parameter</param>
		/// <returns>response generic type from enpoint </returns>
		/// <example>
		/// <code>
		///   using (var client = _httpClient.CreateClient("client"))
		///   {
		///   var result = await _requestHelper.MakeHttpRequestApiResponseAsync<string, BookDTO>(client, $"book/delete-book/{id}", HttpMethod.Delete, null, true);
		///   return result;
		///   }
		/// </code>
		/// </example>
		#region UsingWithApiResponse<T>
		public async Task<ApiResponse<TResponse>> MakeHttpRequestApiResponseAsync<TRequest, TResponse>(
																	HttpClient client,
																	string url,
																	HttpMethod httpMethod,
																	TRequest requestBody = default,
																	bool handleErrors = false,
																	string? token = null)
		{
			token = await GetTokenAsync();
			var request = CreateHttpRequestMessage(client.BaseAddress!, url, httpMethod, requestBody, token);

			var response = await client.SendAsync(request);

			if (handleErrors && !response.IsSuccessStatusCode)
			{
				if (CheckIfUnauthorized(response))
				{
					await GetRefreshToken(client);
					token = await GetTokenAsync();
					request = CreateHttpRequestMessage(client.BaseAddress!, url, httpMethod, requestBody, token);
					response = await client.SendAsync(request);
				}

				if (!response.IsSuccessStatusCode)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					//var apiResponse = JsonSerializer.Deserialize<ApiResponse<TResponse>>(errorContent, _options);
					var apiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse<TResponse>>(errorContent);
					
					if (apiResponse != null && !apiResponse.Success)
					{
						throw new ValidationErrorsException(apiResponse.ValidationErrors);
					}

					await HandleHttpErrors(response);
				}
			}

			var responseJson = await response.Content.ReadAsStringAsync();
			//var responseData = JsonSerializer.Deserialize<ApiResponse<TResponse>>(responseJson, _options);
			var responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse<TResponse>>(responseJson);
			if (responseData == null || !responseData.Success)
			{
				throw new ValidationErrorsException(responseData?.ValidationErrors ?? new List<string> { "Errors" });
			}
			return responseData;
		} 
		#endregion


	

		/// <summary>
		/// for patch request 
		/// </summary>
		/// <typeparam name="TResponse"></typeparam>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <param name="patchDocument"></param>
		/// <param name="handleErrors"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<TResponse> MakeHttpRequestAsyncPatch<TResponse>(
										HttpClient client,
										string url,
										JsonPatchDocument<TResponse> patchDocument,
										bool handleErrors = false,
										string? token = null) where TResponse : class
		{
			token = await GetTokenAsync();
			var request = CreatePatchRequest(client.BaseAddress!, url, patchDocument, token);

			var response = await client.SendAsync(request);

			if (handleErrors && !response.IsSuccessStatusCode)
			{
				if (CheckIfUnauthorized(response))
				{
					await GetRefreshToken(client);

					token = await GetTokenAsync();
					request = CreatePatchRequest(client.BaseAddress!, url, patchDocument, token);

					response = await client.SendAsync(request);
				}

				if (!response.IsSuccessStatusCode)
				{
			     	await HandleHttpErrors(response);
				}
			}

			var responseJson = await response.Content.ReadAsStringAsync();
			var responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(responseJson);

			return responseData;
		}

		private HttpRequestMessage CreatePatchRequest<TResponse>(
			Uri baseAddress,
			string url,
			JsonPatchDocument<TResponse> patchDocument,
			string? token) where TResponse : class
		{
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Patch,
				RequestUri = new Uri(baseAddress, url),
				Content = new StringContent(
					Newtonsoft.Json.JsonConvert.SerializeObject(patchDocument),
					Encoding.UTF8,
					"application/json-patch+json")
			};

			if (!string.IsNullOrEmpty(token))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}

			return request;
		}
		//public async Task<TResponse> MakeHttpRequestAsync<TRequest, TResponse>(HttpClient client, string url, HttpMethod httpMethod, TRequest requestBody = default, bool handleErrors = false, string? token = null)
		//{
		//    var request = new HttpRequestMessage
		//    {
		//        Method = httpMethod,
		//        RequestUri = new Uri(client.BaseAddress!, url)
		//    };
		//    token = await _localStorageService.GetItem(Constants.JWTTokenName);

		//    if (!string.IsNullOrEmpty(token))
		//    {
		//        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		//    }

		//    if (requestBody != null)
		//    {
		//        var json = JsonSerializer.Serialize(requestBody);
		//        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
		//    }

		//    var response = await client.SendAsync(request);

		//    if (handleErrors && !response.IsSuccessStatusCode)
		//    {
		//        if (CheckIfUnauthorized(response))
		//        {
		//            await GetRefreshToken(client);

		//            token = await _localStorageService.GetItem(Constants.JWTTokenName);

		//            if (!string.IsNullOrEmpty(token))
		//            {
		//                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		//            }

		//            response = await client.SendAsync(request);
		//        }

		//        if (!response.IsSuccessStatusCode)
		//        {
		//            switch (response.StatusCode)
		//            {
		//                case System.Net.HttpStatusCode.BadRequest:
		//                    throw new BadRequestException("Bad Request");
		//                case System.Net.HttpStatusCode.NotFound:
		//                    throw new NotFoundException("Not Found");
		//                case System.Net.HttpStatusCode.InternalServerError:
		//                    throw new InternalServerErrorException("Internal Server Error");
		//                default:
		//                    throw new HttpRequestException($"HTTP request failed with status code {response.StatusCode}");
		//            }
		//            // throw new HttpRequestException($"HTTP request failed with status code {response.StatusCode}");
		//        }
		//    }
		//    var responseJson = await response.Content.ReadAsStringAsync();
		//    var responseData = JsonSerializer.Deserialize<TResponse>(responseJson, _options);

		//    return responseData;
		//}

	}
}

//using (var httpClient = _httpClientFactory.CreateClient("GitHub")) //service 
//{
//}