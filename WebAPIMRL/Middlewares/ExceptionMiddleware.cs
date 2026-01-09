using Microsoft.Data.SqlClient;
using System.Net;
using System.Text.Json;

namespace WebAPIMRL.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // 🔹 Clasificación de errores
            if (exception is SqlException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
            }

            var response = new
            {
                status = context.Response.StatusCode,
                message = "Ocurrió un error durante el procesamiento de la solicitud.",
                detail = exception.Message // opcional ocultar en producción
            };

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        }
    }
}
