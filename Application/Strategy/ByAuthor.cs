using Application.Factory.Implementations;
using Infrastructure.Entities;
namespace Application.Strategy;
public class ByAuthor : ISearchStrategy
{
	/// <summary>
	/// Searching in author column base on given term 
	/// </summary>
	/// <param name="books"></param>
	/// <param name="searchTerm">term to search in author column</param>
	/// <returns>IQueryable of filtered records base on term in Author column</returns>
	/// <example>
	/// <code>
	/// ISearchStrategyFactory searchStrategyFactory = new SearchStrategyFactory();
	/// IQueryable<Book> query = repository.GetBooksAsQueryable();
	/// query = searchStrategyFactory.GetSearchStrategy(nameof(BookDTO.Title)).Search(query, filter.SearchTerm);
	/// </code>
	/// </example>
	public IQueryable<Book> Search(IQueryable<Book> books, string searchTerm)
	{
		return books.Where(b => b.Author.Contains(searchTerm));
	}


}
