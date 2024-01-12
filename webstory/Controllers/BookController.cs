using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto;
using Models.Dto.Book;
using Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webstory.Controllers
{
    //[CustomValidateAntiForgeryToken]
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _bookService.GetBooks();
            return Ok(books);
        }

        // find books order by updated at
        [HttpGet("OrderByUpdatedAt/{page}")]
        public async Task<ActionResult<IEnumerable<BooksDto>>> GetBooksOrderByUpdatedAt(int page)
        {
            var books = await _bookService.GetBooksOrderByUpdatedAtAsync(page, 20);
            return Ok(books);
        }

        // find books order by views
        [HttpGet("OrderByViews/{page}")]
        public async Task<ActionResult<IEnumerable<BooksDto>>> GetBooksOrderByViewsAt(int page)
        {
            var books = await _bookService.GetBooksOrderByViewsAtAsync(page, 16);
            return Ok(books);
        }

        // find books order by status complete
        [HttpGet("Status/{page}")]
        public async Task<ActionResult<IEnumerable<BooksDto>>> GetBooksStatusCompleteAsync(int page)
        {
            var books = await _bookService.GetBooksStatusCompleteAsync(page, 16);
            return Ok(books);
        }

        // find books order by status complete
        [HttpGet("Author/{authorId}")]
        public async Task<ActionResult<IEnumerable<BooksDto>>> GetBooksAuthorAsync(int authorId, int page)
        {
            if (page <= 0)
            {
                page = 1;
            }
            var books = await _bookService.GetBooksAuthorAsync(authorId, page, 10);
            return Ok(books);
        }

        // find books order by status complete
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<BooksDto>>> GetBooksUserAsync(string userId, int page)
        {
            if (page <= 0)
            {
                page = 1;
            }
            var books = await _bookService.GetBooksUserAsync(userId, page, 10);
            return Ok(books);
        }

        // find book by id
        [HttpGet("{bookId}")]
        public async Task<ActionResult<BookDto>> GetBookById(int bookId)
        {
            var book = await _bookService.GetBookByIdAsync(bookId);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // find book by slug
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<BookDto>> GetBookByTitleSlugAsync(string slug)
        {
            var book = await _bookService.GetBookByTitleSlugAsync(slug);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // find book by title
        [HttpGet("search")]
        public async Task<ActionResult<BookTotalPageResultDto>> GetBooksByTitleAsync(string title, int page, int pageSize)
        {
            if(page <= 0)
            {
                page = 1;
            }

            var book = await _bookService.GetBooksByTitleAsync(title, page, 20);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }
        
        // find book by title
        [HttpGet("searchAll")]
        public async Task<ActionResult<BookTotalPageResultDto>> GetBooksSearchAllAsync(string keyword, string status, short genre, short chapLength, int page, int pageSize)
        {
            if (page <= 0)
            {
                page = 1;
            }
            var statusArray = status?.Split(',').Select(s => int.Parse(s)).ToArray();
            var book = await _bookService.GetBooksSearchAllAsync(keyword, statusArray, genre, chapLength, page, 20);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddBook([FromBody] BookCreateDto bookCreateDto)
        {
            try
            {
                // Validate the model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Invalid registration data", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                await _bookService.AddBook(bookCreateDto);
                return Ok(new { Message = "Book added successfully" });
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

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBook(int id, [FromBody] BookUpdateDto bookUpdateDto)
        {
            try
            {
                await _bookService.UpdateBook(id, bookUpdateDto);
                return Ok(new { Message = "Book updated successfully" });
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

        [Authorize(SD.ADMIN)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            try
            {
                await _bookService.DeleteBook(id);
                return Ok(new { Message = "Book deleted successfully" });
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

