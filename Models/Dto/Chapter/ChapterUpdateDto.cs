using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Dto
{
	public class ChapterUpdateDto
	{
        public long Id { get; set; }

        [Required(ErrorMessage = "Số chương không được để trống!")]
        public short ChapNumber { get; set; }

        public string? Title { get; set; }

        [Required(ErrorMessage = "Nội dung chương không được để trống!")]
        [MinLength(50, ErrorMessage = "Nội dung chương ít nhất 50 ký tự!")]
        public string Content { get; set; }

        public int BookId { get; set; }
    }
}

