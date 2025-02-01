using Microsoft.JSInterop;

namespace Client.Services
{
	public class LocalStorageService : ILocalStorageService
	{
		private readonly IJSRuntime _jsRuntime;

		public LocalStorageService(IJSRuntime jsRuntime)
		{
			_jsRuntime = jsRuntime;
		}

		public async Task SetItem(string name, string token)
		{
			await _jsRuntime.InvokeVoidAsync("localStorage.setItem", name, token);
		}

		public async Task<string> GetItem(string name)
		{
				return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", name);

		}

		public async Task RemoveItem(string name)
		{
			await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", name);
		}
	}
}
