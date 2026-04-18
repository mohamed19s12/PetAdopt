
using global::PetAdopt.Application.DTOs;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace PetAdopt.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }


        public async ValueTask<bool> TryHandleAsync(
                HttpContext context,
                Exception exception,
                CancellationToken cancellationToken)
        {
            _logger.LogError(exception,"Unhandled Exception: {Message} | Path: {Path}",
                exception.Message, context.Request.Path);

            var statusCode = exception switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Server Error: {Message} | Path: {Path}",
                    exception.Message, context.Request.Path);
            else
                _logger.LogWarning("{StatusCode}: {Message} | Path: {Path}",
                    statusCode, exception.Message, context.Request.Path);


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

