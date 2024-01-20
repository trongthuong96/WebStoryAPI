using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class BookReading
	{
        public int  Id { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

        public Book Book { get; set; }

        [ForeignKey("ChineseBook")]
        public int? ChineseBookId { get; set; }

        public ChineseBook? ChineseBook { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public short ChapNumber { get; set; }

        public string ChapTitle { get; set; }

        public short ChapterIndex { get; set; }

        public string BookTitle { get; set; }

        public string BookSlug { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

