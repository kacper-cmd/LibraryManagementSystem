using Application.DTOs;
using Application.RequestModel;
using Infrastructure.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace Client.Services
{
    public interface IBookService
    {
        Task<PagedListDTO<BookDTO>> GetPagedEntities(BaseFilter filter);
        Task<List<BookDTO>> GetAll();
        Task<BookDTO> CreateBookAsync(BookDTO book);
        Task<BookDTO> UpdateBookAsync(BookDTO book);
        Task DeleteBookAsync(Guid id);
        Task<BookDTO> PatchBookAsync(Guid id, JsonPatchDocument<BookDTO> patchDocument);
        Task<BookDTO> GetBookAsync(Guid bookId);
        Task<PagedListDTO<BookDTO>> GetPagedEntitiesQuery(BaseFilter filter);
        Task<ApiResponse<BookDTO>> DeleteBookApiResponseAsync(Guid id);
        Task ImportBooks(List<BookDTO> books);
	}
}
