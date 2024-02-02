using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	public class Notification
	{
		public long Id { get; set; }

		[Required(ErrorMessage = "Thông báo không được để trống!")]
		[MinLength(5, ErrorMessage = "Thông báo ít nhất có 5 ký tự!")]
		public string Content { get; set; }

		[Required(ErrorMessage = "Kiểm tra đã đọc không được để trống!")]
		public bool IsRead { get; set; }

		[Required(ErrorMessage = "Thời gian không được để trống!")]
		public DateTimeOffset CreatedAt { get; set; }

        [ForeignKey("Book")]
        public string UserId { get; set; }

		public ApplicationUser ApplicationUser { get; set; }
	}
}

