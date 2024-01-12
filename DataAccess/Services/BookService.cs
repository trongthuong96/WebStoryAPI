using System;
using System.Security.Claims;
using AutoMapper;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Models;
using Models.Dto;
using Models.Dto.Book;
using Models.Dto.Crawling;
using Models.Dto.Crawling.shuba;
using Utility;
using static System.Reflection.Metadata.BlobBuilder;

namespace DataAccess.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorRepository _authorRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IGenreBookRepository _genreBookRepository;
        private readonly IChineseBookRepository _chineseBookRepository;
        private readonly IBookTagRepository _bookTagRepository;
        private readonly IBookBookTagRepository _bookBookTagRepository;

        public BookService
            (
                IBookRepository bookRepository,
                IMapper mapper,
                IHttpContextAccessor httpContextAccessor,
                IAuthorRepository authorRepository,
                IGenreRepository genreRepository,
                IGenreBookRepository genreBookRepository,
                IChineseBookRepository chineseBookRepository,
                IBookTagRepository bookTagRepository,
                IBookBookTagRepository bookBookTagRepository
            )
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _authorRepository = authorRepository;
            _genreRepository = genreRepository;
            _genreBookRepository = genreBookRepository;
            _chineseBookRepository = chineseBookRepository;
            TranslatorEngine.LoadDictionaries();
            _bookTagRepository = bookTagRepository;
            _bookBookTagRepository = bookBookTagRepository;
        }

        public async Task<IEnumerable<Book>> GetBooks()
        {
            return await _bookRepository.GetAllAsync();
        }

        // order by updatedAt
        public async Task<IEnumerable<BookListHomeDto?>> GetBooksOrderByUpdatedAtAsync(int page, int pageSize)
        {
            var books = await _bookRepository.GetBooksOrderByUpdatedAtAsync(page, pageSize);
            return books;
        }

        // order by views
        public async Task<IEnumerable<BookListHomeDto?>> GetBooksOrderByViewsAtAsync(int page, int pageSize)
        {
            var books = await _bookRepository.GetBooksOrderByViewsAtAsync(page, pageSize);
            return books;
        }

        // get book by title
        public async Task<BookTotalPageResultDto?> GetBooksByTitleAsync(string title, int page, int pageSize)
        {
            var books = await _bookRepository.GetBooksByTitleAsync(title, page, pageSize);
            return books;
        }

        // get book by status complete
        public async Task<IEnumerable<BookListHomeDto?>> GetBooksStatusCompleteAsync(int page, int pageSize)
        {
            var books = await _bookRepository.GetBooksStatusCompleteAsync(page, pageSize);
            return books;
        }

        // get book author
        public async Task<IEnumerable<BookListHomeDto?>> GetBooksAuthorAsync(int authorId, int page, int pageSize)
        {
            var books = await _bookRepository.GetBooksAuthorAsync(authorId, page, pageSize);
            return books;
        }

        // get book user
        public async Task<IEnumerable<BookListHomeDto?>> GetBooksUserAsync(string userId, int page, int pageSize)
        {
            var books = await _bookRepository.GetBooksUserAsync(userId, page, pageSize);
            return books;
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetBookByIdAsync(id);
        }

        public async Task<BookDto?> GetBookByTitleSlugAsync(string titleSlug)
        {
            var book = await _bookRepository.GetBookByTitleSlugAsync(titleSlug);
            return book;
        }

        public async Task<BookTotalPageResultDto?> GetBooksSearchAllAsync(string keyword, int[] status, short genre, short chapLength, int page, int pageSize)
        {
            var books = await _bookRepository.GetBooksSearchAllAsync(keyword, status, genre, chapLength, page, pageSize);
            return books;
        }

        /// <summary>
        /// Create one book
        /// </summary>
        /// <param name="bookCreateDto"></param>
        /// <returns></returns>
        public async Task AddBook(BookCreateDto bookCreateDto)
        {
            try
            {
                bookCreateDto.Title = await SD.ToTitleCaseAsync(bookCreateDto.Title);

                // Kiểm tra xem sách có tồn tại không
                if (await _bookRepository.AnyAsync(b => b.Title == bookCreateDto.Title))
                {
                    throw new InvalidOperationException("Book with the same title already exists.");
                }

                bookCreateDto.AuthorName = await SD.ToTitleCaseAsync(bookCreateDto.AuthorName);

                // Kiểm tra xem thể loại có tồn tại không
                foreach (var genreBookCreateDto in bookCreateDto.genreBookCreateDto)
                {
                    var genre = await _genreRepository.AnyAsync(g => g.Id == genreBookCreateDto.GenreId);

                    if (!genre)
                    {
                        // Nếu thể loại không tồn tại
                        throw new InvalidOperationException("Invalid Genre");
                    }
                }

                // Kiểm tra xem tác giả có tồn tại không
                var author = await _authorRepository.FindSingleAsync(a => a.Name == bookCreateDto.AuthorName);

                if (author == null)
                {
                    // Nếu tác giả không tồn tại, thêm tác giả mới
                    //author = new Author { Name = bookCreateDto.AuthorName };

                    author = new Author { Name = bookCreateDto.AuthorName};

                    await _authorRepository.AddAsync(author);
                }

                // Chuyển đổi từ BookCreateDto sang Book sử dụng AutoMapper
                var book = _mapper.Map<Book>(bookCreateDto);

                if (String.IsNullOrEmpty(bookCreateDto.UserId))
                {
                    // Lấy thông tin user hiện tại từ HttpContextAccessor
                    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    // Kiểm tra xem UserId của người dùng đang thao tác có trong token hay không
                    if (userId == null)
                    {
                        throw new InvalidOperationException("Invalid User");
                    }
                    book.UserId = userId;
                }                    
                else
                {
                    book.UserId = bookCreateDto.UserId;
                }


                // Gán thuộc tính cho sách
                book.AuthorId = author.Id;
                book.CreatedAt = DateTime.UtcNow;
                book.UpdatedAt = DateTime.UtcNow;
                book.Slug = SD.ConvertToSlug(bookCreateDto.Title);
                // Thêm sách vào repository
                await _bookRepository.AddAsync(book);

                // Tạo liên kết giữa sách và thể loại
                foreach (var genreBookCreateDto in bookCreateDto.genreBookCreateDto)
                {
                    var genreBook = new GenreBook { BookId = book.Id, GenreId = genreBookCreateDto.GenreId };
                    await _genreBookRepository.AddAsync(genreBook);
                }
                
            }
            catch (Exception)
            {
                // Log lỗi hoặc xử lý lỗi theo ý bạn
                throw; // Ném lại lỗi để được xử lý ở tầng Controller hoặc nơi sử dụng phương thức này
            }
        }

        /// <summary>
        /// Update one book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookUpdateDto"></param>
        /// <returns></returns>
        public async Task UpdateBook(int id, BookUpdateDto bookUpdateDto)
        {
            try
            {
                // Kiểm tra xem ID có tồn tại hay không
                bool isIdValid = await _bookRepository.AnyAsync(b => b.Id == id);

                if (!isIdValid)
                {
                    // Nếu ID không hợp lệ, có thể đưa ra xử lý hoặc throw một exception
                    throw new ArgumentException("Invalid book ID");
                }

                // Lấy sách cần cập nhật từ cơ sở dữ liệu
                var existingBook = await _bookRepository.GetByIdAsync(id);

                if (existingBook == null)
                {
                    // Nếu không tìm thấy sách, có thể đưa ra xử lý hoặc throw một exception
                    throw new KeyNotFoundException("Book not found");
                }

                // Lấy thông tin user hiện tại từ HttpContextAccessor
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Kiểm tra xem UserId của người dùng đang thao tác có trong token hay không
                if (userId == null)
                {
                    throw new InvalidOperationException("Invalid UserId");
                }

                // Sửa thành viết hoa đầu mỗi chữ
                bookUpdateDto.Title = await SD.ToTitleCaseAsync(bookUpdateDto.Title);
                bookUpdateDto.AuthorName = await SD.ToTitleCaseAsync(bookUpdateDto.AuthorName);

                // Kiểm tra xem tiêu đề đã được sử dụng bởi cuốn sách khác hay không
                var isTitleUsed = await _bookRepository.AnyAsync(b => b.Id != id && b.Title == bookUpdateDto.Title);

                if (isTitleUsed)
                {
                    // Nếu tiêu đề đã được sử dụng, có thể đưa ra xử lý hoặc throw một exception
                    throw new InvalidOperationException("Title is already in use by another book");
                }

                // Cập nhật thông tin của sách từ dữ liệu mới
                existingBook.Title = bookUpdateDto.Title;
                existingBook.Description = bookUpdateDto.Description;
                existingBook.CoverImage = bookUpdateDto.CoverImage;
                existingBook.UpdatedAt = DateTime.UtcNow;

                // Kiểm tra xem tác giả có tồn tại không
                var author = await _authorRepository.FindSingleAsync(a => a.Name == bookUpdateDto.AuthorName);

                if (author == null)
                {
                    // Nếu tác giả không tồn tại, thêm tác giả mới

                    author = new Author { Name = bookUpdateDto.AuthorName };

                    await _authorRepository.AddAsync(author);
                }

                existingBook.AuthorId = author.Id;
                existingBook.Slug = SD.ConvertToSlug(bookUpdateDto.Title);
                // Cập nhật các trường khác nếu cần

                // Sử dụng repository để cập nhật sách
                await _bookRepository.UpdateAsync(existingBook);
            }
            catch (Exception)
            {
                // Log lỗi hoặc xử lý lỗi theo ý bạn
                throw; // Ném lại lỗi để được xử lý ở tầng Controller hoặc nơi sử dụng phương thức này
            }
        }

        /// <summary>
        /// Delete one book with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task DeleteBook(int id)
        {
            // Kiểm tra xem sách có tồn tại không
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                // Nếu không tìm thấy sách, xử lý tùy thuộc vào yêu cầu của bạn,
                // có thể là throw exception, trả về NotFound, hoặc thông báo khác.
                throw new NotFoundException($"Book with id {id} not found");
            }

            // Xóa sách
            await _bookRepository.DeleteAsync(book);
        }

        /// <summary>
        /// ADD CHINESE BOOK
        /// </summary>
        /// <returns></returns>
        public async Task<(int, int, string)> AddAndUpdateBookChinese(BookCrawlDto bookCrawl)
        {
            try
            {
                var bookChinese = new ChineseBook();
                bookChinese.ChineseTitle = bookCrawl.ChineseTitle;
                bookChinese.ChineseSite = bookCrawl.ChineseSite;
                bookChinese.ChineseSiteName = bookCrawl.ChineseSiteName;
                var authorName = bookCrawl.ChineseAuthorName;
                bookChinese.ChineseDescription = bookCrawl.ChineseDescription;

                var book = new Book();
                book.Title = TranslatorEngine.ChineseToVietPhraseOneMeaningForBatch(bookChinese.ChineseTitle, 0, 0, true).Trim();
                book.Description = TranslatorEngine.ChineseToVietPhraseOneMeaningForBatch(bookChinese.ChineseDescription, 0, 0, true).Trim();
                authorName = TranslatorEngine.ChineseToHanVietForBatch(authorName).Trim();

                book.Title = await SD.ToTitleCaseAsync(book.Title);
                authorName = await SD.ToTitleCaseAsync(authorName);

                //var genresList = new List<string>();


                //foreach(var genreName in bookCrawl.ChineseGenreName)
                //{
                //    var genreNameTemp = await SD.ToTitleCaseAsync(TranslatorEngine.ChineseToVietPhraseOneMeaningForBatch(genreName, 0, 0, true).Trim());
                //    genresList.Add(genreNameTemp);
                //}

                // var genres = new List<Genre>();
                // Kiểm tra xem thể loại có tồn tại không
                //foreach (var (genreName, index) in genresList.Select((genre, index) => (genre, index)))
                //{
                //    var genre = await _genreRepository.AnyAsync(g => g.Name.Equals(genreName));

                //    var genreCreate = new Genre();

                //    if (!genre)
                //    {
                //        // Nếu thể loại không tồn tại
                //        genreCreate.Name = genreName;
                //        genreCreate.ChineseName = bookCrawl.ChineseGenreName[index];
                //        // await _genreRepository.AddAsync(genreCreate);
                //    }
                //    else
                //    {
                //        var name = await SD.ToTitleCaseAsync(TranslatorEngine.ChineseToVietPhraseOneMeaningForBatch(genreName, 0, 0, true).Trim());
                //        genreCreate = await _genreRepository.FindSingleAsync(g => g.ChineseName == genreName || g.Name == name);               
                //    }

                //     genres.Add(genreCreate);
                //}

                var tagList = new List<string>();


                foreach (var tagName in bookCrawl.ChineseGenreName)
                {
                    var tagNameTemp = await SD.ToTitleCaseAsync(TranslatorEngine.ChineseToVietPhraseOneMeaningForBatch(tagName, 0, 0, true).Trim());
                    tagList.Add(tagNameTemp);
                }

                //var tags = new List<BookTag>();
                //// Kiểm tra xem tag có tồn tại không
                //foreach (var (tagName, index) in bookCrawl.ChineseGenreName.Select((tag, index) => (tag, index)))
                //{
                //    var tag = await _bookTagRepository.AnyAsync(g => g.ChineseTagName.Equals(tagName));

                //    var tagCreate = new BookTag();

                //    if (!tag)
                //    {
                //        // Nếu tag không tồn tại
                //        tagCreate.TagName = tagList[index];
                //        tagCreate.ChineseTagName = tagName;
                //        await _bookTagRepository.AddAsync(tagCreate);
                //    }
                //    else
                //    {
                //        var name = await SD.ToTitleCaseAsync(TranslatorEngine.ChineseToVietPhraseOneMeaningForBatch(tagName, 0, 0, true).Trim());
                //        tagCreate = await _bookTagRepository.FindSingleAsync(g => g.ChineseTagName.Equals(tagName) || g.TagName.Equals(name));
                //    }

                //    tags.Add(tagCreate);
                //}

                var tagSemaphore = new SemaphoreSlim(1, 1);
                var tags = new List<BookTag>();

                await Parallel.ForEachAsync(bookCrawl.ChineseGenreName.Select((tag, index) => (tag, index)), async (tagInfo, CancellationToken) =>
                {
                    var (tagName, index) = tagInfo;

                    await tagSemaphore.WaitAsync();

                    try
                    {
                        var tagExists = await _bookTagRepository.AnyAsync(g => g.ChineseTagName.Equals(tagName));

                        var tagCreate = new BookTag();

                        if (!tagExists)
                        {
                            // Nếu tag không tồn tại
                            tagCreate.TagName = tagList[index];
                            tagCreate.ChineseTagName = tagName;
                            await _bookTagRepository.AddAsync(tagCreate);
                        }
                        else
                        {
                            var name = await SD.ToTitleCaseAsync(TranslatorEngine.ChineseToVietPhraseOneMeaningForBatch(tagName, 0, 0, true).Trim());
                            tagCreate = await _bookTagRepository.FindSingleAsync(g => g.ChineseTagName.Equals(tagName) || g.TagName.Equals(name));
                        }

                        lock (tags)
                        {
                            tags.Add(tagCreate);
                        }
                    }
                    finally
                    {
                        tagSemaphore.Release();
                    }
                });


                // genres
                var genresList = await _genreRepository.GetAllAsync();
                IEnumerable<Genre> genres = new List<Genre>();

                var genreTasks = bookCrawl.ChineseGenreName.Select(async genreChinese =>
                {
                    // Thực hiện công việc bất đồng bộ trong mỗi lần lặp
                    var genre = genresList.FirstOrDefault(g => genreChinese.Contains(g.ChineseName));

                    return genre;
                });

                var tempGenres = await Task.WhenAll(genreTasks);

                // Lọc bỏ giá trị null
                genres = tempGenres.Where(genre => genre != null).ToList();

                // Kiểm tra xem tác giả có tồn tại không
                var author = await _authorRepository.FindSingleAsync(a => a.Name == authorName);

                if (author == null)
                {
                    // Nếu tác giả không tồn tại, thêm tác giả mới
                    //author = new Author { Name = bookCreateDto.AuthorName };

                    author = new Author { Name = authorName, ChineseName = bookCrawl.ChineseAuthorName };

                    await _authorRepository.AddAsync(author);
                }

                // Lấy thông tin user hiện tại từ HttpContextAccessor
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Kiểm tra xem UserId của người dùng đang thao tác có trong token hay không
                if (userId == null)
                {
                    userId = "a2ea16da-c3fc-48fa-9a68-0e1623f7a5df";
                }

                // Gán slug book
                string slug  = SD.ConvertToSlug(book.Title);

                // Thêm sách vào repository
                bool checkBook = await _bookRepository.AnyAsync(b => b.Slug.Equals(slug));
                if (!checkBook)
                {
                    // Gán thuộc tính cho sách
                    book.UserId = userId;
                    book.AuthorId = author.Id;
                    book.CreatedAt = DateTime.UtcNow;
                    book.UpdatedAt = DateTime.UtcNow;
                    book.Slug = slug;
                    book.CoverImage = bookCrawl.ChineseCoverImage;
                    book.Status = bookCrawl.ChineseStatus;

                    // add book
                    await _bookRepository.AddAsync(book);
                }
                else
                {
                    book = await _bookRepository.FindSingleAsync(b => b.Slug == slug);

                }

                //// Tạo liên kết giữa sách và thể loại
                //foreach (var genre in genres)
                //{
                //    var genreBook = new GenreBook { BookId = book.Id, GenreId = genre.Id };

                //    bool checkGenreBook = await _genreBookRepository.AnyAsync(gb => gb.BookId == book.Id && gb.GenreId == genre.Id);
                //    if (!checkGenreBook)
                //    {
                //        await _genreBookRepository.AddAsync(genreBook);
                //    }
                //}

                //// Tạo liên kết giữa sách và tags
                //foreach (var tag in tags)
                //{
                //    var tagBook = new BookBookTag { BookId = book.Id, BookTagId = tag.Id };

                //    bool checkTagBook = await _bookBookTagRepository.AnyAsync(gb => gb.BookId == book.Id && gb.BookTagId == tag.Id);
                //    if (!checkTagBook)
                //    {
                //        await _bookBookTagRepository.AddAsync(tagBook);
                //    }
                //}


                var genreSemaphore = new SemaphoreSlim(1, 1);
                tagSemaphore = new SemaphoreSlim(1, 1);

                // Tạo liên kết giữa sách và thể loại
                await Parallel.ForEachAsync(genres, async (genre, CancellationToken) =>
                {
                    await genreSemaphore.WaitAsync();

                    try
                    {
                        var genreBook = new GenreBook { BookId = book.Id, GenreId = genre.Id };

                        bool checkGenreBook = await _genreBookRepository.AnyAsync(gb => gb.BookId == book.Id && gb.GenreId == genre.Id);
                        if (!checkGenreBook)
                        {
                            await _genreBookRepository.AddAsync(genreBook);
                        }
                    }
                    finally
                    {
                        genreSemaphore.Release();
                    }
                });

                // Tạo liên kết giữa sách và tags
                await Parallel.ForEachAsync(tags, async (tag, CancellationToken) =>
                {
                    await tagSemaphore.WaitAsync();

                    try
                    {
                        var tagBook = new BookBookTag { BookId = book.Id, BookTagId = tag.Id };

                        bool checkTagBook = await _bookBookTagRepository.AnyAsync(gb => gb.BookId == book.Id && gb.BookTagId == tag.Id);
                        if (!checkTagBook)
                        {
                            await _bookBookTagRepository.AddAsync(tagBook);
                        }
                    }
                    finally
                    {
                        tagSemaphore.Release();
                    }
                });


                // Kiểm tra xem sách chinese có tồn tại không
                if (!await _chineseBookRepository.AnyAsync(b => b.ChineseSite == bookCrawl.ChineseSite))
                {
                    bookChinese.BookId = book.Id;
                    bookChinese.AuthorId = author.Id;
                    bookChinese.UserId = userId;

                    await _chineseBookRepository.AddAsync(bookChinese);
                }
                else
                {
                    bookChinese = await _chineseBookRepository.FindSingleAsync(b => b.ChineseSite == bookCrawl.ChineseSite);
                }

                return (book.Id, bookChinese.Id, book.Slug);
            }
            catch (Exception)
            {
                // Log lỗi hoặc xử lý lỗi theo ý bạn
                throw; // Ném lại lỗi để được xử lý ở tầng Controller hoặc nơi sử dụng phương thức này
            }
        }
    }
}

