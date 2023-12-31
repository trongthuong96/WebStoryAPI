using System;
using System.Security.Claims;
using AutoMapper;
using DataAccess.Repository.IRepository;
using DataAccess.Services.IServices;
using Models;
using Models.Dto;

namespace DataAccess.Services
{
	public class AuthorService : IAuthorService
	{

        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public AuthorService(IAuthorRepository authorRepository, IMapper mapper)
		{
            _authorRepository = authorRepository;
            _mapper = mapper;
		}

        public async Task AddAuthor(AuthorCreateDto authorCreateDto)
        {
            try
            {
                // Kiểm tra xem sách có tồn tại không
                if (await _authorRepository.AnyAsync(b => b.Name == authorCreateDto.Name))
                {
                    throw new InvalidOperationException("Author with the same title already exists.");
                }
              

                // Chuyển đổi từ BookCreateDto sang Book sử dụng AutoMapper
                var author = _mapper.Map<Author>(authorCreateDto);

                // Thêm sách vào repository
                await _authorRepository.AddAsync(author);
            }
            catch (Exception)
            {
                // Log lỗi hoặc xử lý lỗi theo ý bạn
                throw; // Ném lại lỗi để được xử lý ở tầng Controller hoặc nơi sử dụng phương thức này
            }
        }

        public Task DeleteAuthor(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthorDto> GetAuthorById(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            var authorDto = _mapper.Map<AuthorDto>(author);
            return authorDto;
        }

        public Task<IEnumerable<AuthorDto>> GetAuthors()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAuthor(int id, AuthorUpdateDto authorUpdateDto)
        {
            throw new NotImplementedException();
        }
    }
}

