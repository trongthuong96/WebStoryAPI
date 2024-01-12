using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using DataAccess.Repository.IRepository;
using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using Models.Dto;
using Models.Dto.Chapter;
using Models.Dto.Crawling;
using Newtonsoft.Json;
using Utility;

namespace DataAccess.Services
{
    public class ChapterService : IChapterService
	{

        private readonly IChapterRepository _chapterRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBookRepository _bookRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IBookReadingRepository _bookReadingRepository;

        public ChapterService
            (
                IChapterRepository chapterRepository,
                IMapper mapper,
                IHttpContextAccessor httpContextAccessor,
                IBookRepository bookRepository,
                IDistributedCache distributedCache,
                IBookReadingRepository bookReadingRepository
            )
        {
            _chapterRepository = chapterRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _bookRepository = bookRepository;
            _distributedCache = distributedCache;
            _bookReadingRepository = bookReadingRepository;
        }

        public async Task<ChapterDto> GetChapterByIdAsync(long id)
        {
            var chapterDto = await _chapterRepository.GetChapterByIdAsync(id);

            return chapterDto;
        }

        public async Task<ChapterDto?> GetChapterByChapterIndexAsync(string bookSlug, short chapterIndex)
        {
            var chapterDto = await _chapterRepository.GetChapterByChapterIndexAsync(bookSlug, chapterIndex);
            return chapterDto;
        }

        public async Task<IEnumerable<ChapterDto>> GetChapters()
        {
            throw new NotImplementedException();
        }

        // Get list chap by book id
        public async Task<IEnumerable<ChapterDto>?> GetChaptersByBookIdAsync(int bookId)
        {
            var chapters = await _chapterRepository.GetChaptersByBookIdAsync(bookId);
            return _mapper.Map<IEnumerable<ChapterDto>>(chapters);
        }

        // Get chapter content
        public async Task<ChapterDto?> GetChapterConentAsync(int bookId, int chineseBookId, short chapterIndex)
        {
            var chapter = await _chapterRepository.GetChapterConentAsync(bookId, chineseBookId, chapterIndex);

            // Lấy thông tin user hiện tại từ HttpContextAccessor
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (chapter != null && userId != null)
            {
                int? tempChineseBookId = null;

                if (chineseBookId > 0)
                {
                    tempChineseBookId = chineseBookId;
                }

                var bookReading = await _bookReadingRepository.FindSingleAsync(br => br.UserId == userId && br.BookId == bookId && br.ChineseBookId == tempChineseBookId);

                if (bookReading == null)
                {
                    bookReading = new BookReading();
                    bookReading.BookId = bookId;
                    bookReading.ChineseBookId = tempChineseBookId;
                    bookReading.ChapterIndex = chapter.ChapterIndex;
                    bookReading.ChapNumber = chapter.ChapNumber;
                    bookReading.ChapTitle = chapter.Title;
                    bookReading.BookSlug = SD.ConvertToSlug(chapter.BookTitle);
                    bookReading.CreatedAt = DateTime.UtcNow;
                    bookReading.UpdatedAt = DateTime.UtcNow;
                    bookReading.UserId = userId;

                    // add book reading
                    _bookReadingRepository.AddAsync(bookReading);
                }
                else
                {
                    bookReading.UpdatedAt = DateTime.UtcNow;
                    bookReading.ChapterIndex = chapter.ChapterIndex;
                    bookReading.ChapNumber = chapter.ChapNumber;
                    bookReading.ChapTitle = chapter.Title;

                    // update book reading
                    _bookReadingRepository.UpdateAsync(bookReading);
                }
            }

            return chapter;
        }


