using Microsoft.JSInterop;
using System.Net.Http;

namespace Client.Services
{
	public interface IFontLoader
	{
		Stream GetFontStream(string fontFileName);
	}
	public class FontLoader : IFontLoader
	{
		private readonly IWebHostEnvironment _env;
		public FontLoader(IWebHostEnvironment env)
		{
			_env = env;
		}
		public Stream GetFontStream(string fontFileName)
		{
			var fontFilePath = Path.Combine(_env.WebRootPath, "fonts", fontFileName);
			var fontStream = new FileStream(fontFilePath, FileMode.Open, FileAccess.Read);
			return fontStream;
		}
	}
}
