// Repositories/BookRepository.cs
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Dto;
using Models.Dto.Book;
using Models.Dto.Chapter;

namespace DataAccess.Repository
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        // private readonly string sqlConnectionString = "Server=sql.bsite.net\\MSSQL2016; TrustServerCertificate=True; MultiSubnetFailover=True;Initial Catalog=truyenhay_; User Id=truyenhay_; Password=Thuong@123;";

        // Các phương thức đặc biệt cho BookRepository, nếu cần
        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var bookWithDetails = await _context.Books
                .Where(b => b.Id == id)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Status = b.Status,
                    Description = b.Description,
                    ApplicationUser = new ApplicationUserDto
                    {
                        Id = b.UserId,
                        UserName = b.ApplicationUser.UserName,
                        FullName = b.ApplicationUser.Fullname,
                    },
                    CoverImage = b.CoverImage,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    Views = b.Views,
                    Author = new AuthorDto(b.Author.Id, b.Author.Name),
                    Ratings = b.Ratings,
                    Genres = b.GenreBooks.Select(bg => new GenreDto
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name,
                        Description = bg.Genre.Description
                    }).ToList(),
                    ChineseBooks = b.ChineseBooks.Select(cb => new ChineseBookDto
                    {
                        Id = cb.Id,
                        ChineseTitle = cb.ChineseTitle,
                        ChineseSite = cb.ChineseSite,
                        ChineseSiteName = cb.ChineseSiteName
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (bookWithDetails == null)
            {
                // Xử lý khi không tìm thấy sách
                return null; // hoặc throw exception tùy vào yêu cầu của bạn
            }

            return bookWithDetails!;
        }

        // return BookDto
        public async Task<BookDto?> GetBookByTitleSlugAsync(string titleSlug)
        {
            var bookWithDetails = await _context.Books
                .Where(b => b.Slug == titleSlug)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Status = b.Status,
                    Description = b.Description,
                    ApplicationUser = new ApplicationUserDto
                    {
                        Id = b.UserId,
                        UserName = b.ApplicationUser.UserName,
                        FullName = b.ApplicationUser.Fullname,
                    },
                    CoverImage = b.CoverImage,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    Views = b.Views,
                    Author = new AuthorDto(b.Author.Id, b.Author.Name),
                    Ratings = b.Ratings,
                    Genres = b.GenreBooks.Select(bg => new GenreDto
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name,
                        Description = bg.Genre.Description
                    }).ToList(),
                    ChineseBooks = b.ChineseBooks.Select(cb => new ChineseBookDto
                    {
                        Id = cb.Id,
                        ChineseTitle = cb.ChineseTitle,
                        ChineseSite = cb.ChineseSite,
                        ChineseSiteName = cb.ChineseSiteName
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (bookWithDetails == null)
            {
                // Xử lý khi không tìm thấy sách
                return null; // hoặc throw exception tùy vào yêu cầu của bạn
            }

            return bookWithDetails!;
        }

        //public async Task<Book?> GetBookByTitleSlugAsync(string titleSlug)
        //{
        //    using (var connection = new SqlConnection(sqlConnectionString))
        //    {
        //        string query = @"
        //        SELECT 
        //            b.Id, b.Title, b.Status, b.Description, b.CoverImage, b.CreatedAt, b.UpdatedAt, b.Views,
        //            u.Id, u.UserName, u.FullName,
        //            a.Id, a.Name,
        //            g.Id, g.Name, g.Description
        //        FROM 
        //            Books b
        //            INNER JOIN AspNetUsers u ON b.UserId = u.Id
        //            INNER JOIN Authors a ON b.AuthorId = a.Id
        //            INNER JOIN GenreBooks gb ON gb.BookId = b.Id
        //            INNER JOIN Genres g ON g.Id = gb.GenreId
        //        WHERE 
        //            b.Slug = @titleSlug
        //    ";

        //        var bookRead = new Book();
        //        bookRead.Genres = new List<Genre>();
        //        var genreDict = new Dictionary<int, Genre>();
        //        var bookDict = new Dictionary<int, Book>();

        //        // Sử dụng dynamic để đọc dữ liệu từ nhiều bảng
        //        var result = await connection.QueryAsync<dynamic, dynamic, dynamic, dynamic, dynamic>(
        //            query,
        //            (book, user, author, genre) => // Giả sử chỉ có 4 đối tượng dynamic (book, user, author, chapter)
        //            {
        //            // Mapping logic ở đây

        //            // Trả về một object có chứa tất cả thông tin cần thiết
        //                Book bookEntry;
        //                if (!bookDict.TryGetValue(book.Id, out bookEntry))
        //                {
        //                    bookRead.Id = book.Id;
        //                    bookRead.Title = book.Title;
        //                    bookRead.Slug = book.Slug;
        //                    bookRead.Status = book.Status;
        //                    bookRead.Description = book.Description;
        //                    bookRead.CoverImage = book.CoverImage;
        //                    bookRead.CreatedAt = book.CreatedAt;
        //                    bookRead.UpdatedAt = book.UpdatedAt;
        //                    bookRead.Views = book.Views;
        //                    // ApplicationUser và Author không có thuộc tính UserId và AuthorId, vì vậy bạn cần thay đổi tùy thuộc vào thiết kế của bạn
        //                    bookRead.ApplicationUser = new ApplicationUser { Id = user.Id, UserName = user.UserName, Fullname = user.FullName };
        //                    bookRead.Author = new Author { Id = author.Id, Name = author.Name };
        //                }

        //                // Kiểm tra xem genre đã tồn tại chưa
        //                Genre existingGenre;
        //                if (!genreDict.TryGetValue(genre.Id, out existingGenre))
        //                {
        //                    // Nếu không tồn tại, thêm vào danh sách và dict
        //                    existingGenre = new Genre
        //                    {
        //                        Id = genre.Id,
        //                        Name = genre.Name,
        //                        Description = genre.Description
        //                    };
        //                    genreDict.Add(genre.Id, existingGenre);
        //                    bookRead.Genres.Add(existingGenre);
        //                }

        //                // Chapter là một IEnumerable<Chapter>, đảm bảo rằng nó đã được khởi tạo
        //                if (bookRead.Chapters == null)
        //                {
        //                    bookRead.Chapters = new List<Chapter>();
        //                }
        //                return bookRead;
        //            },
        //            new { titleSlug },
        //            splitOn: "Id, Id, Id, Id"
        //        );

        //        var book = result.FirstOrDefault();

        //        // TODO: Thực hiện mapping logic ở đây

        //        return book;
        //    }
        //}

        // find book author
        // Orderby updatedAt
        public async Task<IEnumerable<BooksDto?>> GetBooksAuthorAsync(int authorId, int page, int pageSize)
        {
            var books = await _context.Books
                .Where(b => b.AuthorId == authorId)
                .Select(b => new BooksDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Status = b.Status,
                    Description = b.Description,
                    ApplicationUserUserName = b.ApplicationUser.UserName,
                    CoverImage = b.CoverImage,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    Views = b.Views,
                    AuthorName = b.Author.Name,
                    Genres = b.GenreBooks.Select(bg => new GenreDto
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name,
                        Description = bg.Genre.Description
                    }).ToList()
                })                .OrderByDescending(b => b.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Thêm phần này để chuyển kết quả thành một danh sách

            return books;
        }


        //public async Task<IEnumerable<Book?>> GetBooksAuthorAsync(int authorId, int page, int pageSize)
        //{
        //    using (var connection = new SqlConnection(sqlConnectionString))
        //    {
        //        string query = @"
        //           SELECT  b.Id, b.Title, b.Slug, b.Status, b.Description, b.CoverImage, b.CreatedAt, b.UpdatedAt, b.Views,
        //                JSON_QUERY((
        //                    SELECT g.Id, g.Name, g.Description
        //                    FROM GenreBooks gb 
        //                    JOIN Genres g ON g.Id = gb.GenreId
        //                    WHERE gb.BookId = b.Id
        //                    FOR JSON PATH
        //                )) AS Genres
        //            FROM Books b
        //            WHERE b.AuthorId = @authorId
        //            ORDER BY b.UpdatedAt DESC
        //            OFFSET @PageSize * (@Page - 1) ROWS
        //            FETCH NEXT @PageSize ROWS ONLY
        //        ";

        //        var bookDict = new Dictionary<int, Book>();
        //        var genreDict = new Dictionary<string, Genre>();

        //        var result = await connection.QueryAsync<Book, string, Book>(
        //            query,
        //            (book, genresJson) =>
        //            {
        //                Book bookEntry;
        //                if (!bookDict.TryGetValue(book.Id, out bookEntry))
        //                {
        //                    // Tạo bản sao của book và thêm vào bookDict
        //                    bookEntry = new Book
        //                    {
        //                        Id = book.Id,
        //                        Title = book.Title,
        //                        Slug = book.Slug,
        //                        Status = book.Status,
        //                        Description = book.Description,
        //                        CoverImage = book.CoverImage,
        //                        CreatedAt = book.CreatedAt,
        //                        UpdatedAt = book.UpdatedAt,
        //                        Views = book.Views,
        //                        AuthorId = authorId
        //                    };

        //                    bookEntry.Genres = JsonConvert.DeserializeObject<List<Genre>>(genresJson);

        //                    bookDict.Add(book.Id, bookEntry);
        //                }

        //                return bookEntry;
        //            },
        //            new { AuthorId = authorId, PageSize = pageSize, Page = page },
        //            splitOn: "Id, Genres"
        //        );


        //        return result;
        //    }
        //}

        // find book user
        public async Task<IEnumerable<BooksDto?>> GetBooksUserAsync(string userId, int page, int pageSize)
        {
            var books = await _context.Books
                .Where(b => b.UserId == userId)
                .Select(b => new BooksDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Status = b.Status,
                    Description = b.Description,
                    ApplicationUserUserName = b.ApplicationUser.UserName,
                    CoverImage = b.CoverImage,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    Views = b.Views,
                    AuthorName = b.Author.Name,
                    Genres = b.GenreBooks.Select(bg => new GenreDto
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name,
                        Description = bg.Genre.Description
                    }).ToList()
                })
                .OrderByDescending(b => b.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Thêm phần này để chuyển kết quả thành một danh sách

            return books;
        }

        //public async Task<IEnumerable<Book?>> GetBooksUserAsync(string userId, int page, int pageSize)
        //{
        //    using (var connection = new SqlConnection(sqlConnectionString))
        //    {
        //        string query = @"
        //           SELECT  b.Id, b.Title, b.Slug, b.Status, b.Description, b.CoverImage, b.CreatedAt, b.UpdatedAt, b.Views,
        //                JSON_QUERY((
        //                    SELECT g.Id, g.Name, g.Description
        //                    FROM GenreBooks gb 
        //                    JOIN Genres g ON g.Id = gb.GenreId
        //                    WHERE gb.BookId = b.Id
        //                    FOR JSON PATH
        //                )) AS Genres
        //            FROM Books b
        //            WHERE b.UserId = @userId
        //            ORDER BY b.UpdatedAt DESC
        //            OFFSET @PageSize * (@Page - 1) ROWS
        //            FETCH NEXT @PageSize ROWS ONLY
        //        ";

        //        var bookDict = new Dictionary<int, Book>();
        //        var genreDict = new Dictionary<string, Genre>();

        //        var result = await connection.QueryAsync<Book, string, Book>(
        //            query,
        //            (book, genresJson) =>
        //            {
        //                Book bookEntry;
        //                if (!bookDict.TryGetValue(book.Id, out bookEntry))
        //                {
        //                    // Tạo bản sao của book và thêm vào bookDict
        //                    bookEntry = new Book
        //                    {
        //                        Id = book.Id,
        //                        Title = book.Title,
        //                        Slug = book.Slug,
        //                        Status = book.Status,
        //                        Description = book.Description,
        //                        CoverImage = book.CoverImage,
        //                        CreatedAt = book.CreatedAt,
        //                        UpdatedAt = book.UpdatedAt,
        //                        Views = book.Views,
        //                        UserId = userId
        //                    };

        //                    bookEntry.Genres = JsonConvert.DeserializeObject<List<Genre>>(genresJson);

        //                    bookDict.Add(book.Id, bookEntry);
        //                }

        //                return bookEntry;
        //            },
        //            new { UserId = userId, PageSize = pageSize, Page = page },
        //            splitOn: "Id, Genres"
        //        );


        //        return result;
        //    }
        //}


        // return book crawling
        public async Task<Book?> GetBookBySlugAsync(string titleSlug)
        {
            var bookWithDetails = await _context.Books
                .FirstOrDefaultAsync(b => b.Slug == titleSlug);

            return bookWithDetails;
        }

        // Orderby updatedAt
        public async Task<IEnumerable<BooksDto?>> GetBooksOrderByUpdatedAtAsync(int page, int pageSize)
        {
            var books = await _context.Books
                .Select(b => new BooksDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Status = b.Status,
                    Description = b.Description,
                    ApplicationUserUserName = b.ApplicationUser.UserName!,
                    CoverImage = b.CoverImage,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    Views = b.Views,
                    AuthorName = b.Author.Name,
                    ChapterLast = b.Chapters!
                                        .OrderByDescending(chap => chap.ChapterIndex)
                                        .Select(chap => new ChapterLast { Id = chap.Id, ChapterIndex = chap.ChapterIndex, ChapNumber = chap.ChapNumber, Title = chap.Title })
                                        .FirstOrDefault()!,
                    Genres = b.GenreBooks!.Select(bg => new GenreDto
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name,
                        Description = bg.Genre.Description
                    }).ToList()
                })
                .OrderByDescending(b => b.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Thêm phần này để chuyển kết quả thành một danh sách

            return books;
        }

        //public async Task<IEnumerable<Book?>> GetBooksOrderByUpdatedAtAsync(int page, int pageSize)
        //{
        //    using (var connection = new SqlConnection(sqlConnectionString))
        //    {
        //        string query = @"
        //           SELECT  b.Id, b.Title, b.Slug, b.Status, b.Description, b.CoverImage, b.CreatedAt, b.UpdatedAt, b.Views,
        //                    u.Id, u.UserName, u.Fullname,
        //                    a.Id, a.Name,
        //                    c.Id, c.ChapterIndex, c.ChapNumber, c.Title,
        //               JSON_QUERY((
        //                 SELECT g.Id, g.Name, g.Description
        //                 FROM GenreBooks gb 
        //                 JOIN Genres g ON g.Id = gb.GenreId
        //                 WHERE gb.BookId = b.Id
        //                 FOR JSON PATH
        //               )) AS Genres
        //            FROM Books b
        //            INNER JOIN AspNetUsers u ON b.UserId = u.Id 
        //            INNER JOIN Authors a ON b.AuthorId = a.Id
        //            LEFT JOIN (
        //                SELECT cc.BookId, MAX(cc.ChapterIndex) AS MaxChapterIndex 
        //                FROM Chapters cc 
        //                GROUP BY cc.BookId  
        //            ) maxChapter ON b.Id = maxChapter.BookId
        //            LEFT JOIN Chapters c ON c.BookId = b.Id AND c.ChapterIndex = maxChapter.MaxChapterIndex
        //            ORDER BY b.UpdatedAt DESC
        //            OFFSET @PageSize * (@Page - 1) ROWS
        //            FETCH NEXT @PageSize ROWS ONLY
        //        ";

        //        var bookDict = new Dictionary<int, Book>();
        //        var genreDict = new Dictionary<string, Genre>();

        //        var result = await connection.QueryAsync<Book, ApplicationUser, Author, Chapter, string, Book>(
        //            query,
        //            (book, user, author, chapter, genresJson) =>
        //            {
        //                Book bookEntry;
        //                if (!bookDict.TryGetValue(book.Id, out bookEntry))
        //                {
        //                    // Tạo bản sao của book và thêm vào bookDict
        //                    bookEntry = new Book
        //                    {
        //                        Id = book.Id,
        //                        Title = book.Title,
        //                        Slug = book.Slug,
        //                        Status = book.Status,
        //                        Description = book.Description,
        //                        CoverImage = book.CoverImage,
        //                        CreatedAt = book.CreatedAt,
        //                        UpdatedAt = book.UpdatedAt,
        //                        Views = book.Views,
        //                        AuthorId = author.Id
        //                    };

        //                    bookEntry.ChapterLast = new ChapterLast
        //                    {
        //                        Id = chapter?.Id ?? 0,
        //                        ChapterIndex = chapter?.ChapterIndex ?? 0,
        //                        ChapNumber = chapter?.ChapNumber ?? 0,
        //                        Title = chapter?.Title
        //                    };

        //                    bookEntry.Genres = JsonConvert.DeserializeObject<List<Genre>>(genresJson);

        //                    bookDict.Add(book.Id, bookEntry);
        //                }

        //                bookEntry.ApplicationUser = new ApplicationUser { Id = user.Id, UserName = user.UserName, Fullname = user.Fullname };
        //                bookEntry.Author = new Author { Id = author.Id, Name = author.Name };

        //                return bookEntry;
        //            },
        //            new { PageSize = pageSize, Page = page },
        //            splitOn: "Id, Id, Id, Id, Genres"
        //        );


        //        return result;
        //    }
        //}



        public async Task<IEnumerable<BooksDto?>> GetBooksOrderByViewsAtAsync(int page, int pageSize)
        {
            var books = await _context.Books
                .Select(b => new BooksDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Status = b.Status,
                    Description = b.Description,
                    ApplicationUserUserName = b.ApplicationUser.UserName!,
                    CoverImage = b.CoverImage,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    Views = b.Views,
                    AuthorName = b.Author.Name,
                    ChapterLast = b.Chapters!
                                        .OrderByDescending(chap => chap.ChapterIndex)
                                        .Select(chap => new ChapterLast { Id = chap.Id, ChapterIndex = chap.ChapterIndex, ChapNumber = chap.ChapNumber, Title = chap.Title })
                                        .FirstOrDefault()!,
                    Genres = b.GenreBooks!.Select(bg => new GenreDto
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name,
                        Description = bg.Genre.Description
                    }).ToList()
                })
                .OrderByDescending(b => b.Views)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Thêm phần này để chuyển kết quả thành một danh sách

            return books;
        }

        //public async Task<IEnumerable<Book?>> GetBooksOrderByViewsAtAsync(int page, int pageSize)
        //{
        //    using (var connection = new SqlConnection(sqlConnectionString))
        //    {
        //        string query = @"
        //           SELECT  b.Id, b.Title, b.Slug, b.Status, b.Description, b.CoverImage, b.CreatedAt, b.UpdatedAt, b.Views,
        //                    u.Id, u.UserName, u.Fullname,
        //                    a.Id, a.Name,
        //                    c.Id, c.ChapterIndex, c.ChapNumber, c.Title,
        //               JSON_QUERY((
        //                 SELECT g.Id, g.Name, g.Description
        //                 FROM GenreBooks gb 
        //                 JOIN Genres g ON g.Id = gb.GenreId
        //                 WHERE gb.BookId = b.Id
        //                 FOR JSON PATH
        //               )) AS Genres
        //            FROM Books b
        //            INNER JOIN AspNetUsers u ON b.UserId = u.Id 
        //            INNER JOIN Authors a ON b.AuthorId = a.Id
        //            LEFT JOIN (
        //                SELECT cc.BookId, MAX(cc.ChapterIndex) AS MaxChapterIndex 
        //                FROM Chapters cc 
        //                GROUP BY cc.BookId  
        //            ) maxChapter ON b.Id = maxChapter.BookId
        //            LEFT JOIN Chapters c ON c.BookId = b.Id AND c.ChapterIndex = maxChapter.MaxChapterIndex
        //            ORDER BY b.Views DESC
        //            OFFSET @PageSize * (@Page - 1) ROWS
        //            FETCH NEXT @PageSize ROWS ONLY
        //        ";

        //        var bookDict = new Dictionary<int, Book>();
        //        var genreDict = new Dictionary<string, Genre>();

        //        var result = await connection.QueryAsync<Book, ApplicationUser, Author, Chapter, string, Book>(
        //            query,
        //            (book, user, author, chapter, genresJson) =>
        //            {
        //                Book bookEntry;
        //                if (!bookDict.TryGetValue(book.Id, out bookEntry))
        //                {
        //                    // Tạo bản sao của book và thêm vào bookDict
        //                    bookEntry = new Book
        //                    {
        //                        Id = book.Id,
        //                        Title = book.Title,
        //                        Slug = book.Slug,
        //                        Status = book.Status,
        //                        Description = book.Description,
        //                        CoverImage = book.CoverImage,
        //                        CreatedAt = book.CreatedAt,
        //                        UpdatedAt = book.UpdatedAt,
        //                        Views = book.Views,
        //                        AuthorId = author.Id
        //                    };

        //                    bookEntry.ChapterLast = new ChapterLast
        //                    {
        //                        Id = chapter?.Id ?? 0,
        //                        ChapterIndex = chapter?.ChapterIndex ?? 0,
        //                        ChapNumber = chapter?.ChapNumber ?? 0,
        //                        Title = chapter?.Title
        //                    };

        //                    bookEntry.Genres = JsonConvert.DeserializeObject<List<Genre>>(genresJson);

        //                    bookDict.Add(book.Id, bookEntry);
        //                }

        //                bookEntry.ApplicationUser = new ApplicationUser { Id = user.Id, UserName = user.UserName, Fullname = user.Fullname };
        //                bookEntry.Author = new Author { Id = author.Id, Name = author.Name };

        //                return bookEntry;
        //            },
        //            new { PageSize = pageSize, Page = page },
        //            splitOn: "Id, Id, Id, Id, Genres"
        //        );
        //        return result;
        //    }
        //}

        public async Task<BookTotalPageResultDto?> GetBooksByTitleAsync(string title, int page, int pageSize)
        {
            var query = _context.Books
                .Where
                (b =>
                    (
                        (title == null ? true : EF.Functions.Collate(b.Title, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(title, "SQL_Latin1_General_CP1_CI_AI")))
                        || (title == null ? true : EF.Functions.Collate(b.Author.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(title, "SQL_Latin1_General_CP1_CI_AI")))
                    )
                )
                .AsQueryable();

            var books = await query
                .Select(b => new BooksDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Status = b.Status,
                    Description = b.Description,
                    ApplicationUserUserName = b.ApplicationUser.UserName!,
                    CoverImage = b.CoverImage,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    Views = b.Views,
                    AuthorName = b.Author.Name,
                    ChapterLast = b.Chapters!
                                        .OrderByDescending(chap => chap.ChapterIndex)
                                        .Select(chap => new ChapterLast { Id = chap.Id, ChapterIndex = chap.ChapterIndex, ChapNumber = chap.ChapNumber, Title = chap.Title })
                                        .FirstOrDefault()!,
                    Genres = b.GenreBooks!.Select(bg => new GenreDto
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name,
                        Description = bg.Genre.Description
                    }).ToList()
                })                .OrderByDescending(b => b.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Lấy tổng số lượng sách dựa trên điều kiện tìm kiếm
            var totalBooks = await query
                .Select(b => new BooksDto
                {
                    Title = b.Title,
                    AuthorName = b.Author.Name
                })
                .CountAsync();

            // Tính toán totalPages
            var totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

            // Đóng gói kết quả
            var result = new BookTotalPageResultDto
            {
                TotalPages = totalPages,
                Books = books
            };

            return result;

        }

        //public async Task<BookTotalPageResult?> GetBooksByTitleAsync(string title, int page, int pageSize)
        //{
        //    using (var connection = new SqlConnection(sqlConnectionString))
        //    {
        //        string query = @"
        //           SELECT  b.Id, b.Title, b.Slug, b.Status, b.Description, b.CoverImage, b.CreatedAt, b.UpdatedAt, b.Views,
        //                    u.Id, u.UserName, u.Fullname,
        //                    a.Id, a.Name,
        //                    c.Id, c.ChapterIndex, c.ChapNumber, c.Title,
        //               JSON_QUERY((
        //                 SELECT g.Id, g.Name, g.Description
        //                 FROM GenreBooks gb 
        //                 JOIN Genres g ON g.Id = gb.GenreId
        //                 WHERE gb.BookId = b.Id
        //                 FOR JSON PATH
        //               )) AS Genres
        //            FROM Books b
        //            INNER JOIN AspNetUsers u ON b.UserId = u.Id 
        //            INNER JOIN Authors a ON b.AuthorId = a.Id
        //            LEFT JOIN (
        //                SELECT cc.BookId, MAX(cc.ChapterIndex) AS MaxChapterIndex 
        //                FROM Chapters cc 
        //                GROUP BY cc.BookId  
        //            ) maxChapter ON b.Id = maxChapter.BookId
        //            LEFT JOIN Chapters c ON c.BookId = b.Id AND c.ChapterIndex = maxChapter.MaxChapterIndex
        //            WHERE 
        //                (
        //                    (@Title IS NULL OR b.Title COLLATE SQL_Latin1_General_CP1_CI_AI LIKE '%' + @title + '%' COLLATE SQL_Latin1_General_CP1_CI_AI) OR
        //                    (@Title IS NULL OR a.name COLLATE SQL_Latin1_General_CP1_CI_AI LIKE '%' + @title + '%' COLLATE SQL_Latin1_General_CP1_CI_AI)
        //                )
        //            ORDER BY b.UpdatedAt DESC
        //            OFFSET @PageSize * (@Page - 1) ROWS
        //            FETCH NEXT @PageSize ROWS ONLY
        //        ";

        //        var bookDict = new Dictionary<int, Book>();

        //        var result = await connection.QueryAsync<Book, ApplicationUser, Author, Chapter, string, Book>(
        //            query,
        //            (book, user, author, chapter, genresJson) =>
        //            {
        //                Book bookEntry;
        //                if (!bookDict.TryGetValue(book.Id, out bookEntry))
        //                {
        //                    bookEntry = new Book
        //                    {
        //                        Id = book.Id,
        //                        Title = book.Title,
        //                        Slug = book.Slug,
        //                        Status = book.Status,
        //                        Description = book.Description,
        //                        CoverImage = book.CoverImage,
        //                        CreatedAt = book.CreatedAt,
        //                        UpdatedAt = book.UpdatedAt,
        //                        Views = book.Views,
        //                        AuthorId = author.Id
        //                    };

        //                    bookEntry.ChapterLast = new ChapterLast
        //                    {
        //                        Id = chapter?.Id ?? 0,
        //                        ChapterIndex = chapter?.ChapterIndex ?? 0,
        //                        ChapNumber = chapter?.ChapNumber ?? 0,
        //                        Title = chapter?.Title
        //                    };

        //                    bookEntry.Genres = JsonConvert.DeserializeObject<List<Genre>>(genresJson);

        //                    bookDict.Add(book.Id, bookEntry);
        //                }

        //                bookEntry.ApplicationUser = new ApplicationUser { Id = user.Id, UserName = user.UserName, Fullname = user.Fullname };
        //                bookEntry.Author = new Author { Id = author.Id, Name = author.Name };

        //                return bookEntry;
        //            },
        //            new { PageSize = pageSize, Page = page, Title = title },
        //            splitOn: "Id, Id, Id, Id, Genres"
        //        );

        //        // Truy vấn để lấy tổng số sách
        //        var totalBooksQuery = @"
        //            SELECT COUNT(b.Id) 
        //            FROM Books b
        //            INNER JOIN AspNetUsers u ON b.UserId = u.Id 
        //            INNER JOIN Authors a ON b.AuthorId = a.Id
        //            WHERE 
        //                (
        //                    (@Title IS NULL OR b.Title COLLATE SQL_Latin1_General_CP1_CI_AI LIKE '%' + @title + '%' COLLATE SQL_Latin1_General_CP1_CI_AI) OR
        //                    (@Title IS NULL OR a.name COLLATE SQL_Latin1_General_CP1_CI_AI LIKE '%' + @title + '%' COLLATE SQL_Latin1_General_CP1_CI_AI)
        //                )
        //        ";

        //        var totalBooks = await connection.ExecuteScalarAsync<int>(totalBooksQuery, new { Title = title });

        //        // Tính toán totalPages
        //        var totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

        //        // Đóng gói kết quả
        //        var response = new BookTotalPageResult
        //        {
        //            TotalPages = totalPages,
        //            Books = result
        //        };

        //        return response;
        //    }
        //}



        public async Task IncreaseBookViewsAsync(int bookId, string slug)
        {
            if (bookId <= 0 && !String.IsNullOrEmpty(slug))
            {
                var book = await _context.Books.SingleOrDefaultAsync(b => b.Slug == slug);

                if (book == null)
                {
                    throw new KeyNotFoundException("Book not found");
                }

                // Tăng views
                book.Views++;

                // Cập nhật thông tin sách vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }
            else
            {
                // Lấy sách từ cơ sở dữ liệu bằng ID
                var book = await _context.Books.FindAsync(bookId);

                if (book == null)
                {
                    throw new KeyNotFoundException("Book not found");
                }

                // Tăng views
                book.Views++;

                // Cập nhật thông tin sách vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }
           
        }

        public async Task<BookTotalPageResultDto?> GetBooksSearchAllAsync(string keyword, int[] status, short genre, short chapLength, int page, int pageSize)
        {
            var statusArray = status?.Select(s => (int)s).ToList() ?? new List<int>();

            var bookstemp = _context.Books
            .Select(b => new BooksDto
            {
                Id = b.Id,
                Title = b.Title,
                Slug = b.Slug,
                Status = b.Status,
                Description = b.Description,
                ApplicationUserUserName = b.ApplicationUser.UserName!,
                CoverImage = b.CoverImage,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                Views = b.Views,
                AuthorName = b.Author.Name,
                ChapterLast = b.Chapters!
                                    .OrderByDescending(chap => chap.ChapterIndex)
                                    .Select(chap => new ChapterLast { Id = chap.Id, ChapterIndex = chap.ChapterIndex, ChapNumber = chap.ChapNumber, Title = chap.Title })
                                    .FirstOrDefault()!,
                Genres = b.GenreBooks!.Select(bg => new GenreDto
                {
                    Id = bg.Genre.Id,
                    Name = bg.Genre.Name,
                    Description = bg.Genre.Description
                }).ToList()
            })
            .Where(b =>
                (
                    (keyword == null ? true : EF.Functions.Collate(b.Title, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(keyword, "SQL_Latin1_General_CP1_CI_AI")))

                    || (keyword == null ? true : EF.Functions.Collate(b.AuthorName, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(keyword, "SQL_Latin1_General_CP1_CI_AI")))
                )

                && (chapLength == 0 ? true : b.ChapterLast.ChapterIndex >= chapLength)
                && (genre == 0 ? true : b.Genres.Any(g => g.Id == genre))
                && (statusArray.Count == 0 ? true : statusArray.Contains(b.Status))
             )
            .AsQueryable();

            var books = await bookstemp
               
                .OrderByDescending(b => b.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Thêm phần này để chuyển kết quả thành một danh sách



            // Lấy tổng số lượng sách dựa trên điều kiện tìm kiếm
            var totalBooks = await bookstemp
                //.Where(b =>
                //(
                //    (keyword == null ? true : EF.Functions.Collate(b.Title, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(keyword, "SQL_Latin1_General_CP1_CI_AI")))

                //    || (keyword == null ? true : EF.Functions.Collate(b.AuthorName, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(keyword, "SQL_Latin1_General_CP1_CI_AI")))
                //)

                //&& (chapLength == 0 ? true : b.ChapterLast.ChapterIndex >= chapLength)
                //&& (genre == 0 ? true : b.Genres.Any(g => g.Id == genre))
                //&& (statusArray.Count == 0 ? true : statusArray.Contains(b.Status))
                //)
                .CountAsync();

            // Tính toán totalPages
            var totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

            // Đóng gói kết quả
            var result = new BookTotalPageResultDto
            {
                TotalPages = totalPages,
                Books = books
            };

            return result;

        }


        //public async Task<BookTotalPageResult?> GetBooksSearchAllAsync(string keyword, int[] status, short genre, short chapLength, int page, int pageSize)
        //{
        //    using (var connection = new SqlConnection(sqlConnectionString))
        //    {
        //        string query = @"
        //            SELECT  b.Id, b.Title, b.Slug, b.Status, b.Description, b.CoverImage, b.CreatedAt, b.UpdatedAt, b.Views,
        //                    u.Id, u.UserName, u.Fullname,
        //                    a.Id, a.Name,
        //                    c.Id, c.ChapterIndex, c.ChapNumber, c.Title,
        //                    JSON_QUERY((
        //                        SELECT g.Id, g.Name, g.Description
        //                        FROM GenreBooks gb 
        //                        JOIN Genres g ON g.Id = gb.GenreId
        //                        WHERE gb.BookId = b.Id
        //                        FOR JSON PATH
        //                    )) AS Genres
        //                FROM Books b
        //                INNER JOIN AspNetUsers u ON b.UserId = u.Id 
        //                INNER JOIN Authors a ON b.AuthorId = a.Id
        //                LEFT JOIN (
        //                    SELECT cc.BookId, MAX(cc.ChapterIndex) AS MaxChapterIndex 
        //                    FROM Chapters cc 
        //                    GROUP BY cc.BookId  
        //                ) maxChapter ON b.Id = maxChapter.BookId
        //                LEFT JOIN Chapters c ON c.BookId = b.Id AND c.ChapterIndex = maxChapter.MaxChapterIndex
        //                WHERE 
        //                    (
        //                        (@Keyword IS NULL OR b.Title COLLATE SQL_Latin1_General_CP1_CI_AI LIKE '%' + @Keyword + '%' COLLATE SQL_Latin1_General_CP1_CI_AI) OR
        //                        (@Keyword IS NULL OR a.Name COLLATE SQL_Latin1_General_CP1_CI_AI LIKE '%' + @Keyword + '%' COLLATE SQL_Latin1_General_CP1_CI_AI)
        //                    )
        //                    AND (@ChapLength = 0 OR c.ChapterIndex >= @ChapLength)
        //                    AND (@Genre = 0 OR EXISTS (SELECT 1 FROM GenreBooks gb WHERE gb.BookId = b.Id AND gb.GenreId = @Genre))
        //                    AND (@StatusCount = 0 OR b.Status in @Status)
        //                ORDER BY b.UpdatedAt DESC
        //                OFFSET @PageSize * (@Page - 1) ROWS
        //                FETCH NEXT @PageSize ROWS ONLY
        //        ";

        //        var bookDict = new Dictionary<int, Book>();

        //        var result = await connection.QueryAsync<Book, ApplicationUser, Author, Chapter, string, Book>(
        //            query,
        //            (book, user, author, chapter, genresJson) =>
        //            {
        //                Book bookEntry;
        //                if (!bookDict.TryGetValue(book.Id, out bookEntry))
        //                {
        //                    bookEntry = new Book
        //                    {
        //                        Id = book.Id,
        //                        Title = book.Title,
        //                        Slug = book.Slug,
        //                        Status = book.Status,
        //                        Description = book.Description,
        //                        CoverImage = book.CoverImage,
        //                        CreatedAt = book.CreatedAt,
        //                        UpdatedAt = book.UpdatedAt,
        //                        Views = book.Views,
        //                        AuthorId = author.Id
        //                    };

        //                    bookEntry.ChapterLast = new ChapterLast
        //                    {
        //                        Id = chapter?.Id ?? 0,
        //                        ChapterIndex = chapter?.ChapterIndex ?? 0,
        //                        ChapNumber = chapter?.ChapNumber ?? 0,
        //                        Title = chapter?.Title
        //                    };

        //                    bookEntry.Genres = JsonConvert.DeserializeObject<List<Genre>>(genresJson);

        //                    bookDict.Add(book.Id, bookEntry);
        //                }

        //                bookEntry.ApplicationUser = new ApplicationUser { Id = user.Id, UserName = user.UserName, Fullname = user.Fullname };
        //                bookEntry.Author = new Author { Id = author.Id, Name = author.Name };

        //                return bookEntry;
        //            },
        //            new { PageSize = pageSize, Page = page, Keyword = keyword, ChapLength = chapLength, Genre = genre, StatusCount = status == null ? 0 : status?.Length, Status = status },
        //            splitOn: "Id, Id, Id, Id, Genres"
        //        );

        //        // Truy vấn để lấy tổng số sách
        //        var totalBooksQuery = @"
        //            SELECT COUNT(b.Id) 
        //            FROM Books b
        //            INNER JOIN AspNetUsers u ON b.UserId = u.Id 
        //            INNER JOIN Authors a ON b.AuthorId = a.Id
        //            WHERE 
        //                (
        //                    (@Keyword IS NULL OR b.Title COLLATE SQL_Latin1_General_CP1_CI_AI LIKE '%' + @Keyword + '%' COLLATE SQL_Latin1_General_CP1_CI_AI) OR
        //                    (@Keyword IS NULL OR a.Name COLLATE SQL_Latin1_General_CP1_CI_AI LIKE '%' + @Keyword + '%' COLLATE SQL_Latin1_General_CP1_CI_AI)
        //                )
        //                AND (@ChapLength = 0 OR EXISTS (SELECT 1 FROM Chapters c WHERE c.BookId = b.Id AND c.ChapterIndex >= @ChapLength))
        //                AND (@Genre = 0 OR EXISTS (SELECT 1 FROM GenreBooks gb WHERE gb.BookId = b.Id AND gb.GenreId = @Genre))
        //                AND (@StatusCount = 0 OR b.Status in @Status)
        //        ";

        //        var totalBooks = await connection.ExecuteScalarAsync<int>(totalBooksQuery, new { Keyword = keyword, ChapLength = chapLength, Genre = genre, StatusCount = status == null ? 0 : status?.Length, Status = status });

        //        // Tính toán totalPages
        //        var totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

        //        // Đóng gói kết quả
        //        var response = new BookTotalPageResult
        //        {
        //            TotalPages = totalPages,
        //            Books = result
        //        };

        //        return response;
        //    }
        //}

        public async Task<IEnumerable<BooksDto?>> GetBooksStatusCompleteAsync(int page, int pageSize)
        {
            var books = await _context.Books
                .Where(b => b.Status == 1)
                .Select(b => new BooksDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Status = b.Status,
                    Description = b.Description,
                    ApplicationUserUserName = b.ApplicationUser.UserName!,
                    CoverImage = b.CoverImage,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    Views = b.Views,
                    AuthorName = b.Author.Name,
                    ChapterLast = b.Chapters!
                                        .OrderByDescending(chap => chap.ChapterIndex)
                                        .Select(chap => new ChapterLast { Id = chap.Id, ChapterIndex = chap.ChapterIndex, ChapNumber = chap.ChapNumber, Title = chap.Title })
                                        .FirstOrDefault()!,
                    Genres = b.GenreBooks!.Select(bg => new GenreDto
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name,
                        Description = bg.Genre.Description
                    }).ToList()
                })
                .OrderByDescending(b => b.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Thêm phần này để chuyển kết quả thành một danh sách

            return books;
        }

        //public async Task<IEnumerable<Book?>> GetBooksStatusCompleteAsync(int page, int pageSize)
        //{
        //    using (var connection = new SqlConnection(sqlConnectionString))
        //    {
        //        string query = @"
        //           SELECT  b.Id, b.Title, b.Slug, b.Status, b.Description, b.CoverImage, b.CreatedAt, b.UpdatedAt, b.Views,
        //                    u.Id, u.UserName, u.Fullname,
        //                    a.Id, a.Name,
        //                    c.Id, c.ChapterIndex, c.ChapNumber, c.Title,
        //               JSON_QUERY((
        //                 SELECT g.Id, g.Name, g.Description
        //                 FROM GenreBooks gb 
        //                 JOIN Genres g ON g.Id = gb.GenreId
        //                 WHERE gb.BookId = b.Id
        //                 FOR JSON PATH
        //               )) AS Genres
        //            FROM Books b
        //            INNER JOIN AspNetUsers u ON b.UserId = u.Id 
        //            INNER JOIN Authors a ON b.AuthorId = a.Id
        //            LEFT JOIN (
        //                SELECT cc.BookId, MAX(cc.ChapterIndex) AS MaxChapterIndex 
        //                FROM Chapters cc 
        //                GROUP BY cc.BookId  
        //            ) maxChapter ON b.Id = maxChapter.BookId
        //            LEFT JOIN Chapters c ON c.BookId = b.Id AND c.ChapterIndex = maxChapter.MaxChapterIndex
        //            WHERE b.Status = 1
        //            ORDER BY b.UpdatedAt DESC
        //            OFFSET @PageSize * (@Page - 1) ROWS
        //            FETCH NEXT @PageSize ROWS ONLY
        //        ";

        //        var bookDict = new Dictionary<int, Book>();
        //        var genreDict = new Dictionary<string, Genre>();

        //        var result = await connection.QueryAsync<Book, ApplicationUser, Author, Chapter, string, Book>(
        //            query,
        //            (book, user, author, chapter, genresJson) =>
        //            {
        //                Book bookEntry;
        //                if (!bookDict.TryGetValue(book.Id, out bookEntry))
        //                {
        //                    // Tạo bản sao của book và thêm vào bookDict
        //                    bookEntry = new Book
        //                    {
        //                        Id = book.Id,
        //                        Title = book.Title,
        //                        Slug = book.Slug,
        //                        Status = book.Status,
        //                        Description = book.Description,
        //                        CoverImage = book.CoverImage,
        //                        CreatedAt = book.CreatedAt,
        //                        UpdatedAt = book.UpdatedAt,
        //                        Views = book.Views,
        //                        AuthorId = author.Id
        //                    };

        //                    bookEntry.ChapterLast = new ChapterLast
        //                    {
        //                        Id = chapter?.Id ?? 0,
        //                        ChapterIndex = chapter?.ChapterIndex ?? 0,
        //                        ChapNumber = chapter?.ChapNumber ?? 0,
        //                        Title = chapter?.Title
        //                    };

        //                    bookEntry.Genres = JsonConvert.DeserializeObject<List<Genre>>(genresJson);

        //                    bookDict.Add(book.Id, bookEntry);
        //                }

        //                bookEntry.ApplicationUser = new ApplicationUser { Id = user.Id, UserName = user.UserName, Fullname = user.Fullname };
        //                bookEntry.Author = new Author { Id = author.Id, Name = author.Name };

        //                return bookEntry;
        //            },
        //            new { PageSize = pageSize, Page = page },
        //            splitOn: "Id, Id, Id, Id, Genres"
        //        );
        //        return result;
        //    }
        //}
    }

}

