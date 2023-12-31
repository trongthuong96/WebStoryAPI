using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Dto
{
	public class BookUpdateDto
	{
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống!")]
        [MinLength(500, ErrorMessage = "Mô tả tối thiểu 500 ký tự.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ảnh không được để trống!")]
        public string CoverImage { get; set; }

        [Required(ErrorMessage = "Tác giả không được để trống!")]
        public string AuthorName { get; set; }
    }
}

