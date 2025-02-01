using API.Controllers;
using Application.DTOs;
using Application.Interfaces.Services;
using Application.RequestModel;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace UnitTest.ControllersUnitTest
{
	/// <summary>
	///   https://marklowg.medium.com/how-to-unit-test-asp-net-mvc-web-api-controllers-be201b7c410a
	/// //https://medium.com/@madu.sharadika/validation-in-net-8-a250c4d278d2
	/// </summary>
	[TestFixture]
	public class BookControllerTests
	{
		private Mock<ILogger<BookController>> _loggerMock;
		private Mock<IValidator<BookDTO>> _bookValidatorMock;
		private Mock<IBookService> _bookServiceMock;
		private BookController _controller;

		[SetUp]
		public void SetUp()
		{
			_loggerMock = new Mock<ILogger<BookController>>();
			_bookValidatorMock = new Mock<IValidator<BookDTO>>();
			_bookServiceMock = new Mock<IBookService>();
			_controller = new BookController(_loggerMock.Object, _bookValidatorMock.Object, _bookServiceMock.Object);
		}
		#region Get

		[Test]
		public async Task Get_ReturnsOkResult_WithBookDTO()
		{
			// arrange
			var book = new BookDTO { ID = Guid.NewGuid(), Title = "Test Book", Author = "Test Author" };
			_bookServiceMock.Setup(service => service.GetBook(It.IsAny<Guid>()))
											  .ReturnsAsync(book);

			// act
			var result = await _controller.Get(book.ID);

			// assert
			var okResult = result.Result as OkObjectResult;
			Assert.IsNotNull(okResult);
			Assert.That(okResult.Value, Is.EqualTo(book));


			_bookServiceMock
			.Verify(x => x.GetBook(It.IsAny<Guid>()), Times.Once());//sprawdzam czy na pewno GetBook z serwisu zostala wywolana raz  ( z wywolania z  controlera web api )

			// Or we can be more specific and ensure that GetBook was called with the correct parameter. //https://stackoverflow.com/questions/4206193/how-do-i-verify-a-method-was-called-exactly-once-with-moq
			_bookServiceMock.Verify(x => x.GetBook(book.ID), Times.Once);
		}

		[Test]
		public async Task GetBooks_Should_Return_Books()
		{
			List<BookDTO> _books;
			// arrange
			_books = new List<BookDTO>();
			_books.Add(new BookDTO { Title = "Author1", Author = "Author", Available = true, ISBN = "9782123456803", PublishedDate = DateTime.Now });
			_books.Add(new BookDTO { Title = "Book", Author = "Author", Available = true, ISBN = "9782123456803", PublishedDate = DateTime.Now });
			_books.Add(new BookDTO { Title = "Bookfd", Author = "Authfor", Available = true, ISBN = "9782123446803", PublishedDate = DateTime.Now });

			_bookServiceMock.Setup(service => service.GetBooks())
											 .ReturnsAsync(_books);
			// act

			var result = await _controller.GetBooks();
			// assert

			Assert.IsInstanceOf<OkObjectResult>(result.Result);
			var okResult = result.Result as OkObjectResult;

			//Assert.AreEqual(_books, okResult.Value);
			Assert.That(okResult?.Value, Is.EqualTo(_books));

			Assert.IsNotNull(_books);

			Assert.IsInstanceOf<IEnumerable<BookDTO>>(okResult.Value);

			var returnedBooks = okResult.Value as IEnumerable<BookDTO>;

			Assert.That(returnedBooks?.Count(), Is.EqualTo(3));

			_bookServiceMock
				.Verify(x => x.GetBooks(), Times.Once());//sprawdzam czy metoda sie wykonała
		}

		[Test]
		public void GetBooks_ThrowsNotFoundException_WhenNoBooksFound()
		{
			// Arrange
			IEnumerable<BookDTO> bookDTOs = null;

			_bookServiceMock.Setup(service => service.GetBooks())
					.Throws(new NotFoundException("The requested resource was not found."));

			// Act & Assert
			Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetBooks());
		}


		[Test]
		public async Task Get_ReturnsNotFound_WhenUserDoesNotExist()
		{
			// Arrange
			var bookNull = null as BookDTO;

			_bookServiceMock.Setup(service => service.GetBook(It.IsAny<Guid>()))
				.ReturnsAsync(bookNull);

			// Act
			var result = await _controller.Get(Guid.NewGuid());

			// Assert
			Assert.IsInstanceOf<NotFoundResult>(result.Result);

		}






		[Test]
		public async Task GetPagedListDiffrentApproach_ReturnsOkResult_WithPagedList()
		{
			var filter = new BaseFilter
			{
				SearchColumn = nameof(BookDTO.Title),
				SearchTerm = "C# Programming",
				Page = 1,
				PageSize = 10
			};
			var pagedList = new PagedListDTO<BookDTO>
			{
				Items = new List<BookDTO> { new BookDTO { Title = "Author1", Author = "Author", Available = true, ISBN = "9782123456803", PublishedDate = DateTime.Now } },
				Page = 1,
				PageSize = 10,
				TotalCount = 1
			};

			var result = Result<PagedListDTO<BookDTO>, Error>.Ok(pagedList);

			 _bookServiceMock.Setup(service => service.GetPagedListAsync2(filter))
										.ReturnsAsync(result);
			// Act
			var actionResult = await _controller.GetPagedListDiffrentApproach(filter);

			// Assert

			Assert.IsInstanceOf<OkObjectResult>(actionResult);
			var okResult = actionResult as OkObjectResult;

			Assert.IsNotNull(okResult);

			Assert.That(okResult.StatusCode, Is.EqualTo(200));
			Assert.That(okResult.Value, Is.EqualTo(pagedList));

		}

		[Test]
		public async Task GetPagedListDiffrentApproach_ReturnsBadRequest_WhenSearchError()
		{
			// Arrange
			var filter = new BaseFilter
			{
				SearchColumn = nameof(BookDTO.Title),
				SearchTerm = "C# Programming",
				Page = 1,
				PageSize = 10
			};
			var error = new Error("search/error", "Invalid search parameter");
			var result = Result<PagedListDTO<BookDTO>, Error>.Err(error);

			_bookServiceMock.Setup(service => service.GetPagedListAsync2(filter))
							.ReturnsAsync(result);
			// Act
			var actionResult = await _controller.GetPagedListDiffrentApproach(filter);
			// Assert
			Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
			var badRequestResult = actionResult as BadRequestObjectResult;
			Assert.IsNotNull(badRequestResult);
			Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
			Assert.That(badRequestResult.Value, Is.EqualTo(error.ToString()));
		}

		[Test]
		public async Task GetPagedListDiffrentApproach_ReturnsInternalServerError_WhenPagingError()
		{
			// Arrange
			var filter = new BaseFilter
			{
				SearchColumn = nameof(BookDTO.Title),
				SearchTerm = "C# Programming",
				Page = 1,
				PageSize = 10
			};

			var error = new Error("paging/error", "Paging failed");
			var result = Result<PagedListDTO<BookDTO>, Error>.Err(error);

			_bookServiceMock.Setup(service => service.GetPagedListAsync2(filter))
							.ReturnsAsync(result);
			// Act
			var actionResult = await _controller.GetPagedListDiffrentApproach(filter);
			// Assert

			Assert.IsInstanceOf<ObjectResult>(actionResult);
			var internalServerErrorResult = actionResult as ObjectResult;

			Assert.IsNotNull(internalServerErrorResult);
			Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(500));

			Assert.That(internalServerErrorResult.Value, Is.EqualTo(error.ToString()));
		}

		[Test]
		public async Task UpdateDiffrentApproach_ReturnsOkResult_WhenSuccessful()
		{
			// Arrange
			var model = new BookDTO { Title = "Kacper", Author = "Author", Available = true, ISBN = "9782123456803", PublishedDate = DateTime.Now };
			var book = new Book { Title = "Kacper", Author = "Author", Available = true, ISBN = "9782123456803", PublishedDate = DateTime.Now };
			
			var validationResult = new ValidationResult();
			_bookValidatorMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(validationResult);
			_bookServiceMock.Setup(service => service.UpdateDiffrentApproach(model))
							.ReturnsAsync(Result<Book, Error>.Ok(book));
			// Act
			var actionResult = await _controller.UpdateDiffrentApproach(model);
			// Assert
			Assert.IsInstanceOf<OkObjectResult>(actionResult);
			var okResult = actionResult as OkObjectResult;
			Assert.IsNotNull(okResult);
			Assert.That(okResult.StatusCode, Is.EqualTo(200));
			var result = (Result<BookDTO, Error>) okResult.Value  ;
			Assert.IsNotNull(result);
			Assert.That(result.Value.Title, Is.EqualTo("Kacper"));
		}
		[Test]
		public async Task UpdateDiffrentApproach_ReturnsNotFound_WhenBookNotFound()
		{
			// Arrange
			var model = new BookDTO { Title = "Kacper", Author = "Author", Available = true, ISBN = "9782123456803", PublishedDate = DateTime.Now };

			var validationResult = new ValidationResult();
			var error = BookErrors.BookNotFound;

			_bookValidatorMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(validationResult);

			_bookServiceMock.Setup(service => service.UpdateDiffrentApproach(model))
							.ReturnsAsync(Result<Book, Error>.Err(error));
			// Act
			var actionResult = await _controller.UpdateDiffrentApproach(model);
			// Assert
			Assert.IsInstanceOf<NotFoundObjectResult>(actionResult);
			var notFoundResult = actionResult as NotFoundObjectResult;

			Assert.IsNotNull(notFoundResult);
			Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
			Assert.That(notFoundResult.Value, Is.EqualTo(error.ToString()));
		}


		#endregion



		#region Create
		[Test]
		public async Task Create_ReturnsOkResult()
		{
			// Arrange
			var newBook = new BookDTO { Title = "New Book", Author = "Kacper" };
			var createdBook = new BookDTO { ID = Guid.NewGuid(), Title = "New Book", Author = "Kacper" };

			_bookValidatorMock.Setup(validator => validator.Validate(newBook))
						  .Returns(new ValidationResult());

			_bookServiceMock.Setup(service => service.AddBook(newBook))
							.ReturnsAsync(createdBook);

			// Act
			var result = await _controller.Create(newBook);

			// Assert
			Assert.IsInstanceOf<OkObjectResult>(result.Result);
			var okResult = result.Result as OkObjectResult;

			Assert.IsInstanceOf<BookDTO>(okResult.Value);
			var returnedBook = okResult.Value as BookDTO;

			Assert.That(returnedBook?.ID, Is.EqualTo(createdBook.ID));
			Assert.That(returnedBook.Title, Is.EqualTo(createdBook.Title));
		}
		[Test]
		public async Task Create_ReturnsBadRequest_WhenValidationFails()
		{
			// Arrange
			var newBook = new BookDTO
			{
				Title = "",
				Author = "Kacper",
				Available = true,
				ISBN = "9788312345678",
				PublishedDate = DateTime.Now.Date,
			};


			var validationFailures = new List<ValidationFailure>
			{
				new ValidationFailure("Title", Messages.RequiredTitle)
			};

			_bookValidatorMock.Setup(validator => validator.Validate(newBook))
						  .Returns(new ValidationResult(validationFailures));

			// Act
			var result = await _controller.Create(newBook);

			// Assert
			Assert.IsInstanceOf<ObjectResult>(result.Result);
			var badRequestResult = result.Result as ObjectResult;

			Assert.That(badRequestResult?.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
			Assert.IsInstanceOf<List<ValidationFailure>>(badRequestResult.Value);

			var errors = badRequestResult.Value as List<ValidationFailure>;

			Assert.That(errors?.Count, Is.EqualTo(1));
			Assert.That(errors[0].PropertyName, Is.EqualTo("Title"));
			Assert.That(errors[0].ErrorMessage, Is.EqualTo(Messages.RequiredTitle));
		}

		[Test]
		public async Task Create_ReturnsBadRequest_WhenDtoIsNull()
		{
			// Arrange
			BookDTO newBook = null;

			// Act
			var result = await _controller.Create(newBook);

			// Assert
			Assert.IsInstanceOf<BadRequestResult>(result.Result);
		}

		#endregion

		#region Edit
		[Test]
		public async Task Edit_ReturnsOkResult_WithUpdatedBook()
		{
			// Arrange
			var bookId = Guid.NewGuid();
			var existingBook = new BookDTO
			{
				ID = bookId,
				Title = "Old Title",
				Author = "Old KACPER",
				Available = true,
				ISBN = "9788312345678",
				PublishedDate = DateTime.Now.Date,
			};
			var updatedBook = new BookDTO
			{
				ID = bookId,
				Title = "New Title",
				Author = "New KACPER",
				Available = true,
				ISBN = "9788312345678",
				PublishedDate = DateTime.Now.Date,
			};

			_bookServiceMock.Setup(service => service.GetBook(bookId))
							.ReturnsAsync(existingBook);

			_bookValidatorMock.Setup(validator => validator.Validate(updatedBook))
			  .Returns(new ValidationResult());

			_bookServiceMock.Setup(service => service.UpdateBook(updatedBook))
							.ReturnsAsync(updatedBook);

			// Act
			var result = await _controller.Edit(bookId, updatedBook);

			// Assert
			Assert.IsInstanceOf<OkObjectResult>(result.Result);
			var okResult = result.Result as OkObjectResult;

			Assert.IsInstanceOf<BookDTO>(okResult.Value);
			var returnedBook = okResult.Value as BookDTO;

			Assert.That(returnedBook?.Title, Is.EqualTo(updatedBook.Title));
			Assert.That(returnedBook?.Author, Is.EqualTo(updatedBook.Author));
		}
		[Test]
		public async Task Edit_ReturnsNotFound_WhenBookDoesNotExist()
		{
			// Arrange
			var bookId = Guid.NewGuid();
			BookDTO nullBook = null;
			var updatedBook = new BookDTO
			{
				ID = bookId,
				Title = "Old Title",
				Author = "Old KACPER",
				Available = true,
				ISBN = "9788312345678",
				PublishedDate = DateTime.Now.Date,
			};

			_bookServiceMock.Setup(service => service.GetBook(bookId))
							.ReturnsAsync(nullBook);

			_bookValidatorMock.Setup(validator => validator.Validate(updatedBook))
			  .Returns(new ValidationResult());
			// Act
			var result = await _controller.Edit(bookId, updatedBook);

			// Assert
			Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
			var notFoundResult = result.Result as NotFoundObjectResult;

			Assert.That(notFoundResult?.Value, Is.EqualTo($"Book with Id = {bookId} not found"));
		}


		#endregion

		#region Delete
		[Test]
		public async Task Delete_ReturnsOkResult_WhenBookIsDeleted()
		{
			// Arrange
			var bookId = Guid.NewGuid();
			var existingBook = new BookDTO { ID = bookId, Title = "Title", Author = "Kacper" };

			_bookServiceMock.Setup(service => service.GetBook(bookId))
							.ReturnsAsync(existingBook);

			_bookServiceMock.Setup(service => service.DeleteBook(bookId))
							.Returns(ValueTask.CompletedTask);

			// Act
			var result = await _controller.Delete(bookId);

			// Assert
			Assert.IsInstanceOf<OkObjectResult>(result.Result);
			var okResult = result.Result as OkObjectResult;

			Assert.That(okResult?.Value, Is.EqualTo(existingBook.ID));
		}

		[Test]
		public async Task Delete_ReturnsNotFound_WhenBookDoesNotExist()
		{
			// Arrange
			var bookId = Guid.NewGuid();
			BookDTO bookDTONull = null;

			_bookServiceMock.Setup(service => service.GetBook(bookId))
							.ReturnsAsync(bookDTONull);

			// Act
			var result = await _controller.Delete(bookId);

			// Assert
			Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
			var notFoundResult = result.Result as NotFoundObjectResult;

			Assert.That(notFoundResult?.Value, Is.EqualTo($"Book with Id = {bookId} not found"));

		}
		#endregion



		//https://stackoverflow.com/questions/50037798/tdd-mock-validate-validateasync-method

		#region Patch
		[Test]
		public async Task PatchBookAsync_ReturnsOkResult_WithUpdatedBook()
		{
			// Arrange
			var bookId = Guid.NewGuid();
			var existingBook = new BookDTO
			{
				ID = bookId,
				Title = "Original Title",
				Author = "Original Kacper",
				Available = true,
				ISBN = "9788312345678",
				PublishedDate = DateTime.Now.Date,
			};
			var patchedBook = new BookDTO
			{
				ID = bookId,
				Title = "Updated Title",
				Author = "Original Kacper",
				Available = true,
				ISBN = "9788312345678",
				PublishedDate = DateTime.Now.Date,
			};


			_bookServiceMock.Setup(service => service.GetBook(bookId))
							.ReturnsAsync(existingBook);

			_bookValidatorMock.Setup(validator => validator.Validate(It.IsAny<BookDTO>()))
						  .Returns(new ValidationResult());


			_bookServiceMock.Setup(service => service.UpdateBook(It.IsAny<BookDTO>()))
							.ReturnsAsync(patchedBook);

			var patchDoc = new JsonPatchDocument<BookDTO>();
			patchDoc.Replace(b => b.Title, "Updated Title");

			// Act
			var result = await _controller.PatchBookAsync(bookId, patchDoc);

			// Assert
			Assert.IsInstanceOf<OkObjectResult>(result.Result);

			var okResult = result.Result as OkObjectResult;
			Assert.IsInstanceOf<BookDTO>(okResult.Value);

			var returnedBook = okResult.Value as BookDTO;

			Assert.That(returnedBook?.Title, Is.EqualTo("Updated Title"));
			Assert.That(returnedBook.Author, Is.EqualTo("Original Kacper"));
		}

		[Test]
		public async Task PatchBookAsync_ReturnsBadRequest_WhenBookDoesNotExist()
		{
			// Arrange
			var bookId = Guid.NewGuid();
			var book = null as BookDTO;
			_bookServiceMock.Setup(service => service.GetBook(bookId))
							.ReturnsAsync(book);

			var patchDoc = new JsonPatchDocument<BookDTO>();
			patchDoc.Replace(b => b.Title, "Updated Title");

			// Act
			var result = await _controller.PatchBookAsync(bookId, patchDoc);

			// Assert
			Assert.IsInstanceOf<BadRequestResult>(result.Result);
		}
		[Test]
		public async Task PatchBookAsync_ReturnsBadRequest_WhenValidationFailsAfterPatch()
		{
			// Arrange
			var bookId = Guid.NewGuid();
			var existingBook = new BookDTO
			{
				ID = bookId,
				Title = "Original Title",
				Author = "Original Kacper",
				Available = true,
				ISBN = "9788312345678",
				PublishedDate = DateTime.Now.Date,
			};

			var validationFailures = new List<ValidationFailure>
			{
				new ValidationFailure("Title", Messages.RequiredTitle)
			};

			_bookServiceMock.Setup(service => service.GetBook(bookId))
							.ReturnsAsync(existingBook);

			_bookValidatorMock.Setup(validator => validator.Validate(It.IsAny<BookDTO>()))
						  .Returns(new ValidationResult(validationFailures));

			var patchDoc = new JsonPatchDocument<BookDTO>();
			patchDoc.Replace(b => b.Title, "");

			// Act
			var result = await _controller.PatchBookAsync(bookId, patchDoc);

			// Assert
			Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;

			Assert.IsInstanceOf<List<ValidationFailure>>(badRequestResult.Value);
			var errors = badRequestResult.Value as List<ValidationFailure>;

			Assert.That(errors?.Count, Is.EqualTo(1));
			Assert.That(errors[0].PropertyName, Is.EqualTo("Title"));
			Assert.That(errors[0].ErrorMessage, Is.EqualTo(Messages.RequiredTitle));

		}
		#endregion
	}
}