        public async Task AddChapter(ChapterCreateDto chapterCreateDto)
        {

            try
            {
                // Lấy thông tin user hiện tại từ HttpContextAccessor
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Kiểm tra xem UserId của người dùng đang thao tác có trong token hay không
                if (userId == null)
                {
                    throw new InvalidOperationException("Invalid User");
                }

                chapterCreateDto.Title = await SD.ToTitleCaseAsync(chapterCreateDto.Title);

                // Kiểm tra xem sách có tồn tại không
                if (!await _bookRepository.AnyAsync(b => b.Id == chapterCreateDto.BookId))
                {
                    throw new InvalidOperationException("Book not exists.");
                }

                // Kiểm tra xem tiêu đề chương có tồn tại không
                if (await _chapterRepository.AnyAsync(b =>b.BookId == chapterCreateDto.BookId && b.Title == chapterCreateDto.Title))
                {
                    throw new InvalidOperationException("chapter with the same title already exists.");
                }

                // Kiểm tra xem số chương có tồn tại không
                if (await _chapterRepository.AnyAsync(b => b.BookId == chapterCreateDto.BookId && b.ChapNumber == chapterCreateDto.ChapNumber))
                {
                    throw new InvalidOperationException("chapter with the same number already exists.");
                }

                // Lấy số lượng chương hiện có trong cuốn sách
                var existingChaptersCount = await _chapterRepository.CountAsync(c => c.BookId == chapterCreateDto.BookId);              


                // Chuyển đổi từ BookCreateDto sang Book sử dụng AutoMapper
                var chapter = _mapper.Map<Chapter>(chapterCreateDto);

                // Gán thuộc tính cho sách
                chapter.UserId = userId;
                chapter.BookId = chapterCreateDto.BookId;
                chapter.CreatedAt = DateTime.UtcNow;
                chapter.UpdatedAt = DateTime.UtcNow;

                // Thiết lập ChapterIndex cho chương mới
                chapter.ChapterIndex = (short)(existingChaptersCount + 1);

                // Thêm sách vào repository
                await _chapterRepository.AddAsync(chapter);

            }
            catch (Exception)
            {
                // Log lỗi hoặc xử lý lỗi theo ý bạn
                throw; // Ném lại lỗi để được xử lý ở tầng Controller hoặc nơi sử dụng phương thức này
            }
        }


