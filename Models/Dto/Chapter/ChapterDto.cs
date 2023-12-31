using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Dto
{
	public class ChapterDto
	{
        public long Id { get; set; }

        public short ChapNumber { get; set; }

        public short ChapterIndex { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public long? Views { get; set; }

        public int? BookId { get; set; }

        public int? ChineseBookId { get; set; }

        public string? BookTitle { get; set; }

        public string? BookSlug { get; set; }
    }
}

