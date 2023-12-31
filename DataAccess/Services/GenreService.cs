using System;
using AutoMapper;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataAccess.Services.IServices;
using Models;
using Models.Dto;

namespace DataAccess.Services
{
	public class GenreService : IGenreService
	{

        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;

        public GenreService(IGenreRepository genreRepository, IMapper mapper)
		{
            _genreRepository = genreRepository;
            _mapper = mapper;
		}

        public async Task<GenreDto> GetBooksByGenreId(short id, int page, int pageSize)
        {
            return await _genreRepository.GetBooksByGenreId(id, page, pageSize);
        }

        public async Task<IEnumerable<Genre>> GetGenres()
        {
            return await _genreRepository.GetAllAsync();
        }

        public Task AddGenre(GenreCreateDto GenreCreateDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateGenre(int id, GenreUpdateDto authorUpdateDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteGenre(int id)
        {
            throw new NotImplementedException();
        }

    }
}

