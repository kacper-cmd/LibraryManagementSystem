using FluentValidation;

namespace API.Extensions
{
	public static class ValidationExtensions
	{
		public static HttpValidationProblemDetails ToProblemDetails(this ValidationException ex)
		{
			var error = new HttpValidationProblemDetails
			{
				Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
				Status = StatusCodes.Status400BadRequest,
			};
			foreach (var validationFailure in ex.Errors)
			{
				if (error.Errors.ContainsKey(validationFailure.PropertyName))
				{
					error.Errors[validationFailure.PropertyName] =
						error.Errors[validationFailure.PropertyName]
						.Concat(new[] { validationFailure.ErrorMessage }).ToArray();
					continue;
				}
				error.Errors.Add(new KeyValuePair<string, string[]>(validationFailure.PropertyName,
					new[] { validationFailure.ErrorMessage }));
			}
			return error;
		}
	}
}
