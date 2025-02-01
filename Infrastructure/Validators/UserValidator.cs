using Infrastructure.Constants;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
namespace Infrastructure.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage(Messages.RequiredName)
            .MinimumLength(3)
            .MaximumLength(100).WithMessage(Messages.ValidName);

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage(Messages.RequiredEmail)
            .EmailAddress().WithMessage(Messages.InvalidEmail);

        RuleFor(user => user.Role)
            .NotEmpty().WithMessage(Messages.RequiredRole)
            .Must(role => role.Equals("Librarian", StringComparison.OrdinalIgnoreCase) || role.Equals("Customer", StringComparison.OrdinalIgnoreCase)
            || role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            )
            .WithMessage("Role must be  'Librarian' or 'Customer' or 'Admin'.");
    }
}

