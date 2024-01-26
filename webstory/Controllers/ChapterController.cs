using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Models.Dto.Chapter;
using Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webstory.Controllers
{
    //[CustomValidateAntiForgeryToken]
    [Route("api/[controller]")]
    public class ChapterController : Controller
    {
        private readonly IChapterService _chapterService;

        public ChapterController(IChapterService chapterService)
        {
            _chapterService = chapterService;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{chapterId}")]
        public async Task<IActionResult> GetChapterById(long chapterId)
        {
            var book = await _chapterService.GetChapterByIdAsync(chapterId);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // GET api/values/5
        [HttpGet("chapterIndex/{bookSlug}/{chapterIndex}")]
        public async Task<IActionResult> GetChapterByChapterIndexAsync(string bookSlug, short chapterIndex)
        {
            var book = await _chapterService.GetChapterByChapterIndexAsync(bookSlug, chapterIndex);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // GET api/values
        [HttpGet("list/{bookId}")]
        public async Task<IActionResult> GetChaptersByBookIdAsync(int bookId)
        {
            var book = await _chapterService.GetChaptersByBookIdAsync(bookId);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddChapter([FromBody] ChapterCreateDto chapterCreateDto)
        {
            try
            {
                // Validate the model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Invalid registration data", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                await _chapterService.AddChapter(chapterCreateDto);
                return Ok(new { Message = "Chapter added successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error. " + ex.Message });
            }
        }

        // PUT api/values/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChapter(int id, [FromBody] ChapterUpdateDto chapterUpdateDto)
        {
            try
            {
                await _chapterService.UpdateChapter(id, chapterUpdateDto);
                return Ok(new { Message = "Chapter updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        // DELETE api/values/5
        [Authorize(SD.ADMIN)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _chapterService.DeleteChapter(id);
                return Ok(new { Message = "Chapter deleted successfully" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        // CHINESE BOOK ID
        [HttpGet("list-chinese/{chineseBookId}")]
        public async Task<IActionResult> GetChaptersByChineseBookIdAsync(int chineseBookId,[FromQuery] int page, [FromQuery] int pageSize, [FromQuery] int arrange)
        {
            var responseChaps = await _chapterService.GetChaptersByChineseBookIdAsync(chineseBookId, page, pageSize, arrange);

            // Sử dụng ChapterApiResponse để đóng gói dữ liệu trả về
            var apiResponse = new ChapterApiResponse
            {
                Chapters = responseChaps.Item1,
                Total = responseChaps.Item2
            };

            return Ok(apiResponse);
        }
    }

    public class ChapterApiResponse
    {
        public IEnumerable<ChapterListDto>? Chapters { get; set; }
        public int Total { get; set; }
    }
}

