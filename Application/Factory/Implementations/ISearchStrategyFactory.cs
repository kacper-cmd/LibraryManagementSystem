using Application.Strategy;

namespace Application.Factory.Implementations
{
	public interface ISearchStrategyFactory
	{
		ISearchStrategy GetSearchStrategy(string searchColumn);
	}
}