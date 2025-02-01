using Microsoft.JSInterop;

namespace Client.Helpers
{
	public static class IJSRuntimeExtensions
	{
		public static ValueTask SaveAs(this IJSRuntime js, string fileName, byte[] content)
		{
			return js.InvokeVoidAsync("saveAsFileBase64", fileName, Convert.ToBase64String(content));
		}
	}
}
