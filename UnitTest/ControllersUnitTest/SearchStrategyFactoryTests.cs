using Application.DTOs;
using Application.Factory.Implementations;
using Application.Strategy;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.ControllersUnitTest
{
	[TestFixture]
    public class SearchStrategyFactoryTests
    {
		#region Fields&Properties
		private Mock<IServiceProvider> _serviceProviderMock;
        private ISearchStrategyFactory _factory;
        private ISearchStrategy _strategyFactory;
		#endregion
		
        [SetUp]
		public void SetUp()
		{
            _serviceProviderMock = new Mock<IServiceProvider>(); 
			_factory = new SearchStrategyFactory(_serviceProviderMock.Object);
		}
		#region TestCode

		[Test]
        public void GetSearchStrategy_ByTitle()
        {
			// Arrange
			_strategyFactory = new ByTitle();
            _serviceProviderMock.Setup(x => x.GetService(typeof(ByTitle))).
                Returns(_strategyFactory);
           
            // Act
            var strategy = _factory.GetSearchStrategy(nameof(BookDTO.Title));

            // Assert
            Assert.IsInstanceOf<ByTitle>(strategy);
            Assert.IsNotInstanceOf<ByAuthor>(strategy);
            Assert.IsNotInstanceOf<ByISBN>(strategy);
        }

        [Test]
        public void GetSearchStrategy_ThrowsArgumentExceptionException_WhenNoSearchStrategyProvided()
        {
            // Arrange
            _strategyFactory = new ByTitle();
            _serviceProviderMock.Setup(x => x.GetService(typeof(ByTitle)))
                .Throws(new ArgumentException());
            //.Returns(_strategyFactory);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _factory.GetSearchStrategy(""));
        }
        
		#endregion
	}
}