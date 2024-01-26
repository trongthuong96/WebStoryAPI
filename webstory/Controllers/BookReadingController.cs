using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Services;
using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto.BookReading;
using Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webstory.Controllers
{
    [CustomValidateAntiForgeryToken]
    [Route("api/[controller]")]
    public class BookReadingController : Controller
    {

        private readonly IBookReadingService _bookReadingService;

        public BookReadingController(IBookReadingService bookReadingService)
        {
            _bookReadingService = bookReadingService;
        }

        // GET: api/values
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetBookReadingsByUserIdAsync()
        {
            var books = await _bookReadingService.GetBookReadingsByUserIdAsync();
            return Ok(books);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [Authorize]
        [HttpDelete("{bookId}/{chineseBookId}")]
        public async Task<IActionResult> Delete(int bookId, int chineseBookId)
        {
            try
            {
                int result = await _bookReadingService.Delete(bookId, chineseBookId);

                if (result == 400)
                {
                    return BadRequest("Not Exist User");
                }
                
                if (result == 404)
                {
                    return NotFound("Not found book reading");
                }

                return Ok(new { Message = "Book reading deleted successfully" });
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
    }
}

