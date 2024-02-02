using Models.Dto.Comment;
using System.ComponentModel.DataAnnotations;

namespace Models.Dto
{
	public class BookDto
	{
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Slug không được để trống!")]
        public string Slug { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống!")]
        [MinLength(500, ErrorMessage = "Mô tả tối thiểu 500 ký tự.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ảnh không được để trống!")]
        public string CoverImage { get; set; }

        [Required(ErrorMessage = "Thời gian tạo không được để trống!")]
        public DateTimeOffset CreatedAt { get; set; }

        [Required(ErrorMessage = "Thời gian cập nhập không được để trống!")]
        public DateTimeOffset UpdatedAt { get; set; }

        public short Status { get; set; }

        public long Views { get; set; }

        public ApplicationUserDto ApplicationUser { get; set; }

        public AuthorDto Author { get; set; }

        public ICollection<Rating>? Ratings { get; set; }

        // Danh sách các comment của cuốn sách
        public ICollection<CommentDto>? Comments { get; set; }

        public ICollection<BookBookTag>? BookBookTags { get; set; }

        public ICollection<GenreDto>? Genres { get; set; }

        public ICollection<ChineseBookDto>? ChineseBooks { get; set; }
    }
}

