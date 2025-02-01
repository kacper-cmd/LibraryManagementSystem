using Application.DTOs;
using Application.Factory.Implementations;
using Application.Interfaces.Repositories;
using Application.RequestModel;
using Application.Services;
using Application.Strategy;
using Infrastructure.Entities;
using Moq;

namespace UnitTest.ServicesUnitTest
{
	[TestFixture]
	public class BookServiceTests
	{
		private Mock<IBookRepository> _repositoryMock;
		private Mock<ISearchStrategyFactory> _searchStrategyFactoryMock;
		private BookService _bookService;
		private Mock<ISearchStrategy> searchStrategyMock;

		[SetUp]
		public void Setup()
		{
			_repositoryMock = new Mock<IBookRepository>();
			_searchStrategyFactoryMock = new Mock<ISearchStrategyFactory>();
			_bookService = new BookService(_repositoryMock.Object, _searchStrategyFactoryMock.Object);
			searchStrategyMock = new Mock<ISearchStrategy>();
		}

		[Test]
		public async Task GetPagedListAsync_Uses_SearchStrategy_When_SearchColumn_And_SearchTerm_Are_Provided()
		{
			// Arrange
			var filter = new BaseFilter
			{
				SearchColumn = nameof(BookDTO.Title),
				SearchTerm = "C# Programming",
				Page = 1,
				PageSize = 10
			};
			
			var books = new List<Book>
			{
				   new Book { Title = "Java Programming", Author = "Kacper O", Available = true, ISBN = "9782123456803", PublishedDate = DateTime.Now },
				   new Book { Title = "C# Programming", Author = "Kacper O", Available = true, ISBN = "9782123456803", PublishedDate = DateTime.Now },
				   new Book { Title = "Ptython Programming", Author = "Kacper O", Available = true, ISBN = "9782123456803", PublishedDate = DateTime.Now },
			}.AsQueryable();


			searchStrategyMock.Setup(s => s.Search(It.IsAny<IQueryable<Book>>(), filter.SearchTerm))
				.Returns(books.Where(b => b.Title.Contains(filter.SearchTerm)));

			// _repositoryMock.Setup(repo => repo.GetBooksAsQueryable("")).Returns(books);

			_searchStrategyFactoryMock.Setup(factory => factory.GetSearchStrategy(filter.SearchColumn))
				.Returns(searchStrategyMock.Object);
			// Act
			var result = await _bookService.GetPagedListAsync(filter);
			// Assert

			Assert.IsNotNull(result);
			Assert.That(result.TotalCount, Is.EqualTo(1));
			Assert.That(result.Page, Is.EqualTo(1));
			Assert.That(result.PageSize, Is.EqualTo(10));

			_searchStrategyFactoryMock.Verify(factory => factory.GetSearchStrategy(filter.SearchColumn), Times.Once);

			searchStrategyMock.Verify(s => s.Search(It.IsAny<IQueryable<Book>>(), filter.SearchTerm), Times.Once);
		}

		[Test]
		public async Task GetPagedListAsync_Throws_ArgumentException_For_Invalid_SearchColumn()
		{
			// Arrange
			var filter = new BaseFilter
			{
				SearchColumn = "Wrong Column xd",
				SearchTerm = "HA HA HA",
				Page = 1,
				PageSize = 10,
				SortColumn = "",
				SortOrder = ""
			};
			 
			    _searchStrategyFactoryMock.Setup(factory => factory.GetSearchStrategy(filter.SearchColumn))
				.Throws(new ArgumentException($"Unsupported search column: {filter.SearchColumn}"));

			// Act & Assert

			var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _bookService.GetPagedListAsync(filter));
			Assert.That(exception.Message, Is.EqualTo($"Unsupported search column: {filter.SearchColumn}"));
			_searchStrategyFactoryMock.Verify(factory => factory.GetSearchStrategy(filter.SearchColumn), Times.Once);
		}

		[Test]
		public async Task GetPagedListAsync_Use_Default_SearchStrategy_Use_ByTitle_When_SearchColumn_Is_Not_Null()
		{
			// Arrange
			var filter = new BaseFilter
			{
				SearchColumn = null,
				SearchTerm = "C# Programming",
				Page = 1,
				PageSize = 10,
				SortColumn = "",
				SortOrder = ""
			};
			var books = new List<Book>
			{
				   new Book { Title = "Java Programming", Author = "Kacper O", Available = true, ISBN = "9782123456803", PublishedDate = DateTime.Now },
				   new Book { Title = "C# Programming", Author = "Kacper O", Available = true, ISBN = "9782123456803", PublishedDate = DateTime.Now },
				   new Book { Title = "Ptython Programming", Author = "Kacper O", Available = true, ISBN = "9782123456803", PublishedDate = DateTime.Now },
			}.AsQueryable();

			searchStrategyMock.Setup(s => s.Search(It.IsAny<IQueryable<Book>>(), filter.SearchTerm))
				.Returns(books.Where(b => b.Title.Contains(filter.SearchTerm)));


			_searchStrategyFactoryMock.Setup(factory => factory.GetSearchStrategy(nameof(BookDTO.Title)))
				.Returns(searchStrategyMock.Object);

			// Act

			var result = await _bookService.GetPagedListAsync(filter);

			// Assert

			Assert.IsNotNull(result);
			Assert.That(result.TotalCount, Is.EqualTo(1));
			Assert.That(result.Page, Is.EqualTo(1));
			Assert.That(result.PageSize, Is.EqualTo(10));

			_searchStrategyFactoryMock.Verify(factory => factory.GetSearchStrategy(nameof(BookDTO.Title)), Times.Once);

			searchStrategyMock.Verify(s => s.Search(It.IsAny<IQueryable<Book>>(), filter.SearchTerm), Times.Once);
		}
	}
}
