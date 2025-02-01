using Application.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace Client.Helpers
{
    public interface IRequestHelper
    {
        Task<TResponse> MakeHttpRequestAsync<TRequest, TResponse>(HttpClient httpClient, string url, HttpMethod httpMethod, TRequest requestBody = default, bool handleErrors = false, string? token = null);
        Task<TResponse> MakeHttpRequestAsyncPatch<TResponse>(HttpClient client, string url, JsonPatchDocument<TResponse> patchDocument, bool handleErrors = false, string? token = null) where TResponse : class;
        Task<ApiResponse<TResponse>> MakeHttpRequestApiResponseAsync<TRequest, TResponse>(HttpClient client, string url, HttpMethod httpMethod, TRequest requestBody = default, bool handleErrors = false, string? token = null);

	}
}
