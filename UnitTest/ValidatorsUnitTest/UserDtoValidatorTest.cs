using Application.DTOs;
using Application.ValidatorsDto;
using FluentValidation.TestHelper;
using Infrastructure.Constants;

namespace UnitTest.ValidatorsUnitTest
{
    [TestFixture]
    public class UserDtoValidatorTest
	{
		#region Fields&Properties

		private UserDtoValidator _validator;
		#endregion
		[SetUp]
        public void SetUp()
        {
            _validator = new UserDtoValidator();
        }

		#region TestCode
		[Test]
        public void Should_Have_Error_When_Name_Is_Too_Long()
        {
            var model = new UserDTO
            {
                Email = "kacper@gotoma.com",
                ID = Guid.NewGuid(),
                Name = new string('a', 101),
                Password = "Password",
                Role = "admiN"
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(user => user.Name)
                  .WithErrorMessage(Messages.ValidName);
		}

        [Test]
        public void Should_Have_Error_When_Name_Is_Too_Short()
        {
            var model = new UserDTO
            {
                Email = "kacper@gotoma.com",
                ID = Guid.NewGuid(),
                Name = "ab",
                Password = "Password",
                Role = "admiN"
            };
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(user => user.Name).
                WithErrorMessage(Messages.MinUserName);
		}

		[Test]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = new UserDTO
            {
                Email = "email",
                ID = Guid.NewGuid(),
                Name = "hab",
                Password = "Password",
                Role = "admiN"
            };
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(user => user.Email)
                  .WithErrorMessage(Messages.InvalidEmail);
        }

        [Test]
        public void Should_Have_Error_When_Role_Is_Invalid()
        {
            var model = new UserDTO
            {
                Email = "kacper@gotoma.com",
				ID = Guid.NewGuid(),
                Name = "hab",
                Password = "Password",
                Role = "InvalidRole"
			};
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(user => user.Role)
                  .WithErrorMessage(Messages.InvalidRole);
        }

        [Test]
        public void Should_Not_Have_Error_When_Model_Is_Valid()
        {
            var model = new UserDTO
            {
                Email = "kacper@gotoma.com",
                ID = Guid.NewGuid(),
                Name = "Test",
                Password = "Password",
                Role = "admiN"
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
		#endregion
	}
}