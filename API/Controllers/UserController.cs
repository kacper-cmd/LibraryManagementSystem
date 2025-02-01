using Application.DTOs;
using Application.Factory.Contracts;
using Application.Factory.Implementations;
using Application.Interfaces.Services;
using Application.RequestModel;
using Application.Services;
using FluentValidation;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Infrastructure.Exceptions;
using Infrastructure.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IValidator<UserDTO> _userValidator;
        private readonly IUserService _userService;
        private readonly IUserFactory _userFactory;

        public UserController(ILogger<UserController> logger, IValidator<UserDTO> userValidator,
            IUserService userService, IUserFactory userFactory)
        {
            _logger = logger;
            _userValidator = userValidator;
            _userService = userService;
            _userFactory = userFactory;
        }


        [HttpGet]
        public async ValueTask<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var result = await _userService.GetUsers();

            return Ok(result);
        }


        [HttpGet("{id:guid}")]
        public async ValueTask<ActionResult<UserDTO>> Get(Guid id)
        {
            try
            {
                var result = await _userService.GetUser(id);

                return Ok(result);
            }
            catch(NotFoundException e)
            {
				return NotFound(e);
			}
            catch (Exception ex)
            {
				_logger.LogError(message: ex.Message, ex);
				return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

		[HttpGet("get-users-paged")]
        [Authorize]
		public ActionResult<PagedListDTO<UserDTO>> GetUsersPaged(BaseFilter query)
		{
			try
			{
				var users = _userService.GetPagedListAsync(query);
				return Ok(users);
			}
			catch (Exception ex)
			{
				_logger.LogError(message: ex.Message, ex);
				return StatusCode(StatusCodes.Status500InternalServerError,
				 "Error retrieving data from the database");
			}
		}
		[HttpPost]
        public async ValueTask<ActionResult<UserDTO>> Create(UserDTO dto)
        {
            try
            {
                if (dto == null)
                {
					return BadRequest();
				}
				var validationResult = _userValidator.Validate(dto);
				if (!validationResult.IsValid)
				{
					var errors = validationResult.Errors;
					return BadRequest(errors);
				}
				var createdUser = await _userService.AddUser(dto);

                return Ok(createdUser);
            }
            catch (Exception ex)
            {
				_logger.LogError(message: ex.Message, ex);
				return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new user record");
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = CustomRoles.Admin)]
        public async Task<ActionResult<UserDTO>> Update(Guid id, UserDTO dto)
        {
            try
            {
                var userToUpdate = await _userService.GetUser(id);
				var validationResult = _userValidator.Validate(dto);
				if (!validationResult.IsValid)
				{
					var errors = validationResult.Errors;
					return BadRequest(errors);
				}
				if (userToUpdate == null)
                    return NotFound($"User with Id = {id} not found");

                var user = await _userService.UpdateUser(dto);

                return Ok(user);
            }
            catch (Exception ex)
            {
				_logger.LogError(message: ex.Message, ex);
				return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = CustomRoles.Admin)]
        public async Task<ActionResult<Guid>> Delete(Guid id)
        {
            try
            {
                var userToDelete = await _userService.GetUser(id);

                if (userToDelete == null)
                {
                    return NotFound($"Employee with Id = {id} not found");
                }
               await _userService.DeleteUser(id);
                return Ok(userToDelete);
            }
            catch (Exception ex)
            {
				_logger.LogError(message: ex.Message, ex);
				return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
        [HttpPatch("{id:guid}")]
        //[Authorize]
        [Authorize(Roles = CustomRoles.AdminOrLibrarian)]
        public async Task<ActionResult<UserDTO>> PatchUserAsync(Guid id, JsonPatchDocument<UserDTO> patchDocument)
        {
            var userDTO = await _userService.GetUser(id);

            if (userDTO == null) return BadRequest();
            patchDocument.ApplyTo(userDTO);
            var validationResult = _userValidator.Validate(userDTO);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors;
                return BadRequest(errors);
            }

            var user = await _userService.UpdateUser(userDTO);

            return Ok(user);
        }


        #region UserFactory 
        [HttpGet("users-factory/{userRole}/{name}/{email}/{password}")]
        public User GetMovies(string userRole, string name, string email, string password)
        {
            return _userFactory.CreateUser(userRole, name, email, password);
        }

        #endregion
    }
}