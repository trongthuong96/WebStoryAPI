using Models;
using Models.Dto.Rating;

namespace DataAccess.Repository.IRepository
{
    public interface IRatingRepository : IRepository<Rating>
    {
        Task<RatingAvgDto> GetRatingAvgAsync(int bookId);
    }
}
