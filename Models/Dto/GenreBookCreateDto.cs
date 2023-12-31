using System;
namespace Models.Dto
{
	public class GenreBookCreateDto
	{
		public short GenreId { get; set; }

		public GenreBookCreateDto()
		{

		}

		public GenreBookCreateDto(short genreId)
		{
			this.GenreId = genreId;
		}
	}
}