using System;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace DataAccess.Middleware
{
    // Middleware/ViewsCounterMiddleware.cs

    public class ViewsCounterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;

        public ViewsCounterMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context, IBookRepository _bookRepository, IChapterRepository _chapterRepository)
        {
            try
            {
                var ipAddress = context.Connection.RemoteIpAddress.ToString();
                var bookId = context.Request.RouteValues["bookId"]?.ToString();
                var bookSlug = context.Request.RouteValues["slug"]?.ToString();
                var chapterId = context.Request.RouteValues["chapterId"]?.ToString(); // Sử dụng cùng một tham số "id" cho cả sách và chương
                var bookSlugChap = context.Request.RouteValues["bookSlug"]?.ToString();
                var chapterIndex = context.Request.RouteValues["chapterIndex"]?.ToString();



                if (!string.IsNullOrEmpty(bookId))
                {
                    var bookCacheKey = $"Views:Book:{bookId}:{ipAddress}";

                    var bookViews = await _cache.GetStringAsync(bookCacheKey);

                    if (bookViews == null)
                    {
                        // Tăng views của truyện trong cache và set thời gian hết hạn (ví dụ: 30 phút)
                        await _cache.SetStringAsync(bookCacheKey, "1", new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                        });

                        // Cập nhật total views của truyện trong cơ sở dữ liệu
                        await _bookRepository.IncreaseBookViewsAsync(int.Parse(bookId), null);
                    }
                }
                if (!string.IsNullOrEmpty(bookSlug))
                {
                    var bookCacheKey = $"Views:Book:{bookSlug}:{ipAddress}";

                    var bookViews = await _cache.GetStringAsync(bookCacheKey);

                    if (bookViews == null)
                    {
                        // Tăng views của truyện trong cache và set thời gian hết hạn (ví dụ: 30 phút)
                        await _cache.SetStringAsync(bookCacheKey, "1", new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                        });

                        // Cập nhật total views của truyện trong cơ sở dữ liệu
                        await _bookRepository.IncreaseBookViewsAsync(0, bookSlug);
                    }
                }

                if (!string.IsNullOrEmpty(chapterId))
                {
                    var chapterCacheKey = $"Views:Chapter:{chapterId}:{ipAddress}";

                    var chapterViews = await _cache.GetStringAsync(chapterCacheKey);

                    if (chapterViews == null)
                    {
                        // Tăng views của chương trong cache và set thời gian hết hạn (ví dụ: 30 phút)
                        await _cache.SetStringAsync(chapterCacheKey, "1", new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                        });

                        // Cập nhật total views của chương trong cơ sở dữ liệu
                        await _chapterRepository.IncreaseChapterViewsAsync(long.Parse(chapterId));
                    }
                }

                if (!string.IsNullOrEmpty(bookSlugChap) && !string.IsNullOrEmpty(chapterIndex))
                {
                    var chapterCacheKey = $"Views:Chapter:{bookSlugChap}:{chapterIndex}:{ipAddress}";

                    var chapterViews = await _cache.GetStringAsync(chapterCacheKey);

                    if (chapterViews == null)
                    {
                        // Tăng views của chương trong cache và set thời gian hết hạn (ví dụ: 30 phút)
                        await _cache.SetStringAsync(chapterCacheKey, "1", new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                        });

                        // Cập nhật total views của chương trong cơ sở dữ liệu
                        await _chapterRepository.IncreaseChapterViewsAsync( bookSlug, short.Parse(chapterIndex));
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Book not found: " + ex.Message);
                return;
            }

            await _next(context);
        }
    }
}

