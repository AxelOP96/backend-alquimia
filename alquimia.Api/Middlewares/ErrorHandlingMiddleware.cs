using alquimia.Services.Models;
using System.Net;
using System.Text.Json;

namespace alquimia.Api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo una excepción no controlada.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int status;
            string error;

            switch (exception)
            {
                case ArgumentNullException:
                case ArgumentException:
                    status = (int)HttpStatusCode.BadRequest; // 400
                    error = string.IsNullOrWhiteSpace(exception.Message)
                        ? "Solicitud inválida. Los parámetros son incorrectos."
                        : exception.Message;
                    break;

                case UnauthorizedAccessException:
                    status = (int)HttpStatusCode.Unauthorized; //401
                    error = string.IsNullOrWhiteSpace(exception.Message)
                        ? "Acceso no autorizado."
                        : exception.Message;
                    break;

                case KeyNotFoundException:
                case NullReferenceException:
                    status = (int)HttpStatusCode.NotFound; //404
                    error = string.IsNullOrWhiteSpace(exception.Message)
                        ? "El recurso solicitado no fue encontrado."
                        : exception.Message;
                    break;

                case InvalidOperationException:
                    status = (int)HttpStatusCode.InternalServerError; //500
                    error = string.IsNullOrWhiteSpace(exception.Message)
                        ? "Ocurrió un error inesperado."
                        : exception.Message;
                    break;

                default:
                    status = (int)HttpStatusCode.InternalServerError; //500
                    error = "Ocurrió un error inesperado. Intente más tarde.";
                    break;
            }

            var response = new ErrorResponseDTO
            {
                Status = status,
                Error = error,
                Timestamp = DateTime.UtcNow,
                TraceId = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = status;
            return context.Response.WriteAsync(json);
        }
    }
}
