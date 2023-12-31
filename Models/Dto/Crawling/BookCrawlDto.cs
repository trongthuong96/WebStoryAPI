using System;
namespace Models.Dto.Crawling
{
	public class BookCrawlDto
	{
		public string ChineseSite { get; set; }

		public string ChineseTitle { get; set; }

		public string ChineseDescription { get; set; }

		public string ChineseSiteName { get; set; }

		public string ChineseAuthorName { get; set; }

		public string ChineseTagNames { get; set; }

		public List<string> ChineseGenreName { get; set; }

		public string ChineseCoverImage { get; set; }

		public short ChineseStatus { get; set; }
	}
}

