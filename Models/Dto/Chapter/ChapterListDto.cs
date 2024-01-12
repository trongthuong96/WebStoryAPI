using System;
namespace Models.Dto.Chapter
{
	public class ChapterListDto
	{
        public short ChapNumber { get; set; }

        public short ChapterIndex { get; set; }

        public string? Title { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

