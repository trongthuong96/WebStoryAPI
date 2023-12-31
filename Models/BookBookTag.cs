using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	public class BookBookTag
	{
        [ForeignKey("Book")]
        public int BookId { get; set; }

		public Book Book { get; set; }

        [ForeignKey("BookTag")]
        public short BookTagId { get; set; }

        public BookTag BookTag { get; set; }
    }
}

