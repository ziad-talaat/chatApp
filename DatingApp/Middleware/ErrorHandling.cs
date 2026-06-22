using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DatingApp.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ErrorHandling(ILogger<ErrorHandling>logger, RequestDelegate _next,IHostEnvironment env)
    {
      

      
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex) {
                logger.LogError(ex,"{message}",ex.Message);
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode =(int)HttpStatusCode.InternalServerError;

                var resposen = env.IsDevelopment() ? new ErrorResult(ex.Message, httpContext.Response.StatusCode, ex.StackTrace) :
                    new ErrorResult(ex.Message, httpContext.Response.StatusCode,"Internal Server Error");


                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };
                var json = JsonSerializer.Serialize(resposen, options);
                await httpContext.Response.WriteAsync(json);
            }

        }
    }

    public static class ErrorHandlingExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandling>();
        }
    }
}
