using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Dto;
using Models.Dto.Chapter;
using System.Data;

namespace DataAccess.Repository
{
    public class ChapterRepository : BaseRepository<Chapter>, IChapterRepository
    {
        public ChapterRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ChapterDto?> GetChapterByIdAsync(long id)
        {
            var chapterWithDetails = await _context.Chapters
                .Where(c => c.Id == id)
                .Select(c => new ChapterDto
                {
                    Title = c.Title,
                    ChapNumber = c.ChapNumber,
                    Content = c.Content,
                    ChapterIndex = c.ChapterIndex,
                    UpdatedAt = c.UpdatedAt,
                    Views = c.Views,
                    BookTitle = c.Book.Title
                })
                .FirstOrDefaultAsync();

            if (chapterWithDetails == null)
            {
                // Xử lý khi không tìm thấy sách
                return null; // hoặc throw exception tùy vào yêu cầu của bạn
            }

            return chapterWithDetails!;
        }

        public async Task<ChapterDto?> GetChapterByChapterIndexAsync(string bookSlug, short chapterIndex)
        {
            var chapterWithDetails = await _context.Chapters
                .Where(c => c.Book.Slug == bookSlug && c.ChapterIndex == chapterIndex && c.ChineseBookId == null)
                .Select(c => new ChapterDto
                {
                    Title = c.Title,
                    ChapNumber = c.ChapNumber,
                    Content = c.Content,
                    ChapterIndex = c.ChapterIndex,
                    UpdatedAt = c.UpdatedAt,
                    Views = c.Views,
                    BookTitle = c.Book.Title
                })
                .FirstOrDefaultAsync();

            if (chapterWithDetails == null)
            {
                // Xử lý khi không tìm thấy sách
                return null; // hoặc throw exception tùy vào yêu cầu của bạn
            }

            return chapterWithDetails!;
        }

        public async Task<IEnumerable<Chapter>?> GetChaptersByBookIdAsync(int bookId)
        {
            var chapterWithDetails = await _context.Chapters
                .Where(c => c.BookId == bookId && c.ChineseBookId == null)
                .Select(c => new Chapter
                {
                    Title = c.Title,
                    ChapNumber = c.ChapNumber,
                    ChapterIndex = c.ChapterIndex,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Views = c.Views
                })
                .OrderBy(c => c.ChapterIndex)
                .ToListAsync();

            if (chapterWithDetails == null)
            {
                // Xử lý khi không tìm thấy sách
                return null; // hoặc throw exception tùy vào yêu cầu của bạn
            }

            return chapterWithDetails!;
        }

        public async Task IncreaseChapterViewsAsync(long chapterId)
        {
            // Lấy sách từ cơ sở dữ liệu bằng ID
            var chapter = await _context.Chapters.FindAsync(chapterId);

            if (chapter == null)
            {
                throw new KeyNotFoundException("Book not found");
            }

            // Tăng views
            chapter.Views++;

            // Cập nhật thông tin sách vào cơ sở dữ liệu
            await _context.SaveChangesAsync();
        }

        public async Task IncreaseChapterViewsAsync(string bookSlug, short chapterIndex)
        {
            // Lấy sách từ cơ sở dữ liệu bằng ID
            var chapter = await _context.Chapters.SingleOrDefaultAsync(c => c.ChapterIndex == chapterIndex && c.Book.Slug == bookSlug);

            if (chapter == null)
            {
                throw new KeyNotFoundException("Book not found");
            }

            // Tăng views
            chapter.Views++;

            // Cập nhật thông tin sách vào cơ sở dữ liệu
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// get content chapter crawl
        /// </summary>
        /// <param name="chineseBookId"></param>
        /// <param name="chapterIndex"></param>
        /// <returns></returns>
        public async Task<ChapterDto?> GetChapterConentAsync(int bookId, int chineseBookId, short chapterIndex)
        {
            if (chineseBookId > 0)
            {
                var chapterWithDetails = await _context.Chapters
                  .Where(c => c.ChineseBookId == chineseBookId && c.ChapterIndex == chapterIndex)
                  .Select(c => new ChapterDto
                  {
                      Title = c.Title,
                      ChapNumber = c.ChapNumber,
                      Content = c.Content,
                      ChapterIndex = c.ChapterIndex,
                      UpdatedAt = c.UpdatedAt,
                      Views = c.Views,
                      BookTitle = c.Book.Title
                  })
                  .FirstOrDefaultAsync();

                if (chapterWithDetails == null)
                {
                    // Xử lý khi không tìm thấy sách
                    return null; // hoặc throw exception tùy vào yêu cầu của bạn
                }

                return chapterWithDetails!;
            }
            else
            {
                var chapterWithDetails = await _context.Chapters
                 .Where(c => c.BookId == bookId && c.ChapterIndex == chapterIndex && c.ChineseBookId == null)
                 .Select(c => new ChapterDto
                 {
                     Title = c.Title,
                     ChapNumber = c.ChapNumber,
                     Content = c.Content,
                     ChapterIndex = c.ChapterIndex,
                     UpdatedAt = c.UpdatedAt,
                     Views = c.Views,
                     BookTitle = c.Book.Title
                 })
                 .FirstOrDefaultAsync();

                if (chapterWithDetails == null)
                {
                    // Xử lý khi không tìm thấy sách
                    return null; // hoặc throw exception tùy vào yêu cầu của bạn
                }

                return chapterWithDetails!;
            }
        }

        // chinese book id
        public async Task<IEnumerable<ChapterListDto>?> GetChaptersByChineseBookIdAsync(int chineseBookId)
        {
            var chapterWithDetails = await _context.Chapters
                .Where(c => c.ChineseBookId == chineseBookId)
                .Select(c => new ChapterListDto
                {
                    Title = c.Title,
                    ChapNumber = c.ChapNumber,
                    ChapterIndex = c.ChapterIndex,
                    UpdatedAt = c.UpdatedAt
                })
                .OrderBy(c => c.ChapterIndex)
                .ToListAsync();

            if (chapterWithDetails == null)
            {
                // Xử lý khi không tìm thấy sách
                return null; // hoặc throw exception tùy vào yêu cầu của bạn
            }

            return chapterWithDetails!;
        }

        //private readonly string sqlConnectionString = "Server=sql.bsite.net\\MSSQL2016; TrustServerCertificate=True; MultiSubnetFailover=True;Initial Catalog=truyenhay_; User Id=truyenhay_; Password=Thuong@123;";

        //public async Task<IEnumerable<ChapterListDto>> GetChaptersByChineseBookIdAsync(int chineseBookId)
        //{
        //    using (var connection = new SqlConnection(sqlConnectionString))
        //    {

        //        string query = @"
        //            SELECT Title, ChapNumber, ChapterIndex, UpdatedAt
        //            FROM Chapters
        //            WHERE ChineseBookId = @ChineseBookId
        //            ORDER BY ChapterIndex";

        //        var parameters = new { ChineseBookId = chineseBookId };

        //        var chapterWithDetails = await connection.QueryAsync<ChapterListDto>(query, parameters);

        //        return chapterWithDetails;
        //    }
        //}

    }
}

