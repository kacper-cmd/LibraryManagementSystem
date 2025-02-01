using Application.DTOs;
using Application.Factory.Implementations;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Mapper;
using Application.RequestModel;
using Application.Strategy;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Infrastructure.Exceptions;
using Infrastructure.Validators;
using System;
using System.Linq.Expressions;

namespace Application.Services
{
	public class BookService : IBookService
	{
		private readonly IBookRepository repository;
		private readonly ISearchStrategyFactory searchStrategyFactory;

		public BookService(IBookRepository repository, ISearchStrategyFactory searchStrategyFactory)
		{
			this.repository = repository;
			this.searchStrategyFactory = searchStrategyFactory;
		}
		/// <summary>
		/// Retrieves a paged list of BooksDto to use it in table with  pagination 
		/// Uses also search stategy to sort base on search column
		/// </summary>
		/// <param name="filter">The filter which provide basic filtering features such a pagination, searching and sorting.</param>
		/// <returns>Paged list of books dto</returns>
		/// <example>
		/// <code>
		/// await _bookService.GetPagedListAsync(baseFilter);
		/// </code>
		/// </example>
		/// <remarks>This method can be enhanced to handle more advance filtering by inheriting from BaseFilter like AdvanceFilter : BaseFilter.</remarks>
		public async ValueTask<PagedListDTO<BookDTO>> GetPagedListAsync(BaseFilter filter)
		{
			IQueryable<Book> query = repository.GetBooksAsQueryable();

			#region SearchingUsingStrategyPattern
			if (!string.IsNullOrEmpty(filter.SearchColumn) && !string.IsNullOrEmpty(filter.SearchTerm))
			{
				ISearchStrategy searchStrategy = searchStrategyFactory.GetSearchStrategy(filter.SearchColumn);
				query = searchStrategy.Search(query, filter.SearchTerm);

			}
			else if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
			{
				query = searchStrategyFactory.GetSearchStrategy(nameof(BookDTO.Title)).Search(query, filter.SearchTerm);
			}
			#endregion
			if (filter.SortOrder?.ToLower() == Constants.desc)
			{
				query = query.OrderByDescending(GetSortProperty(filter.SortColumn!));
			}
			else
			{
				query = query.OrderBy(GetSortProperty(filter.SortColumn!));
			}

			var bookResponsesQuery = query.Select(x => new BookDTO(x.ID, x.Available, x.Author, x.Title, x.ISBN, x.PublishedDate));
			var books = PageList<BookDTO>.Create(bookResponsesQuery, filter.Page, filter.PageSize);
			return new PagedListDTO<BookDTO>()
			{
				Items = books.Items,
				Page = books.Page,
				PageSize = books.PageSize,
				TotalCount = books.TotalCount,
			};
		}
		public async ValueTask<Result<PagedListDTO<BookDTO>, Error>> GetPagedListAsync2(BaseFilter filter)
		{
			IQueryable<Book> query = repository.GetBooksAsQueryable();
			#region SearchingUsingStrategyPattern
			try
			{
				if (!string.IsNullOrEmpty(filter.SearchColumn) && !string.IsNullOrEmpty(filter.SearchTerm))
				{
					ISearchStrategy searchStrategy = searchStrategyFactory.GetSearchStrategy(filter.SearchColumn);
					query = searchStrategy.Search(query, filter.SearchTerm);
				}
				else if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
				{
					query = searchStrategyFactory.GetSearchStrategy(nameof(BookDTO.Title)).Search(query, filter.SearchTerm);
				}
			}
			catch (ArgumentException ex)
			{
				return Result<PagedListDTO<BookDTO>, Error>.Err(new Error("search/error", ex.Message));
			}
			catch (Exception ex)
			{
				return Result<PagedListDTO<BookDTO>, Error>.Err(new Error("search/error", ex.Message));
			}
			#endregion
			try
			{
				if (filter.SortOrder?.ToLower() == Constants.desc)
				{
					query = query.OrderByDescending(GetSortProperty(filter.SortColumn!));
				}
				else
				{
					query = query.OrderBy(GetSortProperty(filter.SortColumn!));
				}
			}
			catch (Exception ex)
			{
				return Result<PagedListDTO<BookDTO>, Error>.Err(new Error("sort/error", ex.Message));
			}
			try
			{
				var bookResponsesQuery = query.Select(x => new BookDTO(x.ID, x.Available, x.Author, x.Title, x.ISBN, x.PublishedDate));
				var books = PageList<BookDTO>.Create(bookResponsesQuery, filter.Page, filter.PageSize);
				var pagedResult = new PagedListDTO<BookDTO>()
				{
					Items = books.Items,
					Page = books.Page,
					PageSize = books.PageSize,
					TotalCount = books.TotalCount,
				};
				return Result<PagedListDTO<BookDTO>, Error>.Ok(pagedResult);
			}
			catch (Exception ex)
			{
				return Result<PagedListDTO<BookDTO>, Error>.Err(new Error("paging/error", ex.Message));
			}
		}



