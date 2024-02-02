using System;
using System.ComponentModel.DataAnnotations;
using Models.Dto.Chapter;

namespace Models.Dto
{
	public class GenreDto
	{
        public short Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public int TotalPages { get; set; }

        public ICollection<BookInGenreDto> BookInGenreDtos { get; set; }
    }

    public class BookInGenreDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public short Status { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public string CoverImage { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public string AuthorName { get; set; }

        public long Views { get; set; }

        public ChapterLast ChapterLast { get; set; }
    }
}

