using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Repositories;
using Infrastructure.Exceptions;
using static System.Reflection.Metadata.BlobBuilder;

namespace Database.Repository;
public class BookRepository : IBookRepository
{
	private readonly ApplicationDbContext _dbContext;
	public BookRepository(ApplicationDbContext appDbContext)
	{
		ArgumentNullException.ThrowIfNull(nameof(appDbContext));
		_dbContext = appDbContext;
	}
	public async Task<IEnumerable<Book>> GetBooks()
	{
		return await _dbContext.Books.ToListAsync();
	}

	public async Task<Book?> GetBook(Guid bookId)
	{
		return await _dbContext.Books.FirstOrDefaultAsync(e => e.ID == bookId);	
	}

	public async Task<Book> AddBook(Book book)
	{

		book.DateCreated = DateTime.Now;
		var result = await _dbContext.Books.AddAsync(book);
		await _dbContext.SaveChangesAsync();
		return result.Entity;
	}

	public async Task<Book?> UpdateBook(Book updatedBook)
	{
		var existingBook = await _dbContext.Books
			.FirstOrDefaultAsync(e => e.ID == updatedBook.ID);

		ArgumentNullException.ThrowIfNull(nameof(existingBook));
			existingBook.Title = updatedBook.Title;
			existingBook.Author = updatedBook.Author;
			existingBook.ISBN = updatedBook.ISBN;
			existingBook.Available = updatedBook.Available;
			existingBook.PublishedDate = updatedBook.PublishedDate;
			existingBook.DateUpdated = DateTime.Now;
			await _dbContext.SaveChangesAsync();
			return existingBook;

	}

	public async Task DeleteBook(Guid bookId)
	{
		var existingBook = await _dbContext.Books
			.FirstOrDefaultAsync(e => e.ID == bookId);
		if (existingBook != null)
		{
			_dbContext.Books.Remove(existingBook);
			await _dbContext.SaveChangesAsync();
		}
	}


	public IQueryable<Book> GetBooksAsQueryable(string keyword = "")
	{
		IQueryable<Book> query = _dbContext.Books.AsQueryable().AsNoTracking();

		if (!string.IsNullOrEmpty(keyword))
		{
			query = query.Where(a => a.Title.Contains(keyword));
		}

		return query.OrderByDescending(a => a.PublishedDate);
	}

	public IQueryable<Book> GetBooksAsQueryableWithFilter(string keyword = "", int page = 1, int pageSize = 10)
	{
		IQueryable<Book> query = _dbContext.Books.AsQueryable().AsNoTracking();


		if (!string.IsNullOrEmpty(keyword))
		{
			query = query.Where(a => a.Title.Contains(keyword));
		}

		query = query.Skip((page - 1) * pageSize).Take(pageSize);

		return query.OrderByDescending(a => a.PublishedDate);
	}

	public async Task ImportBooks(IEnumerable<Book> books)
	{
		var toAdd = new List<Book>();

		foreach (var book in books)
		{
			var exists = await _dbContext.Books.AnyAsync(b => b.Title == book.Title &&
			b.Author == book.Author && b.ISBN == book.ISBN);

			if (!exists)
			{
				book.ID = Guid.NewGuid();
				toAdd.Add(book);
			}
		}

		if (toAdd.Any())
		{
			await _dbContext.Books.AddRangeAsync(toAdd);
			await _dbContext.SaveChangesAsync();
		}
	}
}
