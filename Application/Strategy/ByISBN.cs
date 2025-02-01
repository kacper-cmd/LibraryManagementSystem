using Infrastructure.Entities;
namespace Application.Strategy;
public class ByISBN : ISearchStrategy
{
	/// <summary>
	/// Searching in ISBN column base on given term 
	/// </summary>
	/// <param name="books"></param>
	/// <param name="searchTerm">term to search in ISBN column</param>
	/// <returns>IQueryable of filtered records base on term in ISBN column</returns>
	/// <example>
	/// <code>
	/// ISearchStrategyFactory searchStrategyFactory = new SearchStrategyFactory();
	/// IQueryable<Book> query = repository.GetBooksAsQueryable();
	/// query = searchStrategyFactory.GetSearchStrategy(nameof(BookDTO.ISBN)).Search(query, filter.SearchTerm);
	/// </code>
	/// </example>
	public IQueryable<Book> Search(IQueryable<Book> books, string searchTerm)
    {
        return books.Where(b => b.ISBN.Contains(searchTerm));
    }
}
