using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	public class Genre
	{
		public short Id { get; set; }

		[Required(ErrorMessage = "Tên thể loại không được để trống!")]
		public string Name { get; set; }

		[MinLength(30, ErrorMessage = "Mô tả ít nhất 30 ký tự!")]
		public string? Description { get; set; }

		public string? ChineseName { get; set; }

		// Danh sách các Book của Genre
		public ICollection<GenreBook>? GenreBooks { get; set; }
    }
}

