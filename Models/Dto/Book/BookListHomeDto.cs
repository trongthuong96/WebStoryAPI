using System;
using Models.Dto.Chapter;

namespace Models.Dto.Book
{
	public class BookListHomeDto
	{
        public string Title { get; set; }

        public string Slug { get; set; }

        public short Status { get; set; }

        public long Views { get; set; }

        public string CoverImage { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public ICollection<GenreDto> Genres { get; set; }

        public ChapterLast ChapterLast { get; set; }
    }
}