using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Models.Dto.Crawling.MeTruyenChu;
using Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webstory.Controllers
{
    [CustomValidateAntiForgeryToken]
    [Route("api/[controller]")]
    public class CrawlingController : Controller
    {

        private readonly ICrawlingService _crawlingService;
        private readonly IChineseBookService _chineseBookService;
        private readonly IChapterService _chapterService;

        public CrawlingController(ICrawlingService crawlingService, IChineseBookService chineseBookService, IChapterService chapterService)
        {
            _crawlingService = crawlingService;
            _chineseBookService = chineseBookService;
            _chapterService = chapterService;
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
        [HttpPost("book-listchap-auto-crawl")]
        public async Task<IActionResult> GetBookAndListChapterAutoCrawl([FromBody] UriDto uri, int beginNumber, int count)
        {
            var checkUri = uri.Uri.Split("/")[2].ToLower();
            var uriList = new List<string>();
            if (checkUri.Equals(SD.LINK69SHU) || checkUri.Equals(SD.LINK69XINSHU))
            {
                uriList = await _crawlingService.GetBook69shubaAuto(uri.Uri, beginNumber, count);
            }
            //else if (checkUri.Equals(SD.LINKFANQIE))
            //{
            //    slug = await _crawlingService.GetBookFanqie(uri.Uri);
            //}
            else
            {
                return NotFound();
            }

            return Ok(uriList);
        }

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
        [HttpGet("chap-content-crawl")]
        public async Task<IActionResult> GetContentChapCrawl([FromQuery] Data data)
        {
            try
            {
                var chapter = new ChapterDto();

                if (data.ChineseBookId <= 0)
                {
                    chapter = await _chapterService.GetChapterConentAsync(data.BookId, data.ChineseBookId, data.ChapterIndex);

                    return Ok(chapter);
                }
                
                var chineseBook = await _chineseBookService.GetChineseBookById(data.ChineseBookId);
                if (chineseBook == null)
                {
                    return NotFound();
                }

                

                var uri = chineseBook.ChineseSite!;

                var checkUri = uri.Split("/")[2];

                if (checkUri.Equals(SD.LINK69SHU) || checkUri.Equals(SD.LINK69XINSHU))
                {
                    chapter = await _crawlingService.GetContentChap69shuba(data.BookId, data.ChineseBookId, data.ChapterIndex);
                }
                else if (checkUri.Equals(SD.LINKFANQIE))
                {
                    chapter = await _crawlingService.GetContentChapFanqie(chineseBook.ChineseSite!, data.BookId, data.ChineseBookId, data.ChapterIndex);
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

        //GetListChap69shuba
        // POST api/chap-content-crawl
        [HttpPost("list-chap-crawl")]
        public async Task<IActionResult> GetListChapCrawl([FromBody] DataChap data)
        {
            try
            {
                var chineseBook = await _chineseBookService.GetChineseBookById(data.ChineseBookId);
                if (chineseBook == null)
                {
                    return NotFound();
                }

                var chapter = new ChapterDto();

                var uri = chineseBook.ChineseSite!;

                var checkUri = uri.Split("/")[2];

                if (checkUri.Equals(SD.LINK69SHU) || checkUri.Equals(SD.LINK69XINSHU))
                {
                    var uriChapArray = chineseBook.ChineseSite!.Split(".");
                    string uriChap = uriChapArray[0] + "." + uriChapArray[1] + "." + uriChapArray[2] + "/";
                    await _crawlingService.GetListChap69shuba(uriChap, data.BookId, data.ChineseBookId);
                }
                else if (checkUri.Equals(SD.LINKFANQIE))
                {
                    await _crawlingService.GetListChapFanqie(chineseBook.ChineseSite!, data.BookId, data.ChineseBookId);
                }

                return Ok();
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
    }


    public class Data
    {
        public int BookId { get; set; }
        public int ChineseBookId { get; set; }
        public short ChapterIndex { get; set; }
    }

    public class DataChap
    {
        public int BookId { get; set; }
        public short ChineseBookId { get; set; }
    }
}

