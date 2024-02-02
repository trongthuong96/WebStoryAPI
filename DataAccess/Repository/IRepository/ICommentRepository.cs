using Models;
using Models.Dto.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<CommentTotalPage> GetCommentsAsync(int bookId, int page, int pageSize);
        Task<CommentTotalPage> GetCommentsChildAsync(int commentId, int page, int pageSize);
    }
}
