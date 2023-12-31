using Models;
using Models.Dto;
using Models.Dto.Book;
using Models.Dto.Crawling;

namespace DataAccess.Services.IServices
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetBooks();
        Task<IEnumerable<BooksDto?>> GetBooksOrderByUpdatedAtAsync(int page, int pageSize);
        Task<IEnumerable<BooksDto?>> GetBooksOrderByViewsAtAsync(int page, int pageSize);
        Task<BookTotalPageResultDto?> GetBooksByTitleAsync(string title, int page, int pageSize);
        Task<BookTotalPageResultDto?> GetBooksSearchAllAsync(string keyword, int[] status, short genre, short chapLength, int page, int pageSize);
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDto?> GetBookByTitleSlugAsync(string titleSlug);
        Task<IEnumerable<BooksDto?>> GetBooksStatusCompleteAsync(int page, int pageSize);
        Task<IEnumerable<BooksDto?>> GetBooksAuthorAsync(int authorId, int page, int pageSize);
        Task<IEnumerable<BooksDto?>> GetBooksUserAsync(string userId, int page, int pageSize);
        Task AddBook(BookCreateDto bookCreateDto);
        Task UpdateBook(int id, BookUpdateDto bookUpdateDto);
        Task DeleteBook(int id);
        Task<(int, int, string)> AddAndUpdateBookChinese(BookCrawlDto bookCrawl);
    }
}

