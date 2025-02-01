using Microsoft.JSInterop;

namespace Client.Services
{
	public interface ILocalStorageService
	{
		Task SetItem(string name, string token);
		Task<string> GetItem(string name);
		Task RemoveItem(string name);
	}
}
