namespace PetAdopt.API.Middlewares
{
    using global::PetAdopt.Application.DTOs;
    using Microsoft.AspNetCore.Diagnostics;
    using System.Net;
    using System.Text.Json;

    namespace PetAdopt.API.Middlewares
    {
        public class GlobalExceptionHandler : IExceptionHandler
        {
            public async ValueTask<bool> TryHandleAsync(
                 HttpContext context,
                 Exception exception,
                 CancellationToken cancellationToken)
            {
                var statusCode = exception switch
                {
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                    InvalidOperationException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";


                //According ApiResponse that i made
                var response = ApiResponse<object>.Fail(exception.Message, statusCode);

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response), cancellationToken);

                return true;
            }
        }
    }
}
