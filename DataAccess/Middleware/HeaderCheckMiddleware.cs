using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DataAccess.Middleware
{
	public class HeaderCheckMiddleware
	{
        private readonly RequestDelegate _next;

        public HeaderCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var headers = context.Request.Headers;
            // Kiểm tra các header ở đây
            // Ví dụ: Kiểm tra header có tên là "X-CSRF-TOKEN"
            if (headers.ContainsKey("X-CSRF-TOKEN"))
            {
                var authHeaderValue = headers["X-CSRF-TOKEN"];
                // Xử lý giá trị Authorization header ở đây
                Console.WriteLine("XSRF" + authHeaderValue);
            }

            // Chuyển request sang middleware tiếp theo trong pipeline
            await _next(context);
        }
    }

    public static class HeaderCheckMiddlewareExtensions
    {
        public static IApplicationBuilder UseHeaderCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HeaderCheckMiddleware>();
        }
    }


}

