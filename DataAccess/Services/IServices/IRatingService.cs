using Models.Dto.Rating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.IServices
{
    public interface IRatingService
    {
        Task<RatingDto> AddRatingAsync(RatingCreateDto ratingCreate);
        Task<RatingAvgDto> GetRatingAvgAsync(int bookId);
    }
}
