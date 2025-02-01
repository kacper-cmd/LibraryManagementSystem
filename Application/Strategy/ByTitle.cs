using Infrastructure.Entities;
namespace Application.Strategy;
//Contrete strategy
public class ByTitle : ISearchStrategy
{
    public IQueryable<Book> Search(IQueryable<Book> books, string searchTerm)
    {
        return books.Where(b => b.Title.Contains(searchTerm));
    }
}
