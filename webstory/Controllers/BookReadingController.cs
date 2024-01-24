using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Mvc;

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
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

