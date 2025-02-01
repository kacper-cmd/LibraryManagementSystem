using Application.Factory.Contracts;
using Application.Factory.Implementations;
using Infrastructure.Entities;

namespace UnitTest.ControllersUnitTest
{
	[TestFixture]
	public class UserFactoryTest
	{
		#region Fields&Properties

		private IUserFactory _factory;
		string name = string.Empty;
		string email = string.Empty;
		string password = string.Empty;

		#endregion

		[SetUp]
		public void SetUp()
		{
			_factory = new UserFactory();

			// Arrange
			name = "Kacper Obrzut";
			email = "kacper@gotoma.com";
			password = "password23";
		}
		#region TestCode

		[Test]
		[TestCase("librarian", typeof(Librarian))]
		[TestCase("customer", typeof(Customer))]
		public void CreateUser_ValidRole_Should_CreatesCorrectUserType(string role, Type expectedType)
		{
			// Act
			var user = _factory.CreateUser(role, name, email, password);

			// Assert
			Assert.IsInstanceOf(expectedType, user);
			Assert.That(name, Is.EqualTo(user.Name));
			Assert.That(email, Is.EqualTo(user.Email));
		}

		[Test]
		[TestCase("invalid")]
		[TestCase("Administrator")]
		public void CreateUser_InvalidRole_Should_ThrowsArgumentException(string role)
		{
			// Act & Assert
			var ex = Assert.Throws<ArgumentException>(() => _factory.CreateUser(role, name, email, password));
			Assert.That(ex.Message, Is.EqualTo($"Invalid user role {nameof(role)}"));
		}

		#endregion
	}
}
