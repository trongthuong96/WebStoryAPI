using System;
using System.ComponentModel.DataAnnotations;
using Models.Dto.Chapter;

namespace Models.Dto.Book
{
	public class BooksDto
	{
        public int Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public short Status { get; set; }

        public string Description { get; set; }

        public string CoverImage { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public long Views { get; set; }

        public string ApplicationUserUserName { get; set; }

        public int AuthorId { get; set; }

        public string AuthorName { get; set; }

        public ICollection<GenreDto> Genres { get; set; }

        public ChapterLast ChapterLast { get; set; }
    }
}

