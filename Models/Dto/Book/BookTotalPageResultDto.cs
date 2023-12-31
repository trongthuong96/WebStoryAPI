namespace Models.Dto.Book
{
	public class BookTotalPageResultDto
	{
        public int TotalPages { get; set; }
        public IEnumerable<BooksDto> Books { get; set; }
        
    }
}

