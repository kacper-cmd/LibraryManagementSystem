using Infrastructure.Constants;
using Infrastructure.Entities;
using FluentValidation;

namespace Infrastructure.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(book => book.Title)
           .NotEmpty().WithMessage(Messages.RequiredTitle)
           .MaximumLength(200).WithMessage(Messages.ValidTitle);

            RuleFor(book => book.Author)
                .NotEmpty().WithMessage(Messages.RequiredAuthor)
                .MaximumLength(100).WithMessage(Messages.ValidAuthor);

            RuleFor(book => book.PublishedDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage(Messages.RequiredPublish);


            RuleFor(book => book.Available)
                .NotNull().WithMessage("Availability status is required.");

            RuleFor(book => book.ISBN)
             .NotEmpty().WithMessage(Messages.ValidISBN)
             .Length(13)
             .Matches("^978")
             .Must(HaveValidPLCountry)
             .WithMessage("Invalid country or language code.")
             .WithMessage("ISBN format is invalid.");// https://imker.pl/blog/numer-isbn-dla-ksiazki-siedem-najczesciej-zadawanych-pytan-instrukcje/

        }
        private bool HaveValidPLCountry(string isbn)
        {
            string countryCode = isbn.Substring(3, 2);
            return countryCode == "83";
        }
    }
}
