using System.Linq.Expressions;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Repository
{
    public class BookReadingRepository : BaseRepository<BookReading>, IBookReadingRepository
    {
        public BookReadingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public new async Task<IEnumerable<BookReading>> FindAsync(Expression<Func<BookReading, bool>> predicate)
        {
            return await _dbSet
                .Where(predicate)
                .OrderByDescending(br => br.UpdatedAt)  // Sắp xếp theo thuộc tính UpdatedAt
                .ToListAsync();
        }

    }
}

