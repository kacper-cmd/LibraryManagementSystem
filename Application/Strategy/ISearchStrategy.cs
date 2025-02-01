using Infrastructure.Entities;
namespace Application.Strategy;
public interface ISearchStrategy
{
    IQueryable<Book> Search(IQueryable<Book> books, string searchTerm);
}