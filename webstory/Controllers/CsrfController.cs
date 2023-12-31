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

        [Authorize(SD.ADMIN)]
        [HttpPost("refresh-token")]
        public ActionResult<CrfsToken> RefreshCsrfToken()
        {
            var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
            CrfsToken crfsToken = new CrfsToken { Token = tokenSet.RequestToken };

            // Trả về token mới
            return Ok(crfsToken);
        }


    }

}

