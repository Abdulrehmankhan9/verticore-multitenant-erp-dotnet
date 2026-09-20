using System.Net;
using System.Text.Json;
using VertiCore.Application.Exceptions;
using VertiCore.Domain.Exceptions;

namespace VertiCore.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
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
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                ValidationException => HttpStatusCode.BadRequest,
                ClientNotFoundException => HttpStatusCode.NotFound,
                InvoiceNotFoundException => HttpStatusCode.NotFound,
                TenantNotFoundException => HttpStatusCode.NotFound,
                InvalidCredentialsException => HttpStatusCode.Unauthorized,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            object response;

            if (exception is ValidationException validationEx)
            {
                response = new
                {
                    success = false,
                    message = exception.Message,
                    errors = validationEx.Errors
                };
            }
            else
            {
                response = new
                {
                    success = false,
                    message = exception.Message
                };
            }

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}