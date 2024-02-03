using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Models.Dto.Comment;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Models.Dto.Rating;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using DataAccess.Services.IServices;

namespace DataAccess.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public RatingService
        (
            IRatingRepository ratingRepository, 
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            UserManager<ApplicationUser> userManager
        ) 
        {
            _ratingRepository = ratingRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<RatingDto> AddRatingAsync(RatingCreateDto ratingCreate)
        {
            if (ratingCreate == null)
            {
                throw new InvalidOperationException("Is not null");
            }

            // Lấy thông tin user hiện tại từ HttpContextAccessor
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra xem UserId của người dùng đang thao tác có trong token hay không
            if (userId == null)
            {
                throw new InvalidOperationException("Invalid User");
            }

            var ratingCheck = await _ratingRepository.FindSingleAsync(r => r.UserId == userId && r.BookId == ratingCreate.BookId);

            if (ratingCheck != null)
            {
                throw new InvalidOperationException("User rated");
            }

            var rating = _mapper.Map<Rating>(ratingCreate);

            rating.UserId = userId;
            rating.CreatedAt = DateTimeOffset.UtcNow;

            await _ratingRepository.AddAsync(rating);

            var ratingDto = new RatingDto();
            ratingDto.Id = rating.Id;
            ratingDto.CreatedAt = rating.CreatedAt;
            ratingDto.Content = rating.Content;
            ratingDto.BookId = rating.BookId;
            ratingDto.RatingValue = rating.RatingValue;

            var user = await _userManager.FindByIdAsync(userId);
            ratingDto.UserId = user.Id;
            ratingDto.FullName = user.FullName;
            ratingDto.Avatar = user.Avatar;

            return ratingDto;
        }

        public async Task<RatingAvgDto> GetRatingAvgAsync(int bookId)
        {
            return await _ratingRepository.GetRatingAvgAsync(bookId);
        }
    }
}
