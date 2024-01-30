using shoppingify_backend.Helpers.CustomExceptions;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;

namespace shoppingify_backend.Helpers
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = exception switch
            {
                NotFoundException => NotFoundException.ErrorCode,
                ForbiddenException => ForbiddenException.ErrorCode,
                ValidationException => ValidationException.ErrorCode,
                BadRequestException => BadRequestException.ErrorCode,
                _ => HttpStatusCode.InternalServerError

            };

            var result = JsonSerializer.Serialize(new {  message = exception.Message  });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
