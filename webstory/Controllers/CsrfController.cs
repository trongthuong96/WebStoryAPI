using Azure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.CrfsToken;
using Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webstory.Controllers
{
    [IgnoreAntiforgeryToken]
    //[EnableCors(SD.CORSNAME)]
    [ApiController]
    [Route("api/csrf")]
    public class CsrfController : ControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public CsrfController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        //[HttpPost("refresh-token")]
        //public ActionResult<CrfsToken> RefreshCsrfToken()
        //{
        //    var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
        //    CrfsToken crfsToken = new CrfsToken { Token = tokenSet.RequestToken };

        //    var tokenSet = antiforgery.GetAndStoreTokens(context);

        //    context.Response.Cookies.Append("XSRF-TOKEN", tokenSet.RequestToken!, new CookieOptions
        //    {
        //        HttpOnly = false,
        //        SameSite = SameSiteMode.Strict,  // Hoặc SameSiteMode.Strict
        //        Secure = true,  // Nếu sử dụng HTTPS
        //        Domain = "truyenmoi.click"
        //    });

        //    // Trả về token mới
        //    return Ok(crfsToken);
        //}

        [HttpGet("refresh-token")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> RefreshCsrfTokenAsync()
        {
            string token = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken!;

            CrfsToken crfsToken = new CrfsToken { Token = await SD.EncryptAsync(token) };

            await Task.Delay(0);

            // Trả về token mới
            return Ok(crfsToken);
        }
    }

}

