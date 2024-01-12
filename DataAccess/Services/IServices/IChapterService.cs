using System;
using Models;
using Models.Dto;
using Models.Dto.Chapter;
using Models.Dto.Crawling;

namespace DataAccess.Services.IServices
{
	public interface IChapterService
	{
        Task<IEnumerable<ChapterDto>> GetChapters();
        Task<ChapterDto> GetChapterByIdAsync(long id);
        Task<ChapterDto?> GetChapterByChapterIndexAsync(string bookSlug, short chapterIndex);
        Task AddChapter(ChapterCreateDto chapterCreateDto);
        Task UpdateChapter(long id, ChapterUpdateDto chapterUpdateDto);
        Task DeleteChapter(long id);
        Task<IEnumerable<ChapterDto>?> GetChaptersByBookIdAsync(int bookId);
        Task AddAndUpdateChaptersCrawl(int bookId, int chineseBookId, IEnumerable<ChapterCrawllDto> chapterCrawlls);
        Task<Chapter> AddAndUpdateChaptersContentCrawl(int chineseBookId, short chapterIndex);
        Task<IEnumerable<ChapterListDto>?> GetChaptersByChineseBookIdAsync(int chineseBookId);
        Task<ChapterDto?> GetChapterConentAsync(int bookId, int chineseBookId, short chapterIndex);
    }
}

