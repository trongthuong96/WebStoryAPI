using Models;
using Models.Dto;

namespace DataAccess.Services.IServices
{
	public interface IAuthorService
	{
        Task<IEnumerable<AuthorDto>> GetAuthors();
        Task<AuthorDto> GetAuthorById(int id);
        Task AddAuthor(AuthorCreateDto AuthorCreateDto);
        Task UpdateAuthor(int id, AuthorUpdateDto authorUpdateDto);
        Task DeleteAuthor(int id);
    }
}

