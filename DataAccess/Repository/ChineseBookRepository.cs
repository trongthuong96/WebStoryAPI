using System;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class ChineseBookRepository : BaseRepository<ChineseBook>, IChineseBookRepository
    {
        public ChineseBookRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

