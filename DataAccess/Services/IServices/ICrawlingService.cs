using System;
using Models.Dto;
using Models.Dto.Crawling;
using Models.Dto.Crawling.MeTruyenChu;

namespace DataAccess.Services.IServices
{
	public interface ICrawlingService
	{
        Task<(string, string)> GetBookMeTruyenCV(LinksDto links);
        Task<ChapterDto> GetChapterMeTruyenCV(string slug, short chapterIndex);
       // Task<BookDto> GetBookChapter2MeTruyenCV(string slug);

        // 69shuba
        Task<string> GetBook69shuba(string uri);
        Task<string> GetBookFanqie(string uri);
        Task<ChapterDto> GetContentChap69shuba(int chineseBookId, short chapterIndex);
        Task<ChapterDto> GetContentChapFanqie(string uriBook, int chineseBookId, short chapterIndex);
    }
}

