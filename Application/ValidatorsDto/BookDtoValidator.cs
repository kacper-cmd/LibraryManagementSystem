using Application.DTOs;
using FluentValidation;
using Infrastructure.Constants;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ValidatorsDto
{
	/// <summary>
	/// Checks book validity. See <see cref="BookDTO"/> class for details.
	/// </summary>
	public class BookDtoValidator : AbstractValidator<BookDTO>
    {
        public BookDtoValidator()
        {
            RuleFor(book => book.Title)
             .NotEmpty().WithMessage(Messages.RequiredTitle)
             .MaximumLength(200).WithMessage(Messages.ValidTitle);

            RuleFor(book => book.Author)
                .NotEmpty().WithMessage(Messages.RequiredAuthor)
                .MaximumLength(100).WithMessage(Messages.ValidAuthor);

            RuleFor(book => book.PublishedDate)
                .LessThanOrEqualTo(DateTime.Now.Date).WithMessage(Messages.RequiredPublish);


            RuleFor(book => book.Available)
                .NotNull().WithMessage("Availability status is required.");

			RuleFor(book => book.ISBN)
             .NotEmpty().WithMessage(Messages.ValidISBN)
             .Length(13).WithMessage(Messages.ValidISBNLong)
             .Matches("^978")
             .Must(HaveValidPLCountry)
             .WithMessage(Messages.ValidISBNStart);

        }
		/// <summary>
		/// Retrieves a user by their ID.
		///https://imker.pl/blog/numer-isbn-dla-ksiazki-siedem-najczesciej-zadawanych-pytan-instrukcje/
		/// </summary>
		/// <param name="isbn">The ISBN  International Standard Book Number (Międzynarodowy Znormalizowany Numer Książki) to numer złożony z 13 cyfr. 
        /// Pozwala zidentyfikować wydawcę, tytuł oraz jego różne wydania.</param>
		/// <returns>True is isbn is valid for poland</returns>
		private bool HaveValidPLCountry(string isbn)
        {
            if(string.IsNullOrWhiteSpace(isbn) || isbn.Length <=3) return false;
            string countryCode = isbn.Substring(3, 2);
            return countryCode == "83";
        }
    }
}
