
namespace Application.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
		public List<string> ValidationErrors { get; set; } = new List<string>();
		public ApiResponse()
        {
            Success = true;
        }
    }
}
//public IHttpActionResult Post(UserModel user)
//{
//    ApiResponse<string> response = new ApiResponse<string>();
//    try
//    {
//        // Call the UserService to create a new user
//        _userService.CreateUser(user);
//        response.Data = "User created successfully!";
//    }
//    catch (Exception ex) when (ex is ValidationException || ex is InternalServerErrorException)
//    {
//        // Handle both ValidationException and InternalServerErrorException here
//        response.Success = false;
//        response.ErrorMessage = ex.Message;
//    }
//    catch (Exception ex)
//    {
//        // Log other unexpected exceptions
//        LogException(ex);
//        response.Success = false;
//        response.ErrorMessage = "An unexpected error occurred.";
//    }
//    return Ok(response);
//}