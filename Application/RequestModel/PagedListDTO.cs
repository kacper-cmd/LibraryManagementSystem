using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.RequestModel
{
	public class PagedListDTO<T>
	{

		[JsonPropertyName(nameof(Items))]
		public List<T> Items { get; set; }
		[JsonPropertyName(nameof(Page))]
		public int Page { get; set; }
		[JsonPropertyName(nameof(PageSize))]
		public int PageSize { get; set; }
		[JsonPropertyName(nameof(TotalCount))]
		public int TotalCount { get; set; }
		[JsonPropertyName(nameof(HasNextPage))]
		public bool HasNextPage => Page * PageSize < TotalCount;
		[JsonPropertyName(nameof(HasPreviousPage))]
		public bool HasPreviousPage => Page > 1;

	}
}
