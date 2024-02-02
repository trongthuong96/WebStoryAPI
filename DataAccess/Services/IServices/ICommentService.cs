using Models;
using Models.Dto.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.IServices
{
    public interface ICommentService
    {
        Task<CommentDto> AddCommentAsync(CommentCreateDto commentCreate);

        Task<IEnumerable<Comment?>> GetCommentsByBookAsync(int bookId);

        Task<CommentTotalPage> GetCommentsAsync(int bookId, int page, int pageSize);

        Task<CommentTotalPage> GetCommentsChildAsync(int commentId, int page, int pageSize);

        Task<bool> DeleteCommentAsync(long id);
    }
}
