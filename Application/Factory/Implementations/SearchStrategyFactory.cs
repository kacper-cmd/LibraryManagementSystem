using Application.DTOs;
using Application.Strategy;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Factory.Implementations
{
	public class SearchStrategyFactory : ISearchStrategyFactory
	{
		private readonly IServiceProvider serviceProvider;

		public SearchStrategyFactory(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider; 
		}
		/// <summary>
		/// Get stategy sorting class
		/// </summary>
		/// <param name="searchColumn">Column name to perform searching</param>
		/// <returns>Concrete strategy</returns>
		/// <exception cref="ArgumentException">Thrown when the given search column is not supported.</exception>
		/// <example>
		/// <code>
		///  IQueryable<Book> query = repository.GetBooksAsQueryable();
		///  ISearchStrategy searchStrategy = searchStrategyFactory.GetSearchStrategy(nameof(BookDTO.Title));
		///  query = searchStrategy.Search(query, filter.SearchTerm);
		/// </code>
		/// </example>
		public ISearchStrategy GetSearchStrategy(string searchColumn)
		{
			return searchColumn switch
			{
				nameof(BookDTO.Title) => serviceProvider.GetRequiredService<ByTitle>(),
				nameof(BookDTO.Author) => serviceProvider.GetRequiredService<ByAuthor>(),
				nameof(BookDTO.ISBN) => serviceProvider.GetRequiredService<ByISBN>(),
				_ => throw new ArgumentException($"Unsupported search column: {searchColumn}")
			};
		}
	}
	//lub
	//public static class SearchStrategyFactory
	//{
	//    public static ISearchStrategy GetSearchStrategy(string searchColumn)
	//    {
	//        return searchColumn.ToLower() switch
	//        {
	//            "title" => new ByTitle(),
	//            "author" => new ByAuthor(),
	//            "isbn" => new ByISBN(),
	//            _ => throw new ArgumentException($"Unsupported search column: {searchColumn}")
	//        };
	//    }
	//}
}

