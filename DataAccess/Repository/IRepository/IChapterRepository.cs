using System;
using Models;
using Models.Dto;
using Models.Dto.Chapter;

namespace DataAccess.Repository.IRepository
{
    public interface IChapterRepository : IRepository<Chapter>
    {
        Task<ChapterDto?> GetChapterByIdAsync(long id);
        Task<ChapterDto?> GetChapterByChapterIndexAsync(string bookSlug, short chapterIndex);
        Task IncreaseChapterViewsAsync(long chapterId);
        Task IncreaseChapterViewsAsync(string bookSlug, short chapterIndex);
        Task<IEnumerable<Chapter>?> GetChaptersByBookIdAsync(int bookId);

        // crawl content chapter
        Task<ChapterDto?> GetChapterConentAsync(int bookId, int chineseBookId, short chapterIndex);
        Task<IEnumerable<ChapterListDto>?> GetChaptersByChineseBookIdAsync(int chineseBookId);
    }
}

