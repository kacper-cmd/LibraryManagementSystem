namespace Client.Services
{
	/// <summary>
	/// BASE ON https://medium.com/@shahriyarali08/blazor-wasm-exporting-and-importing-data-to-excel-and-pdf-438e775da1a2
	/// </summary>
	public interface IExcelService
	{
		Task Export<T>(IEnumerable<T> data, string fileName, List<string>? columnsToExport = null);
		Task<List<T>> Read<T>(MemoryStream ms);
	}
}
