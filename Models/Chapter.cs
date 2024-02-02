using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	public class Chapter
	{
		public long Id { get; set; }

		[Required(ErrorMessage = "Số chương không được để trống!")]
		public short ChapNumber { get; set; }

        [Required(ErrorMessage = "Chỉ mục chương  không được để trống!")]
        public short ChapterIndex { get; set; }

        public string? Title { get; set; }

		public string? Content { get; set; }

		[Required(ErrorMessage = "Thời gian tạo không được để trống!")]
		public DateTimeOffset CreatedAt { get; set; }

		[Required(ErrorMessage = "Thời gian cập nhập không được để trống!")]
		public DateTimeOffset UpdatedAt { get; set; }

		public long Views { get; set; }

        public string? ChineseTitle { get; set; }

        public string? ChineseContent { get; set; }

        public string? ChineseSite { get; set; }

        [ForeignKey("ChineseBook")]
        public int? ChineseBookId { get; set; }

        public ChineseBook? ChineseBook { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

		public Book Book { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

		public ApplicationUser ApplicationUser { get; set; }

        public ICollection<UserBookmark>? UserBookmarks { get; set; }

        public Chapter()
        {
            Book = new Book();
            ChineseBook = new ChineseBook();
            ApplicationUser = new ApplicationUser();
        }
    }
}

