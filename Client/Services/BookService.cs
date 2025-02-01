using Application.DTOs;
using Application.RequestModel;
using Client.Helpers;
using Infrastructure.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Xml.XPath;

namespace Client.Services
{
	public class BookService : IBookService
    {
        //strzyc client factory          var httpClient = _httpClientFactory.CreateClient("GitHub"); ktorty uzyje predefiniowane httpclienta 
        //wrzykuje generyczny servuce do IREquest 
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRequestHelper _requestHelper;
        public BookService(IHttpClientFactory httpClientFactory, IRequestHelper request)
        {
            _httpClientFactory = httpClientFactory;
            _requestHelper = request;
        }

        public async Task<List<BookDTO>> GetAll()
        {
            using (var client = _httpClientFactory.CreateClient("client"))
            {
                var result = await _requestHelper.MakeHttpRequestAsync<object, List<BookDTO>>(
				 client, "get-books-paged", HttpMethod.Get, null, true);
                return result;
            }
        }
        public async Task<PagedListDTO<BookDTO>> GetPagedEntitiesQuery(BaseFilter filter)
        {
            using (var client = _httpClientFactory.CreateClient("client"))
            {
				var query = $"?page={filter.Page}&pageSize={filter.PageSize}&searchTerm={filter.SearchTerm}&searchColumn={filter.SearchColumn}&sortColumn={filter.SortColumn}&sortOrder={filter.SortOrder}";
                var result = await _requestHelper.MakeHttpRequestAsync<object, PagedListDTO<BookDTO>>(
                 client, $"book/get-books-pagedquery{query}", HttpMethod.Get, null, true);
                return result;
            }
        }
        public async Task<PagedListDTO<BookDTO>> GetPagedEntities(BaseFilter filter)
        {
            using (var client = _httpClientFactory.CreateClient("client"))
            {
                var result = await _requestHelper.MakeHttpRequestAsync<BaseFilter, PagedListDTO<BookDTO>>(
				 client, "book/get-books-paged", HttpMethod.Get, filter, true);
                return result;
            }
        }

		public async Task<BookDTO> CreateBookAsync(BookDTO book)
		{
			using (var client = _httpClientFactory.CreateClient("client"))
			{
				var result = await _requestHelper.MakeHttpRequestAsync<BookDTO, BookDTO>(client, "book", HttpMethod.Post, book, true);
				return result;
			}
		}
		public async Task<BookDTO> UpdateBookAsync(BookDTO book)
		{
			using (var client = _httpClientFactory.CreateClient("client"))
			{
				var result = await _requestHelper.MakeHttpRequestAsync<BookDTO, BookDTO>(client, $"book/{book.ID}", HttpMethod.Put, book, true);
				return result;
			}
		}

		public async Task DeleteBookAsync(Guid id)
		{
			using (var client = _httpClientFactory.CreateClient("client"))
			{
				var result =await _requestHelper.MakeHttpRequestAsync<string, Guid>(client, $"book/{id}", HttpMethod.Delete, null, true);
				//return result;
			}
		}
		public async Task<ApiResponse<BookDTO>> DeleteBookApiResponseAsync(Guid id)
		{
			using (var client = _httpClientFactory.CreateClient("client"))
			{
				var result = await _requestHelper.MakeHttpRequestApiResponseAsync<string, BookDTO>(client, $"book/delete-book/{id}", HttpMethod.Delete, null, true);
				return result;
			}
		}
		public async Task<BookDTO> PatchBookAsync(Guid id, JsonPatchDocument<BookDTO> patchDocument)
        {
            using (var client = _httpClientFactory.CreateClient("client"))
            {
                var result = await _requestHelper.MakeHttpRequestAsyncPatch<BookDTO>(client, $"book/{id}", patchDocument, true, null);
                return result;
            }
        }
	
		public async Task<BookDTO> GetBookAsync(Guid bookId)
		{
			using (var client = _httpClientFactory.CreateClient("client"))
			{
				var result = await _requestHelper.MakeHttpRequestAsync<object, BookDTO>(client, $"book/{bookId}", HttpMethod.Get, null, true);
				return result;
			}
		}

		public async Task ImportBooks(List<BookDTO> books)
		{
			using (var client = _httpClientFactory.CreateClient("client"))
			{
				var result = await _requestHelper.MakeHttpRequestAsync<object, BookDTO>(client, $"book/import-books", HttpMethod.Post, books, true);
				
			}
		}
	}
}