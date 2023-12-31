using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
	public class Author
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Tên tác giả không được để trống!")]
		public string Name { get; set; }

		public DateTime? Birthday { get; set; }

		[MinLength(30, ErrorMessage = "Tiểu sử tác giả ít nhất 30 ký tự!")]
		public string? Bio { get; set; }

		public string? ChineseName { get; set; }

		public ICollection<Book>? Books { get; set; }
	}
}

