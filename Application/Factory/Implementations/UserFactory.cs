using Application.Factory.Contracts;
using Infrastructure.Entities;

namespace Application.Factory.Implementations
{
	/// Base on https://medium.com/null-exception/factory-pattern-using-built-in-dependency-injection-of-asp-net-core-f91bd3b58665	/// </summary>
	public class UserFactory : IUserFactory
    {
		/// <summary>
		/// Create a user type base on provided role  using factory design pattern 
		/// </summary>
		/// <param name="role">User role</param>
		/// <param name="name">User name</param>
		/// <param name="email">User email </param>
		/// <param name="password">User password</param>
		/// <returns>The created user type base on role.</returns>
		/// <exception cref="ArgumentException">Thrown when the role is not supported</exception>
		/// <example>
		/// <code>
		///  IUserFactory _userFactory   = UserFactory();
		///  _userFactory.CreateUser("customer", "kacper", "kacper@gotoma.pl" , "password");
		/// </code>
		/// </example>


		public User CreateUser(string role, string name, string email, string password)
        {
            return role?.ToLower() switch
            {
                "librarian" => new Librarian { Name = name, Email = email },
                "customer" => new Customer { Name = name, Email = email },
                _ => throw new ArgumentException($"Invalid user role {nameof(role)}")
            };
        }
    }
}