using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Dto.Comment;
using Models.Dto.Rating;
using System.Drawing.Printing;

namespace DataAccess.Repository
{
    public class RatingRepository : BaseRepository<Rating>, IRatingRepository
    {
        public RatingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RatingAvgDto> GetRatingAvgAsync(int bookId)
        {
            var ratingSum = await _context.Ratings
                .Where(r => r.BookId == bookId)
                .SumAsync(r => r.RatingValue);

            var ratingCount = await _context.Ratings
                .Where(r => r.BookId == bookId)
                .CountAsync();

            var ratingAvg = ratingCount > 0
                ? (float)Math.Round((float)ratingSum / ratingCount, 1)
                : 0;

            var ratingResponse = new RatingAvgDto
            {
                RatingValueAvg = ratingAvg,
                Count = ratingCount
            };

            return ratingResponse;
        }

    }
}
