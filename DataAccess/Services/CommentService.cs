using AutoMapper;
using DataAccess.Repository.IRepository;
using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.Dto.Comment;
using System.Net;
using System.Security.Claims;

namespace DataAccess.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentService
        (
            ICommentRepository commentRepository, 
            IHttpContextAccessor httpContextAccessor, 
            IMapper mapper, 
            UserManager<ApplicationUser> userManager
        )
        {
            _commentRepository = commentRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _mapper = mapper;
        }

        // order by updatedAt
        public async Task<IEnumerable<Comment?>> GetCommentsByBookAsync(int bookId)
        {
            var books = await _commentRepository.FindAsync(c => c.BookId == bookId);

            return books;
        }

        public async Task<CommentDto> AddCommentAsync(CommentCreateDto commentCreate)
        {
            if (commentCreate == null)
            {
                throw new InvalidOperationException("Is not null");
            }

            // Lấy thông tin user hiện tại từ HttpContextAccessor
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra xem UserId của người dùng đang thao tác có trong token hay không
            if (userId == null)
            {
                throw new InvalidOperationException("Invalid User");
            }

            if (string.IsNullOrEmpty(commentCreate.Content) || commentCreate.Content.Length < 3)
            {
                throw new InvalidOperationException("ContentMustLength3");
            }

            var comment = _mapper.Map<Comment>(commentCreate);

            comment.UserId = userId;
            comment.CreatedAt = DateTimeOffset.UtcNow;

            await _commentRepository.AddAsync(comment);

            var commentDto = new CommentDto();
            commentDto.CreatedAt = comment.CreatedAt;
            commentDto.Content = comment.Content;
            commentDto.Id = comment.Id;
            commentDto.ParentId = comment.ParentId;
            commentDto.BookId = comment.BookId;

            var user = await _userManager.FindByIdAsync(userId);
            commentDto.UserId = user.Id;
            commentDto.FullName = user.FullName;
            commentDto.Avatar = user.Avatar;

            return commentDto;
        }

        // Get comment
        public async Task<CommentTotalPage> GetCommentsAsync(int bookId, int page, int pageSize)
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (pageSize <= 0 || pageSize > 100)
            {
                pageSize = 20;
            } 

            var comments = await _commentRepository.GetCommentsAsync(bookId, page, pageSize);

            return comments;
        }

        // get comment child
        public async Task<CommentTotalPage> GetCommentsChildAsync(int commentId, int page, int pageSize)
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (pageSize <= 0 || pageSize > 100)
            {
                pageSize = 20;
            }

            var comments = await _commentRepository.GetCommentsChildAsync(commentId, page, pageSize);

            return comments;
        }

        public async Task<bool> DeleteCommentAsync(long id)
        {
            if (id < 0)
            {
                throw new InvalidOperationException("id not exist");
            }

            // Lấy thông tin user hiện tại từ HttpContextAccessor
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra xem UserId của người dùng đang thao tác có trong token hay không
            if (userId == null)
            {
                throw new InvalidOperationException("Invalid User");
            }

            var comment = await _commentRepository.FindSingleAsync(c => c.Id == id && c.UserId == userId);

            if (comment == null)
            {
                throw new KeyNotFoundException("Not find comment");
            }

            var commentChild = await _commentRepository.FindAsync(c => c.ParentId == comment.Id);

            if (commentChild != null)
            {
                foreach(var commentTemp in commentChild)
                {
                    commentTemp.Hide = true;
                }

                await _commentRepository.BulkUpdateRangeAsync(commentChild);
            }

            comment.Hide = true;
            await _commentRepository.UpdateAsync(comment);

            return true;
        }
    }
}
