using System;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Utility;

namespace DataAccess.Middleware
{
    // Middleware để xác thực chữ ký
    public class SignatureVerificationMiddleware
    {
        private readonly RequestDelegate _next;

        public SignatureVerificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Lấy dữ liệu từ yêu cầu
            //var requestData = await GetRequestData(context.Request);
            // Lấy thời gian hiện tại
            DateTime currentTime = DateTime.UtcNow;

            // Lấy số phút
            int hour = currentTime.Hour + 1026;
            var requestData = hour.ToString();

            // Lấy chữ ký từ yêu cầu
            var signature = context.Request.Headers["X-Signature"].FirstOrDefault();

            // Kiểm tra chữ ký
            if (VerifySignature(requestData, signature))
            {
                // Nếu chữ ký hợp lệ, tiếp tục xử lý yêu cầu
                await _next(context);
            }
            else
            {
                // Nếu chữ ký không hợp lệ, trả về lỗi 401 Unauthorized
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }

        //private async Task<string> GetRequestData(HttpRequest request)
        //{
        //    using (var reader = new StreamReader(request.Body))
        //    {
        //        return await reader.ReadToEndAsync();
        //    }
        //}

        private bool VerifySignature(string data, string signature)
        {
            // Đây là một ví dụ đơn giản, bạn cần sử dụng thư viện xác thực chữ ký thực tế
            // Sử dụng khóa bí mật chia sẻ (shared secret key) giữa máy chủ và client

            // Trong thực tế, bạn nên lưu khóa bí mật một cách an toàn và không chia sẻ nó trong mã nguồn
            string sharedSecretKey = SD.SHAREDSECRETKEY;

            // Thực hiện hash dữ liệu sử dụng khóa bí mật
            string hashedData = ComputeHash(data, sharedSecretKey);

            // So sánh chữ ký được gửi từ client với chữ ký tính toán trên máy chủ
            return string.Equals(hashedData, signature, StringComparison.Ordinal);
        }

        private string ComputeHash(string data, string key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

    }

}

