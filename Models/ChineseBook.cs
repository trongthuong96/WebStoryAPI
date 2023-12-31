using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	public class ChineseBook
	{
        public int Id { get; set; }

        public string? ChineseTitle { get; set; }

        public string? ChineseDescription { get; set; }

        public string? ChineseSite { get; set; }

        public string? ChineseSiteName { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

        public Book Book { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        public Author Author { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}

