using System;
using Models.Dto;

namespace DataAccess.Services.IServices
{
	public interface IChineseBookService
	{
        Task<ChineseBookDto?> GetChineseBookById(int id);
    }
}

