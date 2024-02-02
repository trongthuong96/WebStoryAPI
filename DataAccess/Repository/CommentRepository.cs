using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Dto;
using Models.Dto.Book;
using Models.Dto.Comment;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataAccess.Repository
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context) {}

        public async Task<CommentTotalPage> GetCommentsAsync(int bookId, int page, int pageSize)
        {
            var commentsTemp = _context.Comments
                .Where(c => c.BookId == bookId && c.ParentId == null && c.Hide == false)
                .AsQueryable();

            var comments = await commentsTemp
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    CreatedAt = c.CreatedAt,
                    Content = c.Content,
                    BookId = c.BookId,
                    ParentId = c.ParentId,
                    UserId = c.UserId,
                    FullName = c.ApplicationUser.FullName,
                    Avatar = c.ApplicationUser.Avatar,
                    Count = _context.Comments
                    .Where(cr => cr.ParentId == c.Id && cr.Hide == false)
                    .Count()
                })
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Thêm phần này để chuyển kết quả thành một danh sách

            // Lấy tổng số lượng sách dựa trên điều kiện tìm kiếm
            var totalComments = await commentsTemp.CountAsync();

            // Tính toán totalPages
            var totalPages = (int)Math.Ceiling((double)totalComments / pageSize);

            var commentTotalPage = new CommentTotalPage
            {
                Comments = comments,
                TotalPage = totalPages
            };

            return commentTotalPage;
        }

        public async Task<CommentTotalPage> GetCommentsChildAsync(int commentId, int page, int pageSize)
        {
            var commentsTemp = _context.Comments
                .Where(c => c.ParentId == commentId && c.Hide == false)
                .AsQueryable();

            var comments = await commentsTemp
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    CreatedAt = c.CreatedAt,
                    Content = c.Content,
                    BookId = c.BookId,
                    ParentId = c.ParentId,
                    UserId = c.UserId,
                    FullName = c.ApplicationUser.FullName,
                    Avatar = c.ApplicationUser.Avatar,
                })
                .OrderBy(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Thêm phần này để chuyển kết quả thành một danh sách

            // Lấy tổng số lượng sách dựa trên điều kiện tìm kiếm
            var totalComments = await commentsTemp.CountAsync();

            // Tính toán totalPages
            var totalPages = (int)Math.Ceiling((double)totalComments / pageSize);

            var commentTotalPage = new CommentTotalPage
            {
                Comments = comments,
                TotalPage = totalPages
            };

            return commentTotalPage;
        }
    }
}
