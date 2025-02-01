using Application.DTOs;
using Application.RequestModel;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IBookService
    {
		ValueTask<PagedListDTO<BookDTO>> GetPagedListAsync(BaseFilter filter);
		ValueTask<Result<PagedListDTO<BookDTO>, Error>> GetPagedListAsync2(BaseFilter filter);
		ValueTask<Result<Book, Error>> UpdateDiffrentApproach(BookDTO model);
		ValueTask<IEnumerable<BookDTO>> GetBooks();
		ValueTask<BookDTO> GetBook(Guid bookId);
		ValueTask<BookDTO> AddBook(BookDTO dto);
		ValueTask<BookDTO> UpdateBook(BookDTO dto);
		ValueTask DeleteBook(Guid bookId);
		ValueTask ImportBooks(List<BookDTO> books);

	}
}
