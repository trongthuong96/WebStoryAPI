using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Dto.Chapter;

namespace Models
{
	public class Book
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Tiêu đề không được để trống!")]
		public string Title { get; set; }

        [Required(ErrorMessage = "Slug không được để trống!")]
        public string Slug { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống!")]
		[MinLength(50, ErrorMessage = "Mô tả tối thiểu 50 ký tự.")]
		public string Description { get; set; }

		[Required(ErrorMessage = "Ảnh không được để trống!")]
		public string CoverImage { get; set; }

		[Required(ErrorMessage = "Thời gian tạo không được để trống!")]
		public DateTimeOffset CreatedAt { get; set; }

		[Required(ErrorMessage = "Thời gian cập nhập không được để trống!")]
		public DateTimeOffset UpdatedAt { get; set; }

        [Required(ErrorMessage = "Trạng thái truyện không được để trống!")]
        public short Status { get; set; }

        public long Views { get; set; }

        [Required(ErrorMessage = "Người dùng không được để trống!")]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
      
		public ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "Tác giả không được để trống!")]
        [ForeignKey("Author")]
        public int AuthorId { get; set; }

		public Author Author { get; set; }

		public ICollection<Chapter>? Chapters { get; set; }

        public ICollection<Rating>? Ratings { get; set; }

        // Danh sách các comment của cuốn sách
        public ICollection<Comment>? Comments { get; set; }    

        // Danh sách các BookTag của cuốn sách
        public ICollection<BookBookTag>? BookBookTags { get; set; }       

        // Danh sách các Genre của cuốn sách
        public ICollection<GenreBook>? GenreBooks { get; set; }

        // Danh sách các Bookmark của cuốn sách
        public ICollection<UserBookmark>? UserBookmarks { get; set; }       

        [NotMapped]
        public ICollection<Genre>? Genres { get; set; }

        [NotMapped]
        public ChapterLast ChapterLast { get; set; }

        public ICollection<ChineseBook>? ChineseBooks { get; set; }
    }
}

