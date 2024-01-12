using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Dto
{
	public class ChapterDto
	{
        public short ChapNumber { get; set; }

        public short ChapterIndex { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public DateTime UpdatedAt { get; set; }

        public long? Views { get; set; }

        public string? BookTitle { get; set; }
    }
}

