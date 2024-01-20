using System.Linq.Expressions;
using Models;

namespace DataAccess.Repository.IRepository
{
    public interface IBookReadingRepository : IRepository<BookReading>
	{
        Task<IEnumerable<BookReading>> FindAsync(Expression<Func<BookReading, bool>> predicate);

    }
}

