using System;
using System.Net;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Dto;
using Models.Dto.Chapter;

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
                .Select(c => new ChapterDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    ChapNumber = c.ChapNumber,
                    Content = c.Content,
                    ChapterIndex = c.ChapterIndex,
                    UpdatedAt = c.UpdatedAt,
                    BookId = c.BookId,
                    Views = c.Views,
                    BookTitle = c.Book.Title,
                    BookSlug = c.Book.Slug
                })
                .FirstOrDefaultAsync(c => c.Id == id);

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
                .Select(c => new ChapterDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    ChapNumber = c.ChapNumber,
                    Content = c.Content,
                    ChapterIndex = c.ChapterIndex,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    BookId = c.BookId,
                    Views = c.Views,
                    BookTitle = c.Book.Title,
                    BookSlug = c.Book.Slug
                })
                .FirstOrDefaultAsync(c => c.BookSlug == bookSlug && c.ChapterIndex == chapterIndex);

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
                    Id = c.Id,
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
        public async Task<ChapterDto?> GetChapterByChapterConentCrawlAsync(int chineseBookId, short chapterIndex)
        {
            var chapterWithDetails = await _context.Chapters
                .Where(c => c.ChineseBookId == chineseBookId && c.ChapterIndex == chapterIndex)
                .Select(c => new ChapterDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    ChapNumber = c.ChapNumber,
                    Content = c.Content,
                    ChapterIndex = c.ChapterIndex,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    BookId = c.BookId,
                    Views = c.Views,
                    BookTitle = c.Book.Title,
                    BookSlug = c.Book.Slug,
                    ChineseBookId = c.ChineseBookId
                })
                .FirstOrDefaultAsync();

            if (chapterWithDetails == null)
            {
                // Xử lý khi không tìm thấy sách
                return null; // hoặc throw exception tùy vào yêu cầu của bạn
            }

            return chapterWithDetails!;
        }

        // chinese book id
        public async Task<IEnumerable<Chapter>?> GetChaptersByChineseBookIdAsync(int chineseBookId)
        {
            var chapterWithDetails = await _context.Chapters
                .Where(c => c.ChineseBookId == chineseBookId)
                .Select(c => new Chapter
                {
                    Id = c.Id,
                    Title = c.Title,
                    ChapNumber = c.ChapNumber,
                    ChapterIndex = c.ChapterIndex,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Views = c.Views,
                    ChineseBookId = c.ChineseBookId
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
    }
}

