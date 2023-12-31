using System;
using AutoMapper;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataAccess.Services.IServices;
using Models.Dto;

namespace DataAccess.Services
{
	public class ChineseBookService : IChineseBookService
	{
		private readonly IChineseBookRepository _chineseBookRepository;
		private readonly IMapper _mapper;

		public ChineseBookService(IChineseBookRepository chineseBookRepository, IMapper mapper)
		{
			_chineseBookRepository = chineseBookRepository;
			_mapper = mapper;
		}

        public async Task<ChineseBookDto?> GetChineseBookById(int id)
        {
            var chineseBook = await _chineseBookRepository.GetByIdAsync(id);
			return _mapper.Map<ChineseBookDto>(chineseBook);
        }
	}
}

