using System;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Dto;
using Models.Dto.Chapter;

namespace DataAccess.Repository
{
    public class GenreRepository : BaseRepository<Genre>, IGenreRepository
    {
        public GenreRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<GenreDto> GetBooksByGenreId(short id, int page, int pageSize)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            var genre = await _context.Genres
                .Select(c => new GenreDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    BookInGenreDtos = c.GenreBooks
                        .Select(bg => new BookInGenreDto
                        {
                            Id = bg.BookId,
                            Title = bg.Book.Title,
                            Slug = bg.Book.Slug,
                            Status = bg.Book.Status,
                            AuthorName = bg.Book.Author.Name,
                            Description = bg.Book.Description,
                            CoverImage = bg.Book.CoverImage,
                            UpdatedAt = bg.Book.UpdatedAt,
                            Views = bg.Book.Views,
                            ChapterLast = bg.Book.Chapters
                                        .OrderByDescending(chap => chap.ChapterIndex)
                                        .Select(chap => new ChapterLast { Id = chap.Id, ChapterIndex = chap.ChapterIndex, ChapNumber = chap.ChapNumber, Title = chap.Title})
                                        .FirstOrDefault()
                        })
                         .OrderByDescending(b => b.UpdatedAt)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList()

                })
                .FirstOrDefaultAsync(c => c.Id == id);
#pragma warning restore CS8601 // Possible null reference assignment.

            if (genre == null)
            {
                // Xử lý khi không tìm thấy sách
                return null; // hoặc throw exception tùy vào yêu cầu của bạn
            }

            // Calculate total pages based on the total number of items and page size
            int totalItems = await _context.GenreBooks.CountAsync(bg => bg.GenreId == id);
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Add total pages information to the GenreDto
            genre.TotalPages = totalPages;

            return genre!;
        }
    }
}

