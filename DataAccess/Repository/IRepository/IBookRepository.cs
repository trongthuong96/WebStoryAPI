using Models;
using Models.Dto;
using Models.Dto.Book;

namespace DataAccess.Repository.IRepository
{
    public interface IBookRepository : IRepository<Book>
    {
        // Các phương thức đặc biệt cho BookRepository, nếu cần
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDto?> GetBookByTitleSlugAsync(string titleSlug);
        Task<Book?> GetBookBySlugAsync(string titleSlug);
        Task<IEnumerable<BooksDto?>> GetBooksOrderByUpdatedAtAsync(int page, int pageSize);
        Task<IEnumerable<BooksDto?>> GetBooksOrderByViewsAtAsync(int page, int pageSize);
        Task<BookTotalPageResultDto?> GetBooksByTitleAsync(string title, int page, int pageSize);
        Task<BookTotalPageResultDto?> GetBooksSearchAllAsync(string keyword, int[] status, short genre, short chapLength, int page, int pageSize);
        Task<IEnumerable<BooksDto?>> GetBooksStatusCompleteAsync(int page, int pageSize);
        Task<IEnumerable<BooksDto?>> GetBooksAuthorAsync(int authorId, int page, int pageSize);
        Task<IEnumerable<BooksDto?>> GetBooksUserAsync(string userId, int page, int pageSize);
        Task IncreaseBookViewsAsync(int bookId, string slug);
    }

}

