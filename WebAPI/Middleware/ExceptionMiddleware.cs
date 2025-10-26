using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPI.Middleware
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
                _logger.LogError(ex, "Bir hata oluştu: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                ApplicationException => new
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Uygulama hatası: " + exception.Message
                },
                KeyNotFoundException => new
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Kaynak bulunamadı: " + exception.Message
                },
                UnauthorizedAccessException => new
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "Yetkisiz erişim: " + exception.Message
                },
                DbUpdateConcurrencyException => new
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Message = "Veri çakışması: " + exception.Message
                },
                _ => new
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "İç sunucu hatası: " + exception.Message
                }
            };

            // StackTrace'i ayrı olarak ekleyelim
            var result = JsonSerializer.Serialize(new
            {
                response.StatusCode,
                response.Message,
                StackTrace = exception.StackTrace
            });

            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsync(result);
        }
    }
}