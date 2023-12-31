using System;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class GenreBookRepository : BaseRepository<GenreBook>, IGenreBookRepository
    {
        public GenreBookRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

