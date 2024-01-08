using FluentValidation;
using Microsoft.AspNetCore.Http;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monster.Application.Exceptions
{
    public class ExceptionMiddleware : IMiddleware
    {
        // HTTP isteğini işleyen asenkron metot.
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                // Hata durumunu ele almadan önce, isteği sonraki ortam katmanına (middleware) ileterek devam et.
                await next(httpContext);
            }
            catch (Exception ex)
            {
                // Eğer bir hata oluşursa, HandleExceptionAsync metodu çağrılır.
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        // Hata durumunu ele alan özel metot.
        private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            // Hata durumuna uygun HTTP durum kodunu alır.
            int statusCode = GetStatusCode(exception);

            // HTTP yanıtının içeriğinin JSON formatında olduğunu belirtir.
            httpContext.Response.ContentType = "application/json";

            // HTTP yanıtının durum kodunu ayarlar.
            httpContext.Response.StatusCode = statusCode;

            // Eğer fırlatılan istisna türü ValidationException ise:
            if (exception.GetType() == typeof(ValidationException))

                // ValidationException'dan gelen hataları alarak bir ExceptionModel oluşturur ve JSON olarak yanıtı yazar.
                return httpContext.Response.WriteAsync(new ExceptionModel
                {
                    // ValidationException'dan gelen hataları alır ve sadece hata mesajlarını içeren bir koleksiyon oluşturur.
                    Errors = ((ValidationException)exception).Errors.Select(x=>x.ErrorMessage),

                    // HTTP yanıtının durum kodunu belirtir.
                    StatusCode = StatusCodes.Status400BadRequest
                }.ToString());
            

            // Hata mesajlarını içeren bir liste oluşturulur.
            List<string> errors = new()
            {
                $"Hata: {exception.Message}",
                $"Hata Açıklaması : {exception.InnerException?.ToString()}"
            };

            // JSON formatında bir hata modeli oluşturulur ve HTTP yanıtı olarak yazılır.
            return httpContext.Response.WriteAsync(new ExceptionModel
            {
                Errors = errors,
                StatusCode = statusCode
            }.ToString());

        }

        // Hata durumuna göre uygun HTTP durum kodunu döndüren metot.
        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status400BadRequest,
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status500InternalServerError
            };
    }
}
