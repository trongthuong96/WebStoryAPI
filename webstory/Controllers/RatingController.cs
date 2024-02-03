using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Rating;

namespace webstory.Controllers
{
    [CustomValidateAntiForgeryToken]
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : Controller
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet("bookId/{bookId}")]
        public async Task<IActionResult> GetRatingAvgAsync(int bookId)
        {
            return Ok(await _ratingService.GetRatingAvgAsync(bookId));
        }

        [HttpPost]
        public async Task<IActionResult> AddRating([FromBody] RatingCreateDto ratingCreate)
        {
            try
            {
                var rating = await _ratingService.AddRatingAsync(ratingCreate);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
