using DataAccess.Data;
using DataAccess.Repository.IRepository;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace DataAccess.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> GetByIdAsync(short id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<T> Include(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        // lấy tất
        public async Task<T> FindSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        // lấy thuộc tính cần chọn
        public async Task<TResult> FindSingleAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            #pragma warning disable CS8603 // Possible null reference return.
            return await _dbSet
                .Where(predicate)
                .Select(selector)
                .FirstOrDefaultAsync();
            #pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public async Task AddOrUpdateRangeAsync(IEnumerable<T> entities)
        {
            await _context.BulkInsertOrUpdateAsync(entities);
        }

        public async Task BulkReadAsync(IEnumerable<T> entities,
                                  Expression<Func<T, object>> searchExpression)
        {
            var data = entities.Select(searchExpression.Compile()).ToList();
            await _context.BulkReadAsync(data);
        }

        public async Task BulkUpdateRangeAsync(IEnumerable<T> entities)
        {
            await _context.BulkUpdateAsync(entities);
        }

        public async Task BulkAddRangeAsync(IEnumerable<T> entities)
        {
            await _context.BulkInsertAsync(entities);
        }

        public async Task BulkDeleteRangeAsync(IEnumerable<T> entities)
        {
            await _context.BulkDeleteAsync(entities);
        }
    }

}

