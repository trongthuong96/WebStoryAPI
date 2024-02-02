using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Dto.BookReading
{
	public class BookReadingDto
	{
        public int BookId { get; set; }

        public int? ChineseBookId { get; set; }

        public short ChapNumber { get; set; }

        public string ChapTitle { get; set; }

        public short ChapterIndex { get; set; }

        public string BookSlug { get; set; }

        public string BookTitle { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}

