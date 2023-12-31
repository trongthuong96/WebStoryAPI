using System;
namespace Models.Dto.Crawling.MeTruyenChu
{
	public class BookChaptersCrawlingMTCDto
	{
		public BookCreateDto Book { get; set; }

		public IEnumerable<ChapterCreateDto> Chapters { get; set; }
	}
}

