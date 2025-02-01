using Infrastructure.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace API.Middlewares
{
    //https://medium.com/@balibatuhan19/the-new-way-to-handle-exceptions-in-net-8-bb28f9029b02
    public class GlobalErrorHandling(RequestDelegate next, ILogger<GlobalErrorHandling> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<GlobalErrorHandling> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }
        /// <summary>
        /// Global exception handling method that base on exception return status and log exception
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            switch (exception)
            {
                case ValidationException validationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsJsonAsync(new { StatusCode = context.Response.StatusCode, Errors = validationException });
                    _logger.LogError(validationException.Message);
                    break;
                case NotFoundException notFoundException: //czy BookNotFoundException ?
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsJsonAsync(new { StatusCode = context.Response.StatusCode, Errors = notFoundException.Message });
                    _logger.LogError(notFoundException.Message);
                    break;
                case UnauthorizedAccessException unauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { StatusCode = context.Response.StatusCode, Errors = unauthorizedAccessException.Message });
                    _logger.LogError(unauthorizedAccessException.Message);
                    break;
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new { StatusCode = context.Response.StatusCode, Message = "Internal Server Error." });
                    _logger.LogError(exception.Message);
                    break;
            }
        }
    }
}