		/// <summary>
		/// Get expression to sort base on column
		/// </summary>
		/// <param name="sortColumn">Column base on which i want to sort</param>
		/// <returns>Expression to sort</returns>
		/// <example>
		/// <code>
		/// query.OrderByDescending(GetSortProperty(filter.SortColumn!));
		/// </code>
		/// </example>
		private static Expression<Func<Book, object>> GetSortProperty(string sortColumn)
		{
			return sortColumn switch
			{
				nameof(BookDTO.Title) => item => item.Title,
				nameof(BookDTO.ISBN) => item => item.ISBN,
				nameof(BookDTO.Author) => item => item.Author,
				nameof(BookDTO.PublishedDate) => item => item.PublishedDate,
				_ => item => item.ID
			};
		}


		/// <summary>
		/// Get IEnumerable of Books 
		/// </summary>
		/// <returns>The all books from db mapped to dto </returns>
		public async ValueTask<IEnumerable<BookDTO>> GetBooks()
		{
			var repo = await repository.GetBooks();

			if (repo is null)
				throw new NotFoundException("The requested resource was not found.");

			return repo!.MapToDTO();
		}



		/// <summary>
		/// Get Book base on parameter
		/// </summary>
		/// <param name="bookId">The books's unique identifier.</param>
		/// <returns>The book mapped to data transfer object</returns>
		public async ValueTask<BookDTO> GetBook(Guid bookId)
		{
			var repo = await repository.GetBook(bookId);

			if (repo is null)
				throw new NotFoundException("The requested resource was not found.");

			return repo.MapToDTO();
		}



		/// <summary>
		/// Takes a book dto to be added in db
		/// </summary>
		/// <param name="dto">The book dto to be added in database</param>
		/// <returns>The latest added dto book mapped to dto</returns>
		public async ValueTask<BookDTO> AddBook(BookDTO dto)
		{
			var entity = dto.MapToDomainModel();
			var repo = await repository.AddBook(entity);
			return repo.MapToDTO();
		}

		/// <summary>
		/// Takes a book dto to be update in db
		/// </summary>
		/// <param name="dto">The book dto to be updated in database</param>
		/// <returns>The latest updated book mapped to dto</returns>
		public async ValueTask<BookDTO> UpdateBook(BookDTO dto)
		{

			var entity = dto.MapToDomainModel();
			var repo = await repository.UpdateBook(entity);

			return repo!.MapToDTO();
		}



		/// <summary>
		/// Delete book base on its Giid
		/// </summary>
		/// <param name="bookId">The book's unique identifier.</param>
		public async ValueTask DeleteBook(Guid bookId)
		{
			await repository.DeleteBook(bookId);
		}
		/// <summary>
		/// Imports list of books and saves its into db
		/// </summary>
		/// <param name="books">The list of books to be imported into db </param>
		public async ValueTask ImportBooks(List<BookDTO> books)
		{
			var dto = books.MapToDomainModel();
			await repository.ImportBooks(dto);
		}

		public async ValueTask<Result<Book, Error>> UpdateDiffrentApproach(BookDTO model)
		{
			if(model is null)
			{
				return BookErrors.BookNotFound;
			}
			var updatedBook = await UpdateBook(model);
			var domainModel = updatedBook.MapToDomainModel();
			return domainModel;
		}

	}
}
