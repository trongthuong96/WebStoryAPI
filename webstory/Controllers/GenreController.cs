using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto;
using Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webstory.Controllers
{
    [CustomValidateAntiForgeryToken]
    [Route("api/[controller]")]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetGenres()
        {
            var books = await _genreService.GetGenres();
            return Ok(books);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<GenreDto> GetBooksByGenreId(short id, int page)
        {
            if (page == 0)
            {
                page = 1;
            }
            return await _genreService.GetBooksByGenreId(id, page, 20);
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

