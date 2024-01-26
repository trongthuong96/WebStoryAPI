using System;
using Models.Dto.BookReading;

namespace DataAccess.Services.IServices
{
	public interface IBookReadingService
	{
        Task<IEnumerable<BookReadingDto>?> GetBookReadingsByUserIdAsync();
        Task<int> Delete(int bookId, int chineseBookId);
    }
}

