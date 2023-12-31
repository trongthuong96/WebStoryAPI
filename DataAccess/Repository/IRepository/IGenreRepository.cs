using System;
using Models;
using Models.Dto;

namespace DataAccess.Repository.IRepository
{
	public interface IGenreRepository : IRepository<Genre>
    {
        Task<GenreDto> GetBooksByGenreId(short id, int page, int pageSize);
    }
}

