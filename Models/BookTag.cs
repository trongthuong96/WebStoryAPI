using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	public class BookTag
	{
		public short Id { get; set; }

		public string? TagName { get; set; }

        public string? ChineseTagName { get; set; }

        // Danh sách các BookBookTag của BookTag
        public ICollection<BookBookTag>? BookBookTags { get; set; }

    }
}

