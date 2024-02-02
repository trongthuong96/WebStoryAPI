using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto;
using Models.Dto.Comment;

namespace webstory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentsAsync(int bookId, int page, int pageSize)
        {
            return Ok(await _commentService.GetCommentsAsync(bookId, page, pageSize));
        }

        [HttpGet("child")]
        public async Task<IActionResult> GetCommentsChildAsync(int commentId, int page, int pageSize)
        {
            return Ok(await _commentService.GetCommentsChildAsync(commentId, page, pageSize));
        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CommentCreateDto commentCreate)
        {
            try
            {
                var comment = await _commentService.AddCommentAsync(commentCreate);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(long id)
        {
            try
            {
                var comment = await _commentService.DeleteCommentAsync(id);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
