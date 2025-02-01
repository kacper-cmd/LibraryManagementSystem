using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Token;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Application.DTOs.CustomResponses;

namespace Database.Repository
{
    public class Account : IAccount
    {
        private readonly ApplicationDbContext _context;

        private readonly IConfiguration _config;
        public Account(IConfiguration config, ApplicationDbContext context)
        {
			ArgumentNullException.ThrowIfNull(nameof(config));
			ArgumentNullException.ThrowIfNull(nameof(context));
			_context = context;
            _config = config;
        }
		/// <summary>
		/// Login user method with  generating jwt token
		/// </summary>
		/// <param name="user">The user's login data</param>
		/// <returns>The Login Response with info about success </returns>
		public async Task<LoginResponse> LoginAsync(LoginDTO user)
        {
            var findUser = await GetUser(user.Email);
            if (findUser == null)
            {
                return new LoginResponse(false, "User not found");
            }
            if (!BCrypt.Net.BCrypt.Verify(user.Password, findUser?.Password))
            {
                return new LoginResponse(false, "Email/Password not valid");
            }
            string jwtToken = GenerateToken(findUser);
            return new LoginResponse(true, "User logged in successfully", jwtToken);
        }
		/// <summary>
		/// Generate JWT token to given user
		/// </summary>
		/// <param name="user">The user's to generate jwt token to him</param>
		/// <returns>The jwt token</returns>
		private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(_config["Jwt:ExpiryInDays"]!));
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role!)
            };
            var token = new JwtSecurityToken(
              issuer: _config["Jwt:Issuer"]!,
             audience: _config["Jwt:Issuer"]!,
              claims: userClaims,
              expires: expiry,
              signingCredentials: credentials
              );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
		/// <summary>
		/// Register a user and storing password in encrypted way
		/// </summary>
		/// <param name="model">The dto modal providing data from registration</param>
		/// <returns>The Registration Response which containe flag to indicate result and message</returns>
		public async Task<CustomResponses.RegistrationResponse> RegisterAsync(RegisterDTO model)
        {
            var findUser = await GetUser(model.Email);
            if (findUser != null)
            {
                return new RegistrationResponse(false, "User already exists");
            }
            if (findUser != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(model.Password, findUser?.Password))
                {
                    return new RegistrationResponse(false, "Email/Password not valid ");
                }
            }

            _context.Users.Add(
                new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Role = model.Role,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
                });
            await _context.SaveChangesAsync();
            return new RegistrationResponse(true, "User created successfully");

        }

        private async Task<User> GetUser(string email) =>
            await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

		/// <summary>
		/// Refresh token if it has been expired 
		/// </summary>
		/// <param name="userSession">The user jwt token in current session </param>
		/// <returns>The Login Response which contain flag to indicate result and message and newly created JWT token  </returns>
		public LoginResponse RefreshToken(UserSession userSession)
        {
            CustomUserClaims customUserClaims = DecryptJWTService.DecryptToken(userSession.JWTToken);
            if (customUserClaims is null) return new LoginResponse(false, "Invalid token");

            string newToken = GenerateToken(new User
            {
                Name = customUserClaims.Name,
                Email = customUserClaims.Email,
                Role = customUserClaims.Role
            });
            return new LoginResponse(true, "Token refreshed successfully", newToken);
        }
    }

}
