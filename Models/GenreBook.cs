using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models
{
	public class GenreBook
    {
        [ForeignKey("Book")]
		public int BookId { get; set; }

		public Book Book { get; set; }

        [ForeignKey("Genre")]
        public short GenreId { get; set; }

		public Genre Genre { get; set; }
	}
}

