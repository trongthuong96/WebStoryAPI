using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	public class Rating
	{
		public long Id { get; set; }

		[Required(ErrorMessage = "Điểm đánh giá không được trống!")]
		public short RatingValue { get; set; }

		public string? Content { get; set; }

		[Required(ErrorMessage = "Thời gian tạo không được để trống!")]
		public DateTimeOffset CreatedAt { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

		public ApplicationUser ApplicationUser { get; set; }

		[ForeignKey("Book")]
        public int BookId { get; set; }

		public Book Book { get; set; }
	}
}