        // crawling
        public async Task AddAndUpdateChaptersCrawl(int bookId, int chineseBookId, IEnumerable<ChapterCrawllDto> chapterCrawlls)
        {

            // Lấy thông tin user hiện tại từ HttpContextAccessor
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra xem UserId của người dùng đang thao tác có trong token hay không
            if (userId == null)
            {
                userId = "a2ea16da-c3fc-48fa-9a68-0e1623f7a5df";
            }

            if (chapterCrawlls != null && chapterCrawlls.Count() > 0)
            {
                int countChap = await _chapterRepository.CountAsync(c => c.BookId == bookId && c.ChineseBookId == chineseBookId);
                if (countChap <= 0)
                {
                    List<Chapter> chapters = new List<Chapter>();

                    await Parallel.ForEachAsync(chapterCrawlls, async (chap, cancellationToken) =>
                    {
                        var title = await SD.ToTitleCaseAsync(TranslatorEngine.ChineseToVietPhraseOneMeaningForBatch(chap.ChineseTitle, 0, 0, true).Trim());

                        var chapTemp = new Chapter
                        {
                            ChapNumber = chap.ChapNumber,
                            ChapterIndex = chap.ChapterIndex,
                            Title = title,
                            ChineseTitle = chap.ChineseTitle,
                            BookId = bookId,
                            ChineseBookId = chineseBookId,
                            ChineseSite = chap.ChineseSite,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            UserId = userId
                        };

                        lock (chapters)
                        {
                            chapters.Add(chapTemp);
                        }
                    });

                    // Sử dụng List<T> thay vì Task.WhenAll vì Parallel.ForEachAsync hoạt động bất đồng bộ.
                    //var batchSize = 2000;

                    //for (var i = 0; i < chapters.Count; i += batchSize)
                    //{
                    //    var batch = chapters.Skip(i).Take(batchSize).ToList();
                    //    await _chapterRepository.AddRangeAsync(batch);
                    //}

                    // Sử dụng Entity Framework Plus
                    await _chapterRepository.BulkAddRangeAsync(chapters.ToList());

                }

                else
                {

                    List<Chapter> chaptersList = new List<Chapter>();

                    await Parallel.ForEachAsync(chapterCrawlls, async (chap, cancellationToken) =>
                    {
                        var title = await SD.ToTitleCaseAsync(TranslatorEngine.ChineseToVietPhraseOneMeaningForBatch(chap.ChineseTitle, 0, 0, true).Trim());

                        var chapTemp = new Chapter
                        {
                            ChapNumber = chap.ChapNumber,
                            ChapterIndex = chap.ChapterIndex,
                            Title = title,
                            ChineseTitle = chap.ChineseTitle,
                            BookId = bookId,
                            ChineseBookId = chineseBookId,
                            ChineseSite = chap.ChineseSite,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            UserId = userId
                        };

                        lock (chaptersList)
                        {
                            chaptersList.Add(chapTemp);
                        }
                    });

                    

                    // Sử dụng Entity Framework Plus
                    var chaptersDataList = await _chapterRepository.FindAsync(c => c.ChineseBookId == chineseBookId && c.BookId == bookId);


                    var chapsRead = new List<Chapter>();

                    await Parallel.ForEachAsync(chaptersList, (chap, cancellationToken) => {
                        try
                        {
                            var temp = chaptersDataList.FirstOrDefault(c => c.BookId == chap.BookId && c.ChineseBookId == chap.ChineseBookId && c.ChapterIndex == chap.ChapterIndex);

                            //if (temp != null)
                            //{
                            //    temp.Title = chap.Title;
                            //    temp.ChapNumber = chap.ChapNumber;
                            //    temp.ChineseSite = chap.ChineseSite;
                            //}

                            lock (chapsRead)
                            {
                                if (temp == null)
                                {
                                    chapsRead.Add(chap);
                                }
                                //chapsRead.Add(temp == null ? chap : temp);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }

                        return new ValueTask();
                    });

                    //var batchSize = 2000;

                    //for (var i = 0; i < chapsRead.Count; i += batchSize)
                    //{
                    //    var batch = chapsRead.Skip(i).Take(batchSize).ToList();
                    //    await _chapterRepository.AddRangeAsync(batch);
                    //}

                    // Tiếp tục xử lý sau khi tất cả công việc bất đồng bộ hoàn tất
                    await _chapterRepository.BulkAddRangeAsync(chapsRead.ToList());

                    var book = await _bookRepository.GetByIdAsync(bookId);

                    book.UpdatedAt = DateTime.UtcNow;

                    _bookRepository.UpdateAsync(book);
                }
            }
        }

        // crawling
        public async Task<Chapter> AddAndUpdateChaptersContentCrawl(int chineseBookId, short chapterIndex)
        {

            // Lấy thông tin user hiện tại từ HttpContextAccessor
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra xem UserId của người dùng đang thao tác có trong token hay không
            if (userId == null)
            {
                userId = "a2ea16da-c3fc-48fa-9a68-0e1623f7a5df";
            }

            var chapter = await _chapterRepository.FindSingleAsync(c => c.ChineseBookId == chineseBookId && c.ChapterIndex == chapterIndex);

            if (chapter == null)
            {
                return null;
            }

            return chapter;
        }

        public async Task UpdateChapter(long id, ChapterUpdateDto chapterUpdateDto)
        {
            try
            {
                // Lấy sách cần cập nhật từ cơ sở dữ liệu
                var existingChapter = await _chapterRepository.GetByIdAsync(id);

                if (existingChapter == null)
                {
                    // Nếu không tìm thấy sách, có thể đưa ra xử lý hoặc throw một exception
                    throw new KeyNotFoundException("Chapter not found");
                }

                // Kiểm tra xem ID có tồn tại hay không
                bool isIdValid = await _chapterRepository.AnyAsync(b => b.Id == id && b.BookId == chapterUpdateDto.BookId);

                if (!isIdValid)
                {
                    // Nếu ID không hợp lệ, có thể đưa ra xử lý hoặc throw một exception
                    throw new ArgumentException("Fail book for this chapter");
                }

                // Lấy thông tin user hiện tại từ HttpContextAccessor
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Kiểm tra xem UserId của người dùng đang thao tác có trong token hay không
                if (userId == null)
                {
                    throw new InvalidOperationException("Invalid UserId");
                }

                // Kiểm tra xem số chương có tồn tại không
                if (await _chapterRepository.AnyAsync(c => c.Id != id && c.BookId == chapterUpdateDto.BookId && c.ChapNumber == chapterUpdateDto.ChapNumber))
                {
                    throw new InvalidOperationException("Chapter with the same number already exists.");
                }

                // Sửa thành viết hoa đầu mỗi chữ
                chapterUpdateDto.Title = await SD.ToTitleCaseAsync(chapterUpdateDto.Title);

                // Kiểm tra xem tiêu đề đã được sử dụng bởi cuốn sách khác hay không
                var isTitleUsed = await _chapterRepository.AnyAsync(c => c.Id != id && c.BookId == chapterUpdateDto.BookId && c.Title == chapterUpdateDto.Title);

                if (isTitleUsed)
                {
                    // Nếu tiêu đề đã được sử dụng, có thể đưa ra xử lý hoặc throw một exception
                    throw new InvalidOperationException("Title is already in use by another chapter");
                }

                // Cập nhật thông tin của sách từ dữ liệu mới
                existingChapter.Title = chapterUpdateDto.Title;
                existingChapter.Content = chapterUpdateDto.Content;
                existingChapter.ChapNumber = chapterUpdateDto.ChapNumber;
                existingChapter.UpdatedAt = DateTime.UtcNow;

                // Cập nhật các trường khác nếu cần

                // Sử dụng repository để cập nhật sách
                await _chapterRepository.UpdateAsync(existingChapter);
            }
            catch (Exception)
            {
                // Log lỗi hoặc xử lý lỗi theo ý bạn
                throw; // Ném lại lỗi để được xử lý ở tầng Controller hoặc nơi sử dụng phương thức này
            }
        }

        public async Task DeleteChapter(long id)
        {
            // Kiểm tra xem sách có tồn tại không
            var chapter = await _chapterRepository.GetByIdAsync(id);
            if (chapter == null)
            {
                // Nếu không tìm thấy sách, xử lý tùy thuộc vào yêu cầu của bạn,
                // có thể là throw exception, trả về NotFound, hoặc thông báo khác.
                throw new NotFoundException($"Chapter with id {id} not found");
            }

            // Xóa sách
            await _chapterRepository.DeleteAsync(chapter);
        }

        // GET CHINESE BOOK ID
        public async Task<IEnumerable<ChapterListDto>?> GetChaptersByChineseBookIdAsync(int chineseBookId)
        {
            if (chineseBookId < 0)
            {
                return null;
            }

            var chaptersFromRepository = await _chapterRepository.GetChaptersByChineseBookIdAsync(chineseBookId);

            return chaptersFromRepository;
        }

        //public async Task<IEnumerable<ChapterListDto>?> GetChaptersByChineseBookIdAsync(int chineseBookId)
        //{
        //    if (chineseBookId < 0)
        //    {
        //        return null;
        //    }

        //    string cacheKey = $"chapters_{chineseBookId}";

        //    // Check if data exists in Redis cache
        //    var cachedData = await _distributedCache.GetStringAsync(cacheKey);

        //    if (!string.IsNullOrEmpty(cachedData))
        //    {
        //        // If data exists in cache, deserialize and return it
        //        return JsonConvert.DeserializeObject<IEnumerable<ChapterListDto>>(cachedData);
        //    }
        //    else
        //    {
        //        // If data doesn't exist in cache, retrieve it from the repository
        //        var chaptersFromRepository = await _chapterRepository.GetChaptersByChineseBookIdAsync(chineseBookId);

        //        if (chaptersFromRepository != null)
        //        {
        //            // Serialize the data to store in Redis cache
        //            string serializedData = JsonConvert.SerializeObject(chaptersFromRepository);

        //            // Save the data in Redis cache with an expiration time
        //            var cacheOptions = new DistributedCacheEntryOptions
        //            {
        //                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) // Expires in 1 hour
        //            };

        //            await _distributedCache.SetStringAsync(cacheKey, serializedData, cacheOptions);

        //            return chaptersFromRepository;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}
    }
}

