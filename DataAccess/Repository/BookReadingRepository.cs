using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class BookReadingRepository : BaseRepository<BookReading>, IBookReadingRepository
    {
        public BookReadingRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

