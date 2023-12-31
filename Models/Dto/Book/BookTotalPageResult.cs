namespace Models.Dto.Book
{
    public class BookTotalPageResult
    {
        public int TotalPages { get; set; }
        public IEnumerable<Models.Book> Books { get; set; }

    }
}

