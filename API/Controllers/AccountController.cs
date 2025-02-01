using API.Extensions;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Application.DTOs.CustomResponses;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
		private readonly ILogger<AccountController> _logger;
		private readonly IAccount _account;
        private readonly IUserService _userService;
        public AccountController(ILogger<AccountController> logger, IAccount account, IUserService userService)
        {
            _logger = logger;
            _account = account;
            _userService = userService;
		}
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegistrationResponse>> RegisterAsync(RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
				return BadRequest(ModelState);
            }
            var result = await _account.RegisterAsync(model);
            if (result.Flag) 
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> LoginAsync(LoginDTO user)
        {
            var result = await _account.LoginAsync(user);
            if (result.Flag) 
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public ActionResult<LoginResponse> RefreshToken(UserSession userSession)
        {
            var result = _account.RefreshToken(userSession);
            if (result.Flag) 
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetCurrentUser()
        {
			var userId = User.GetUserId();
			var s = User.GetUserEmail();
            if (userId == null)
            {
				_logger.LogError($"Unauthorized acccess -> {userId}");
				return Unauthorized();
            }

            var user = await _userService.GetUser(Guid.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new UserDTO
            {
                ID = user.ID,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            });
        }
    }
}
