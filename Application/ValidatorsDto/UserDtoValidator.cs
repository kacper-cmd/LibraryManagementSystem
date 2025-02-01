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
	/// Checks user validity. See <see cref="UserDTO"/> class for details.
	/// </summary>
	public class UserDtoValidator : AbstractValidator<UserDTO>
    {
        public UserDtoValidator()
        {
            RuleFor(user => user.Name)
                .NotEmpty().WithMessage(Messages.RequiredName)
                .MinimumLength(3).WithMessage(Messages.MinUserName)
                .MaximumLength(100).WithMessage(Messages.ValidName);

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage(Messages.RequiredEmail)
                .EmailAddress().WithMessage(Messages.InvalidEmail);

            RuleFor(user => user.Role)
                .NotEmpty().WithMessage(Messages.RequiredRole)
                .Must(role => role.Equals("Librarian", StringComparison.OrdinalIgnoreCase) || role.Equals("Customer", StringComparison.OrdinalIgnoreCase)
                || role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                )
                .WithMessage(Messages.InvalidRole);
        }
    }

}
