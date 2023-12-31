using System;
using Models;
using Models.Dto;

namespace DataAccess.Services.IServices
{
	public interface IGenreService
	{
        Task<IEnumerable<Genre>> GetGenres();
        Task<GenreDto> GetBooksByGenreId(short id, int page, int pageSize);
        Task AddGenre(GenreCreateDto GenreCreateDto);
        Task UpdateGenre(int id, GenreUpdateDto authorUpdateDto);
        Task DeleteGenre(int id);
    }
}

