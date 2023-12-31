using System;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class BookBookTagRepository : BaseRepository<BookBookTag>, IBookBookTagRepository
    {
        public BookBookTagRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

