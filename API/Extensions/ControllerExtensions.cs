using Application.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions
{
	public static class ControllerExtensions
	{
		public static IActionResult ToOk<TResult, TContract>(this Result<TResult, Error> result, Func<TResult, TContract> mapper)
		{
			return result.Match<IActionResult>(
				success =>
				{
					var response = Result<TContract, Error>.Ok(mapper(success));
					return new OkObjectResult(response);
				},
				error =>
				{
					return error.Code switch
					{
						var code when code == BookErrors.BookNotFound.Code => new NotFoundObjectResult(error.ToString()),
						var code when code == "search/error" => new BadRequestObjectResult(error.ToString()),
						var code when code == "sort/error" => new BadRequestObjectResult(error.ToString()),
						var code when code == "paging/error" => new StatusCodeResult(StatusCodes.Status500InternalServerError),
						_ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
					};
				}
			);
		}
	
	}
}
