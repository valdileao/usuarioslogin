using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using UsuariosLogin.Models;

namespace UsuariosLogin.Infrastructure
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext);
            }
        }
        private Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(new ErrorDetails()
            {
                Message =  Startup.StaticConfig["MessagesError:Default"],
                ErrorCode = Convert.ToInt32(Startup.StaticConfig["MessagesError:DefaultErrorCode"])
            }.ToString());
        }
    }
}
