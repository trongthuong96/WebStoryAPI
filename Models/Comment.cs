using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	public class Comment
	{
		public long Id { get; set; }

		[Required(ErrorMessage = "Nội dung không được để trống!")]
		[MinLength(3, ErrorMessage = "Nội dung ít nhất 3 ký tự!")]
		public string Content { get; set; }

		[Required(ErrorMessage = "Ngày tạo không được để trống!")]
		public DateTime CreatedAt { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

		public Book Book { get; set; }

        [ForeignKey("ApplicationUser")]
		public string UserId { get; set; }

		public ApplicationUser ApplicationUser { get; set; }
	}
}

