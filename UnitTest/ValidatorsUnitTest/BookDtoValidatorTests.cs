using API.Controllers;
using Application.DTOs;
using Application.ValidatorsDto;
using FluentValidation.TestHelper;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.ComponentModel.DataAnnotations;
namespace UnitTest.ValidatorsUnitTest
{
	/// <summary>
	/// BASE ON https://docs.fluentvalidation.net/en/latest/testing.html
	/// </summary>
	[TestFixture]
	public class BookDtoValidatorTests
	{
		#region Fields&Properties
		private BookDtoValidator _validator;
		#endregion
		[SetUp]
		public void SetUp()
		{
			_validator = new BookDtoValidator();
		}
		#region TestCode
		[Test]
		public void Should_Not_Have_Error_When_Model_Is_Valid()
		{
			var model = new BookDTO
			{
				Title = "Harry Potter",
				Author = "JK Rowling ",
				PublishedDate = DateTime.Now.Date,
				Available = true,
				ISBN = "9788312345678"
			};

			var result = _validator.TestValidate(model);

			result.ShouldNotHaveAnyValidationErrors();
		}

		[Test]
		public void Should_Have_Error_When_ISBN_Is_Not_13_Characters()
		{
			var model = new BookDTO
			{
				ISBN = "978832345689",
				Title = "Test",
				Author = "Test",
				PublishedDate = DateTime.Now.Date,
			};

			var result = _validator.TestValidate(model);

			result.ShouldHaveValidationErrorFor(book => book.ISBN)
				  .WithErrorMessage(Messages.ValidISBNLong);

		}

		[Test]
		public void Should_Have_Error_When_ISBN_Does_Not_Start_With_978_For_Valid_PL()
		{
			var model = new BookDTO
			{
				Title = "Harry Potter",
				Author = "JK Rowling ",
				PublishedDate = DateTime.Now.Date,
				Available = true,
				ISBN = "9781234567890"
			};
			var result = _validator.TestValidate(model);

			result.ShouldHaveValidationErrorFor(book => book.ISBN)
				  .WithErrorMessage(Messages.ValidISBNStart);

		}
		#endregion
	}
}
