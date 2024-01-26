using System;
using System.Security.Claims;
using AutoMapper;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Http;
using Models.Dto.Book;
using Models.Dto.BookReading;

namespace DataAccess.Services
{
	public class BookReadingService : IBookReadingService
	{
		private readonly IBookReadingRepository _bookReadingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public BookReadingService(
                IBookReadingRepository bookReadingRepository,
                IHttpContextAccessor httpContextAccessor,
                IMapper mapper
            )
		{
			_bookReadingRepository = bookReadingRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
		}

        public async Task<IEnumerable<BookReadingDto>?> GetBookReadingsByUserIdAsync()
        {
            // Lấy thông tin user hiện tại từ HttpContextAccessor
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return null;
            }

            var books = await _bookReadingRepository.FindAsync(br => br.UserId == userId);

            return _mapper.Map<IEnumerable<BookReadingDto>>(books);
        }

        public async Task<int> Delete(int bookId, int chineseBookId)
        {
            // Lấy thông tin user hiện tại từ HttpContextAccessor
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return 400;
            }

            var books = await _bookReadingRepository.FindSingleAsync(br => br.UserId == userId && br.BookId == bookId && br.ChineseBookId == chineseBookId);

            if (books == null)
            {
                return 404;
            }

            await _bookReadingRepository.DeleteAsync(books);

            return 200;
        }
    }
}

