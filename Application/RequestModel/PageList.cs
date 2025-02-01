using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.RequestModel
{
    public class PageList<T>
    {
        private PageList(List<T> items, int page, int pageSize, int totalCount)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
        [JsonPropertyName(nameof(Items))]   
        public List<T> Items { get; }
		[JsonPropertyName(nameof(Page))]
		public int Page { get; }
		[JsonPropertyName(nameof(PageSize))]
		public int PageSize { get; }
		[JsonPropertyName(nameof(TotalCount))]
		public int TotalCount { get; }
		[JsonPropertyName(nameof(HasNextPage))]
		public bool HasNextPage => Page * PageSize < TotalCount;
		[JsonPropertyName(nameof(HasPreviousPage))]
		public bool HasPreviousPage => Page > 1;
		public static PageList<T> Create(IQueryable<T> query, int page, int pageSize)
        {
            var totalCount = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PageList<T>(items, page, pageSize, totalCount);
        }

    }
}
