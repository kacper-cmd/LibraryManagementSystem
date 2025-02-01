using Infrastructure.Entities;
namespace Application.Strategy
{
    //https://medium.com/codenx/strategy-pattern-net-c-ea0d122f60c4
    //context class uzycie 
    public class Search
    {
        private ISearchStrategy _strategy;
        public Search(ISearchStrategy strategy)
        {
            _strategy = strategy;
        }
        public void SetSearchStrategy(ISearchStrategy strategy)
        {
            _strategy = strategy;
        }
        public IQueryable<Book> ExecuteSearch(IQueryable<Book> books, string searchTerm)
        {
            return _strategy.Search(books, searchTerm);
        }
    }
    //or using lambga
    //public delegate IQueryable<Book> SearchStrategy(IQueryable<Book> books, string searchTerm);

    //public class Search
    //{
    //    public IQueryable<Book> ExecuteSearch(IQueryable<Book> books, string searchTerm, SearchStrategy strategy)
    //    {
    //        return strategy(books, searchTerm);
    //    }
    //}
}
