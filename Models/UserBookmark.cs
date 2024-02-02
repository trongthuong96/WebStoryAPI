using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	public class UserBookmark
	{
		[Required(ErrorMessage = "Thời gian tạo không được để trống!")]
		public DateTimeOffset CreatedAt { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

		public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

		public Book Book { get; set; }

        [ForeignKey("Chapter")]
        public long? ChapterId { get; set; }

		public Chapter? Chapter { get; set; }
	}
}

