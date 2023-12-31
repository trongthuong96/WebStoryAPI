using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Dto
{
	public class BookCreateDto
	{
        [Required(ErrorMessage = "Tiêu đề không được để trống!")]
        public string Title { get; set; }

        public short status { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống!")]
        [MinLength(500, ErrorMessage = "Mô tả tối thiểu 500 ký tự.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ảnh không được để trống!")]
        public string CoverImage { get; set; }

        [Required(ErrorMessage = "Tác giả không được để trống!")]
        public string AuthorName { get; set; }

        [Required(ErrorMessage = "Thể loại không được để trống!")]
        public ICollection<GenreBookCreateDto> genreBookCreateDto { get; set; }

        public string? UserId { get; set; } = null;
    }
}

