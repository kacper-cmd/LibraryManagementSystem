using Infrastructure.Entities;
namespace Application.Interfaces.Repositories
{
	public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetBooks();
        Task<Book?> GetBook(Guid bookId);
        Task<Book> AddBook(Book book);
        Task<Book?> UpdateBook(Book updatedBook);
        Task DeleteBook(Guid bookId);
        IQueryable<Book> GetBooksAsQueryable(string keyword = "");
        IQueryable<Book> GetBooksAsQueryableWithFilter(string keyword = "", int page = 1, int pageSize = 10);

		Task ImportBooks(IEnumerable<Book> books);
	}
}