using Application.DTOs;
using FluentValidation;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using Application.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Constants;
using Microsoft.AspNetCore.JsonPatch;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using API.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using FluentValidation.Results;
using Application.Mapper;
using System;
using Azure;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BookController : ControllerBase
	{
		private readonly ILogger<BookController> _logger;
		private readonly IValidator<BookDTO> _bookValidator;
		private readonly IBookService _bookService;

		public BookController(ILogger<BookController> logger, IValidator<BookDTO> bookValidator, IBookService bookService)
		{
			_logger = logger;
			_bookValidator = bookValidator;
			_bookService = bookService;
		}

		[HttpGet]
		public async ValueTask<ActionResult<IEnumerable<BookDTO>>> GetBooks()
		{
			var result = await _bookService.GetBooks();
			return Ok(result);
		}

		[HttpGet("get-books-paged")]
		[Authorize]
		public async ValueTask<ActionResult<PagedListDTO<BookDTO>>> GetBooksPaged(BaseFilter query)
		{
			var books = await _bookService.GetPagedListAsync(query);
			return Ok(books);
		}

		[HttpGet("get-books-pagedquery")]
		[Authorize]
		public async ValueTask<ActionResult<PageList<BookDTO>>> GetBooksPagedQuery([FromQuery] BaseFilter query)
		{
			var books = await _bookService.GetPagedListAsync(query);

			return Ok(books);
		}

		[HttpGet("{id:guid}")]
		public async ValueTask<ActionResult<BookDTO>> Get(Guid id)
		{
			try
			{
				var result = await _bookService.GetBook(id);

				return Ok(result);
			}
			catch (NotFoundException e)
			{
				return NotFound(e);
			}
			catch (Exception ex)
			{
				_logger.LogError(message: ex.Message, ex);
				return StatusCode(StatusCodes.Status500InternalServerError,
					"Error retrieving data from the database");
			}
		}
	

		[HttpPost("update-diffrent-approach")]
		public async Task<IActionResult> UpdateDiffrentApproach(BookDTO model)
		{
			var validationResult = await _bookValidator.ValidateAsync(model);
			if (!validationResult.IsValid)
			{
				var validationException = new ValidationException(validationResult.Errors);
				return BadRequest(validationException.ToProblemDetails());
			}
			Result<Book, Error> result = await _bookService.UpdateDiffrentApproach(model);
			return result.ToOk(customer => customer.MapToDTO());

			//return result.Match<IActionResult>(
			//	success =>
			//	{
			//		var d = Result<BookDTO, Error>.Ok(success.MapToDTO());
			//		return Ok(d);
			//	},
			//	error =>
			//	{
			//		switch (error.Code)
			//		{
			//			case var code when code == BookErrors.BookNotFound.Code:
			//				return NotFound(result.Error.ToString());
			//			default:
			//				return BadRequest(result.Error.ToString());
			//		}
			//	}
			//);
		}

		[HttpGet("get-books-paged-diffrent-approach")]
		public async Task<IActionResult> GetPagedListDiffrentApproach(BaseFilter filter)
		{
			Result<PagedListDTO<BookDTO>, Error> result = await _bookService.GetPagedListAsync2(filter);
			return result.Match<IActionResult>(
				success =>
				{
					return Ok(success);
				},
				error =>
				{
					switch (error.Code)
					{
						case "search/error":
							return BadRequest(result.Error.ToString());
						case "sort/error":
							return BadRequest(result.Error.ToString());
						case "paging/error":
							return StatusCode(500, result.Error.ToString());
						default:
							return StatusCode(500, result.Error.ToString());
					}
				}
			);
		}


		[HttpPost]
		public async Task<ActionResult<BookDTO>> Create(BookDTO dto)
		{
			try
			{
				if (dto == null)
					return BadRequest();

				var validationResult = _bookValidator.Validate(dto);
				if (!validationResult.IsValid)
					return StatusCode(StatusCodes.Status400BadRequest, validationResult.Errors);

				var createdBook = await _bookService.AddBook(dto);
				return Ok(createdBook);
			}
			catch (Exception ex)
			{
				_logger.LogError(message: ex.Message, ex);
				return StatusCode(StatusCodes.Status500InternalServerError,
					"Error creating new book record");
			}
		}




		[HttpPut("{id:guid}")]
		[Authorize(Roles = CustomRoles.AdminOrLibrarian)]
		public async Task<ActionResult<BookDTO>> Edit(Guid id, BookDTO dto)
		{
			try
			{
				var bookToUpdate = await _bookService.GetBook(id);

				var validationResult = _bookValidator.Validate(dto);
				if (!validationResult.IsValid)
				{
					var errors = validationResult.Errors;
					return BadRequest(errors);
				}

				if (bookToUpdate == null)
					return NotFound($"Book with Id = {id} not found");

				var user = await _bookService.UpdateBook(dto);

				return Ok(user);
			}
			catch (Exception ex)
			{
				_logger.LogError(message: ex.Message, ex);
				return StatusCode(StatusCodes.Status500InternalServerError,
						  "Error updating data");
			}
		}

		[HttpDelete("{id:guid}")]
		[Authorize(Roles = CustomRoles.AdminOrLibrarian)]
		public async ValueTask<ActionResult<string>> Delete(Guid id)
		{
			try
			{
				var bookToDelete = await _bookService.GetBook(id);

				if (bookToDelete == null)
				{
					return NotFound($"Book with Id = {id} not found");
				}
				await _bookService.DeleteBook(id);
				return Ok(id);
			}
			catch (Exception ex)
			{
				_logger.LogError(message: ex.Message, ex);
				return StatusCode(StatusCodes.Status500InternalServerError,
					"Error deleting data");
			}
		}




		#region TylkoDoTestowUzycia
		[HttpDelete("delete-book/{id:guid}")]
		[Authorize(Roles = CustomRoles.AdminOrLibrarian)]
		public async ValueTask<ActionResult<ApiResponse<BookDTO>>> DeleteBookAsyncApiResponse(Guid id)
		{
			var response = new ApiResponse<BookDTO>();
			try
			{
				var bookToDelete = await _bookService.GetBook(id);

				if (bookToDelete == null)
				{
					response.Success = false;
					response.ValidationErrors = new List<string>() { $"Book with Id = {id} not found " };
					response.Data = null;
					return NotFound(response);
				}

				await _bookService.DeleteBook(id);
				response.Data = bookToDelete;
				return Ok(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(message: ex.Message, ex);
				response.Success = false;
				response.ErrorMessage = $"Error {ex.Message}";
				return BadRequest(response);
			}
		}

		#endregion

		[HttpPatch("{id:guid}")]
		[Authorize(Roles = CustomRoles.AdminOrLibrarian)]
		public async ValueTask<ActionResult<BookDTO>> PatchBookAsync(Guid id, JsonPatchDocument<BookDTO> patchDocument)
		{
			var bookDTO = await _bookService.GetBook(id);

			if (bookDTO == null) return BadRequest();
			patchDocument.ApplyTo(bookDTO);
			var validationResult = _bookValidator.Validate(bookDTO);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors;
				return BadRequest(errors);
			}

			var book = await _bookService.UpdateBook(bookDTO);

			return Ok(book);
		}
		[HttpPost("import-books")]
		public async ValueTask<IActionResult> ImportBooks([FromBody] List<BookDTO> books)
		{
			if (books == null || books.Count == 0)
			{
				return BadRequest("No books to import.");
			}
			try
			{
				foreach (var book in books)
				{
					var validationResult = _bookValidator.Validate(book);
					if (!validationResult.IsValid)
					{
						var errors = validationResult.Errors;
						return BadRequest(errors);
					}
				}
				await _bookService.ImportBooks(books);

				return Ok();
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(message: ex.Message, ex);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}


	}

}

