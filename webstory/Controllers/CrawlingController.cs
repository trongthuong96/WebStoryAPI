using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Models.Dto.Crawling.MeTruyenChu;
using Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webstory.Controllers
{
    //[EnableCors(SD.CORSNAME)]
    [Route("api/[controller]")]
    public class CrawlingController : Controller
    {

        private readonly ICrawlingService _crawlingService;
        private readonly IChineseBookService _chineseBookService;

        public CrawlingController(ICrawlingService crawlingService, IChineseBookService chineseBookService)
        {
            _crawlingService = crawlingService;
            _chineseBookService = chineseBookService;
        }

        // GET: api/values
        [HttpPost("crawling")]
        public async Task<IActionResult> GetMetruyenCV([FromBody] LinksDto links)
        {

            var bookChapters = await _crawlingService.GetBookMeTruyenCV(links);

            if (bookChapters.Item1 == null)
                return BadRequest();

            return Ok(bookChapters.Item1);
        }


        // GET api/values/5
        [HttpGet("{slug}/{chapterIndex}")]
        public async Task<IActionResult> GetChapterMeTruyenCV(string slug, short chapterIndex)
        {
            var chapter = await _crawlingService.GetChapterMeTruyenCV(slug, chapterIndex);
            return Ok(chapter);
        }

        // POST api/values
        //[HttpPost("book")]
        //public async Task<IActionResult> GetBookChapter2MeTruyenCV([FromBody] UriDto uri)
        //{
        //    var book = await _crawlingService.GetBookChapter2MeTruyenCV(uri.Uri);
        //    return Ok(book);
        //}

        // POST api/book-listchap-crawl
        [HttpPost("book-listchap-crawl")]
        public async Task<IActionResult> GetBookAndListChapterCrawl([FromBody] UriDto uri)
        {
            var checkUri = uri.Uri.Split("/")[2].ToLower();
            var slug = "";
            if (checkUri.Equals(SD.LINK69SHU) || checkUri.Equals(SD.LINK69XINSHU))
            {
                slug = await _crawlingService.GetBook69shuba(uri.Uri);
            }
            else if (checkUri.Equals(SD.LINKFANQIE))
            {
                slug = await _crawlingService.GetBookFanqie(uri.Uri);
            }
            else
            {
                return NotFound();
            }

            return Ok(slug);
        }

        // POST api/chap-content-crawl
        [HttpPost("chap-content-crawl")]
        public async Task<ActionResult> GetContentChapCrawl([FromBody] Data data)
        {
            try
            {
                var chineseBook = await _chineseBookService.GetChineseBookById(data.chineseBookId);
                if (chineseBook == null)
                {
                    return NotFound();
                }

                var chapter = new ChapterDto();

                var uri = chineseBook.ChineseSite!;

                var checkUri = uri.Split("/")[2];

                if (checkUri.Equals(SD.LINK69SHU) || checkUri.Equals(SD.LINK69XINSHU))
                {
                    chapter = await _crawlingService.GetContentChap69shuba(data.chineseBookId, data.chapterIndex);
                }
                else if (checkUri.Equals(SD.LINKFANQIE))
                {
                    chapter = await _crawlingService.GetContentChapFanqie(chineseBook.ChineseSite!, data.chineseBookId, data.chapterIndex);
                }

                return Ok(chapter);
            }
            catch (NotFoundException ex)
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

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class Data
    {
        public int chineseBookId { get; set; }
        public short chapterIndex { get; set; }
    }
}

