using Newtonsoft.Json;
using Register.Web.CustomExceptions;
using System.Net;

namespace Register.Web.Middlewares
{
    public class CustomExceptionHandler
    {
        private readonly RequestDelegate _next;

        public CustomExceptionHandler(RequestDelegate next)
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

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            var result = string.Empty;

            switch (exception)
            {
                //для обробки 400 коду помилки
                case ResultEmptyException resultEmpty:
                    code = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(resultEmpty.Message);
                    break;

                //для обробки 500 коду помилки
                case ServerErrorsException resultServerError:
                    code = HttpStatusCode.InternalServerError;
                    result = JsonConvert.SerializeObject(resultServerError.Message);
                    break;

                default:
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            if (string.IsNullOrEmpty(result))
            {
                result = JsonConvert.SerializeObject(new { error = exception.Message });
            }

            return context.Response.WriteAsync(result);
        }
    }

    public static class CustomExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandler>();
        }
    }
}
