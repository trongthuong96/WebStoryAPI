using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using AutoMapper;
using DataAccess.Repository.IRepository;
using DataAccess.Services.IServices;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Models;
using Models.Dto;
using Models.Dto.Crawling;
using Models.Dto.Crawling.MeTruyenChu;
using Models.Dto.Crawling.shuba;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Utility;

namespace DataAccess.Services
{
    public class CrawlingService : ICrawlingService
    {
        private readonly HttpClient _httpClient;
        private bool _isCallingApi;
        private readonly IBookRepository _bookRepository;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        private readonly IChapterRepository _chapterRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IChapterService _chapterService;
        private readonly IChineseBookRepository _chineseBookRepository;

        public CrawlingService
            (
                HttpClient httpClient,
                IBookRepository bookRepository,
                IMapper mapper,
                IChapterRepository chapterRepository,
                IHttpContextAccessor httpContextAccessor,
                IBookService bookService,
                IChapterService chapterService,
                IChineseBookRepository chineseBookRepository
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _bookService = bookService;
            _chapterRepository = chapterRepository;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient), "HttpClient instance cannot be null.");
            _isCallingApi = false;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _chapterService = chapterService;
            _chineseBookRepository = chineseBookRepository;
        }

        public async Task<(string, string)> GetBookMeTruyenCV(LinksDto links)
        {
            try
            {
                var requestData = new
                {
                    url = links.UrlBook,
                    url_api = links.UrlChapter
                };

                string jsonContent = JsonConvert.SerializeObject(requestData);

                using (HttpClient httpClient = _httpClient)
                {
                    // Lấy thông tin user hiện tại từ HttpContextAccessor
                    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    // Kiểm tra xem UserId của người dùng đang thao tác có trong token hay không
                    if (userId == null)
                    {
                        userId = "a2ea16da-c3fc-48fa-9a68-0e1623f7a5df";
                    }

                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await httpClient.PostAsync(links.UrlPost, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();

                        BookChaptersCrawlingMTCDto? data = JsonConvert.DeserializeObject<BookChaptersCrawlingMTCDto>(responseBody);

                        string bookSlug = SD.ConvertToSlug(data.Book.Title);

                        if (data != null)
                        {

                            var book = await _bookRepository.GetBookBySlugAsync(bookSlug);

                            bool flag = false;

                            if (book == null)
                            {
                                BookCreateDto bookCreateDto = data.Book;

                                await _bookService.AddBook(bookCreateDto);

                                book = await _bookRepository.GetBookBySlugAsync(bookSlug);

                                flag = true;
                            }

                            var chapList = data.Chapters;

                            if (chapList != null && chapList.Count() > 0)
                            {
                                int countChap = await _chapterRepository.CountAsync(c => c.BookId == book.Id);
                                if (flag || countChap <= 0)
                                {
                                    var chaps = _mapper.Map<IEnumerable<Chapter>>(chapList);

                                    chaps.ToList().ForEach(c =>
                                    {
                                        c.BookId = book.Id;
                                        c.UserId = userId;
                                        c.CreatedAt = DateTime.UtcNow;
                                        c.UpdatedAt = DateTime.UtcNow;
                                    });

                                    await _chapterRepository.AddOrUpdateRangeAsync(chaps);
                                }

                                else
                                {
                                    var chaps = _mapper.Map<IEnumerable<Chapter>>(chapList);

                                    var chapsRead = new List<Chapter>();


                                    foreach (var chap in chaps)
                                    {
                                        chap.BookId = book.Id;
                                        chap.CreatedAt = DateTime.UtcNow;
                                        chap.UpdatedAt = DateTime.UtcNow;
                                        chap.UserId = userId;

                                        var temp = await _chapterRepository.FindSingleAsync(c => c.BookId == chap.BookId && c.ChapterIndex == chap.ChapterIndex);

                                        if (temp != null)
                                        {
                                            temp.Title = chap.Title;
                                            temp.ChapNumber = chap.ChapNumber;
                                        }
                                        chapsRead.Add(temp == null ? chap : temp);
                                    }

                                    await _chapterRepository.AddOrUpdateRangeAsync(chapsRead);

                                }
                            }
                        }

                        return (bookSlug, response.StatusCode.ToString());
                    }
                    else
                    {
                        return (null, "Error: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                return (null, "Error: " + ex.Message);
            }
        }

        //public async Task<BookDto> GetBookChapter2MeTruyenCV(string uri)
        //{
        //    // Tách lấy phần slug từ đường dẫn
        //    string[] segments = uri.Split('/');
        //    string slug = segments[segments.Length - 1];

        //    // Khai báo driver kiểu IWebDriver 
        //    var options = new FirefoxOptions();
        //    options.AddArgument("--headless"); // Thêm tùy chọn để chạy ở chế độ headless

        //    IWebDriver driver = new FirefoxDriver(options);

        //    // Sử dụng các phương thức của IWebDriver 
        //    driver.Navigate().GoToUrl(uri);

        //    // Lấy HTML của trang web bằng Selenium
        //    string htmlIntro = driver.PageSource;

        //    // Find element
        //    var navChapterTab = driver.FindElement(By.XPath("//*[@id=\"nav-tab-chap\"]"));

        //    navChapterTab.Click();

        //    // Sử dụng WebDriverWait để đợi cho một điều kiện cụ thể
        //    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        //    //*[@id="chapter-list"]/div/div[2]/div/span
        //    By spannerLocator = By.XPath("//*[@id=\"chapter-list\"]/div/div[2]/div/span");
        //    wait.Until(ExpectedConditions.InvisibilityOfElementWithText(spannerLocator, "Loading..."));

        //    // Lấy HTML của trang web bằng Selenium
        //    string htmlChap = driver.PageSource;

        //    // Đóng trình duyệt
        //    driver.Quit();

        //    // Sử dụng HtmlAgilityPack để parse HTML Intro book
        //    var docIntro = new HtmlDocument();
        //    docIntro.LoadHtml(htmlChap);

        //    // book info
        //    string titleBook = docIntro.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/main/div/div[1]/div/div/div/div[2]/div[1]/h1").InnerText;

        //    string description = docIntro.DocumentNode.SelectSingleNode("//*[@id=\"nav-intro\"]/div/div[1]/div[1]/div").InnerHtml;

        //    string authorName = docIntro.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/main/div/div[1]/div/div/div/div[2]/ul[1]/li[1]").InnerText;

        //    string genre = docIntro.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/main/div/div[1]/div/div/div/div[2]/ul[1]/li[3]").InnerText;

        //    var coverImageDiv = docIntro.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/main/div/div[1]/div/div/div/div[1]/div/img");

        //    string status = docIntro.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/main/div/div[1]/div/div/div/div[2]/ul[1]/li[2]").InnerText;

        //    var coverImage = "";
        //    // Kiểm tra xem divNode có tồn tại không
        //    if (coverImageDiv != null)
        //    {
        //        // Lấy giá trị của thuộc tính href
        //        coverImage = coverImageDiv.GetAttributeValue("src", "");
        //    }

        //    short statusNum = 0;

        //    if (status.Trim().Equals("Đang ra"))
        //    {
        //        statusNum = 0;
        //    }
        //    else if (status.Trim().Equals("Hoàn thành"))
        //    {
        //        statusNum = 1;
        //    }

        //    var genres = new List<GenreBookCreateDto>();

        //    genres.Add(new GenreBookCreateDto
        //    {
        //        GenreId = SD.ChangeGenreStringToShort(genre)
        //    });

        //    // Lấy thông tin user hiện tại từ HttpContextAccessor
        //    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    // Kiểm tra xem UserId của người dùng đang thao tác có trong token hay không
        //    if (userId == null)
        //    {
        //        userId = "a2ea16da-c3fc-48fa-9a68-0e1623f7a5df";
        //    }

        //    var bookCreateDto = new BookCreateDto
        //    {
        //        Title = titleBook,
        //        Description = description,
        //        AuthorName = authorName,
        //        CoverImage = coverImage,
        //        status = statusNum,
        //        genreBookCreateDto = genres,
        //        UserId = userId
        //    };

        //    if (bookCreateDto == null)
        //    {
        //        throw new InvalidOperationException("Invalid crawling");
        //    }

        //    var book = await _bookRepository.GetBookBySlugAsync(slug);

        //    bool flag = false;

        //    if (book == null)
        //    {
        //        await _bookService.AddBook(bookCreateDto);

        //        book = await _bookRepository.GetBookBySlugAsync(slug);

        //        flag = true;
        //    }

        //    // Sử dụng HtmlAgilityPack để parse HTML Chapter
        //    var docChap = new HtmlDocument();
        //    docChap.LoadHtml(htmlChap);

        //    var chapterListNode = docChap.DocumentNode.SelectSingleNode("//*[@id=\"chapter-list\"]/div/div[2]/div");

        //    if (chapterListNode == null)
        //    {
        //        throw new InvalidOperationException("Invalid crawling");
        //    }

        //    // Lấy danh sách các div con
        //    // Lấy tất cả các thẻ div và a
        //    var divNodes = chapterListNode.SelectNodes("//*[@id=\"chapter-list\"]/div/div[2]/div/div/a/div/div/small");

        //    if (divNodes != null)
        //    {
        //        // Xóa tất cả các div và a
        //        foreach (var node in divNodes)
        //        {
        //            node.Remove();
        //        }
        //    }

        //    var chapterContent = chapterListNode.InnerText;

        //    // Tách các dòng bằng ký tự xuống dòng "\n" và bỏ qua dòng đầu tiên
        //    var lines = chapterContent.Split('\n').Skip(1).Select(line => SD.ExtractTitleChapter(line.Trim())).ToList();

        //    // Gán danh sách ChapterDto
        //    var chapters = lines.Select((title, index) => new ChapterCreateDto
        //    {
        //        ChapterIndex = (short)(index + 1),
        //        ChapNumber = (short)(index + 1),
        //        Title = title
        //    }).ToList();

        //    var chapList = chapters;
        //    if (chapList != null && chapList.Count() > 0)
        //    {
        //        int countChap = await _chapterRepository.CountAsync(c => c.BookId == book.Id);
        //        if (flag || countChap <= 0)
        //        {
        //            var chaps = _mapper.Map<IEnumerable<Chapter>>(chapList);

        //            chaps.ToList().ForEach(c =>
        //            {
        //                c.BookId = book.Id;
        //                c.UserId = userId;
        //                c.CreatedAt = DateTime.UtcNow;
        //                c.UpdatedAt = DateTime.UtcNow;
        //            });

        //            await _chapterRepository.AddOrUpdateRangeAsync(chaps);
        //        }

        //        else
        //        {
        //            var chaps = _mapper.Map<IEnumerable<Chapter>>(chapList);

        //            // Sử dụng Entity Framework Plus
        //            var chapsData = await _chapterRepository.FindAsync(c => c.BookId == book.Id);


        //            var chapsRead = new List<Chapter>();


        //            await Parallel.ForEachAsync(chaps, async (chap, cancellationToken) =>
        //            {
        //                chap.BookId = book.Id;
        //                chap.CreatedAt = DateTime.UtcNow;
        //                chap.UpdatedAt = DateTime.UtcNow;
        //                chap.UserId = userId;

        //                var temp = chapsData.FirstOrDefault(c => c.BookId == chap.BookId && c.ChapterIndex == chap.ChapterIndex);

        //                if (temp != null)
        //                {
        //                    temp.Title = chap.Title;
        //                    temp.ChapNumber = chap.C‌​hapNumber;
        //                }

        //                lock (chapsRead)
        //                {
        //                    chapsRead.Add(temp == null ? chap : temp);
        //                }
        //            });


        //            await _chapterRepository.AddOrUpdateRangeAsync(chapsRead);

        //        }
        //    }

        //    var getBook = await _bookRepository.GetBookByIdAsync(book.Id);

        //    return _mapper.Map<BookDto>(getBook);
        //}


        public async Task<ChapterDto> GetChapterMeTruyenCV(string slug, short chapterIndex)
        {
            try
            {
                var chapter = await _chapterRepository.FindSingleAsync(c => c.Book.Slug == slug && c.ChapterIndex == chapterIndex);

                if (chapter == null)
                {
                    throw new Utility.NotFoundException("Not found");
                }

                if (chapter.Content != null && chapter.Content.Length > 500)
                {
                    return await _chapterRepository.GetChapterByChapterIndexAsync(slug, chapterIndex);
                }

                string urlChap = "https://metruyencv.com/truyen/" + slug + "/chuong-" + chapterIndex;

                var client = new HttpClient();
                var response = await client.GetAsync(urlChap);

                if (response.IsSuccessStatusCode)
                {
                    var html = await response.Content.ReadAsStringAsync();

                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    var chapterNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"article\"]");

                    if (chapterNode == null)
                    {
                        throw new InvalidOperationException("Invalid crawling");
                    }

                    // Lấy danh sách các div con
                    // Lấy tất cả các thẻ div và a
                    var divNodes = chapterNode.SelectNodes("//div | //a");

                    if (divNodes != null)
                    {
                        // Xóa tất cả các div và a
                        foreach (var node in divNodes)
                        {
                            node.Remove();
                        }
                    }

                    var chapterContent = chapterNode.InnerHtml;

                    chapter.Content = chapterContent;
                    chapter.UpdatedAt = DateTime.UtcNow;

                    await _chapterRepository.UpdateAsync(chapter);

                    return await _chapterRepository.GetChapterByChapterIndexAsync(slug, chapterIndex);

                }
                else
                {
                    throw new InvalidOperationException("Invalid crawling");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// https://www.69shuba.com/book/43246.htm
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="chapterIndex"></param>
        /// <returns></returns>
        /// <exception cref="Utility.NotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<string> GetBook69shuba(string uri)
        {
            try
            {
                var uriTemp = uri.Split("/")[2];
                if (!uriTemp.Equals(SD.LINK69XINSHU) && !uriTemp.Equals(SD.LINK69SHU))
                {
                    return null;
                }

                if (string.IsNullOrEmpty(uri.Split("/")[4].Split(".")[0]))
                {
                    return null;
                }

                string bookId69ShubaString = uri.Split("/")[4].Split(".")[0];

                int bookId69Shuba = 0;

                if (!int.TryParse(bookId69ShubaString, out bookId69Shuba))
                {
                    return null;
                }

                BookCrawlDto bookCrawl = new BookCrawlDto();
                var chapterCrawlls = new List<ChapterCrawllDto>();

                string urlInfo = "";
                string urlBase = "";
                string chineseSiteName = "";

                if (uriTemp.Equals(SD.LINK69XINSHU))
                {
                    urlBase = "https://www.69xinshu.com/book/";
                    chineseSiteName = "69xinshu";

                } else if (uriTemp.Equals(SD.LINK69SHU))
                {
                    urlBase = "https://www.69shuba.com/book/";
                    chineseSiteName = "69shuba";
                }

                urlInfo = urlBase + bookId69ShubaString + ".htm";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlInfo);
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

                await using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("GB2312")))
                    {

                        var chineseBook = await _chineseBookRepository.FindSingleAsync<ChineseBook>(cb => cb.ChineseSite == urlInfo, cb => new ChineseBook { Id = cb.Id, BookId = cb.BookId });

                        int bookId = 0;
                        int chineseBookId = 0;
                        string slug = "";

                        if (chineseBook == null)
                        {
                            var html = await reader.ReadToEndAsync();

                            if (html == null)
                            {
                                return null;
                            }


                            // book info
                            var doc = new HtmlDocument();
                            doc.LoadHtml(html);

                            ///html/body/div[2]/ul/li[1]/div[2]/div/div[2]/div
                            ///html/body/div[2]/ul/li[1]/div[2]/div[2]/div[2]/div
                            var descriptionNode = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/ul/li[1]/div[2]/div[2]/div[2]/div");

                            if (descriptionNode == null)
                            {
                                descriptionNode = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/ul/li[1]/div[2]/div/div[2]/div");
                            }

                            string description = descriptionNode.InnerHtml;

                            var coverImage = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/ul/li[1]/div[1]/div/div[1]/img").Attributes["src"].Value;

                            // Tìm thẻ meta với thuộc tính property là "og:novel:status"
                            var metaNode = doc.DocumentNode.SelectSingleNode($"//meta[@property='og:novel:status']");

                            // Nếu tìm thấy, trả về giá trị của thuộc tính content
                            var status = metaNode?.GetAttributeValue("content", null).Trim();

                            // Lấy nội dung của thẻ script
                            var scriptNode = doc.DocumentNode.SelectSingleNode("//script[contains(., 'var bookinfo')]");
                            if (scriptNode != null)
                            {
                                // Lấy nội dung JSON từ thẻ script
                                string jsonContent = scriptNode.InnerText;

                                // Lấy dữ liệu JSON bằng cách loại bỏ các phần không phải là JSON
                                int startIndex = jsonContent.IndexOf("{");
                                int endIndex = jsonContent.LastIndexOf("}") + 1;

                                if (startIndex >= 0 && endIndex > startIndex)
                                {
                                    string jsonOnly = jsonContent.Substring(startIndex, endIndex - startIndex);

                                    // Phân tích dữ liệu JSON sử dụng Newtonsoft.Json
                                    var bookInfo = JsonConvert.DeserializeObject<BookInfoDto>(jsonOnly);
                                    bookCrawl.ChineseSite = urlInfo;
                                    bookCrawl.ChineseSiteName = chineseSiteName;
                                    bookCrawl.ChineseTagNames = bookInfo.tags;
                                    bookCrawl.ChineseAuthorName = bookInfo.author;
                                    bookCrawl.ChineseTitle = bookInfo.articlename;
                                    bookCrawl.ChineseDescription = new string(description.Where(c => !"\r\n".Contains(c)).ToArray());
                                    bookCrawl.ChineseGenreName = new List<string>();
                                    bookCrawl.ChineseGenreName.Add(bookInfo.sortName);
                                    bookCrawl.ChineseCoverImage = coverImage;

                                    if (status.Equals("全本"))
                                    {
                                        bookCrawl.ChineseStatus = 1;
                                    }
                                    else
                                    {
                                        bookCrawl.ChineseStatus = 0;
                                    }
                                }
                            }

                            var temp = await _bookService.AddAndUpdateBookChinese(bookCrawl);
                            bookId = temp.Item1;
                            chineseBookId = temp.Item2;
                            slug = temp.Item3;
                        }
                        else
                        {
                            bookId = chineseBook.BookId;
                            chineseBookId = chineseBook.Id;

                            slug = await _bookRepository.FindSingleAsync<string>(b => b.Id == bookId, b => b.Slug);
                        }
                        // chapter info
                        string urlChap = urlBase + bookId69ShubaString + "/";

                        request = (HttpWebRequest)WebRequest.Create(urlChap);
                        response = (HttpWebResponse)await request.GetResponseAsync();

                        await using (Stream stream1 = response.GetResponseStream())
                        {
                            using (StreamReader reader1 = new StreamReader(stream1, Encoding.GetEncoding("GB2312")))
                            {
                                var html1 = await reader1.ReadToEndAsync();

                                if (html1 == null)
                                {
                                    return null;
                                }

                                var doc1 = new HtmlDocument();
                                doc1.LoadHtml(html1);

                                var titleNode = doc1.DocumentNode.SelectSingleNode("//*[@id=\"catalog\"]/ul");

                                if (titleNode != null)
                                {
                                    var titleElements = titleNode.SelectNodes("//*[@id=\"catalog\"]/ul/li/a");
                                    if (titleElements != null)
                                    {
                                        var tasks = titleElements.Select(async (titleElement) =>
                                        {
                                            string title = await RemoveChapterNumberAsync(titleElement.InnerText);

                                            // Lấy giá trị của thuộc tính data-num
                                            int dataNum;
                                            if (titleElement.ParentNode.Attributes["data-num"] != null &&
                                                int.TryParse(titleElement.ParentNode.Attributes["data-num"].Value, out dataNum))
                                            {
                                                // Lấy giá trị của thuộc tính href
                                                var hrefAttribute = titleElement.Attributes["href"].Value;
                                                string href = hrefAttribute != null ? hrefAttribute : null;

                                                var chapter = new ChapterCrawllDto
                                                {
                                                    ChineseTitle = title,
                                                    ChapNumber = (short)(dataNum),
                                                    ChapterIndex = (short)(dataNum), // Bắt đầu từ 1
                                                    ChineseSite = href
                                                };

                                                chapterCrawlls.Add(chapter);
                                            }
                                        });

                                        // Chờ tất cả các nhiệm vụ bất đồng bộ hoàn thành
                                        await Task.WhenAll(tasks);

                                    }
                                }


                                await _chapterService.AddAndUpdateChaptersCrawl(bookId, chineseBookId, chapterCrawlls);

                                return slug;

                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// https://69shuba 69SHU chapter content
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="Utility.NotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<ChapterDto> GetContentChap69shuba(int chineseBookId, short chapterIndex)
        {
            try
            {

                var chapter = await _chapterService.AddAndUpdateChaptersContentCrawl(chineseBookId, chapterIndex);

                if (chapter == null)
                {
                    throw new Utility.NotFoundException("Not found");
                }

                string uri = chapter.ChineseSite;

                if (chapter.Content == null || chapter.Content.Length < 1000)
                {
                    var uriTemp = uri.Split("/")[2];
                    if (!uriTemp.Equals(SD.LINK69SHU) && !uriTemp.Equals(SD.LINK69XINSHU))
                    {
                        throw new Utility.NotFoundException("Not found");
                    }

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

                    await using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("GB2312")))
                        {
                            var html = await reader.ReadToEndAsync();

                            if (html == null)
                            {
                                throw new Utility.NotFoundException("Not found");
                            }


                            // book info
                            var doc = new HtmlDocument();
                            doc.LoadHtml(html);
                            //html/body/div[2]/div[1]/div[3]
                            var contentNode = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[3]");
                            var nodeRemove1 = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[3]/h1");
                            var nodeRemove2 = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[3]/div[1]");
                            var nodeRemove3 = doc.DocumentNode.SelectSingleNode("//*[@id=\"txtright\"]");
                            var nodeRemove4 = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[3]/text()[1]");
                            var nodeRemove5 = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[3]/div[4]");

                            string content = "";

                            // Kiểm tra xem node cần loại bỏ có tồn tại hay không
                            if (contentNode != null)
                            {
                                if (nodeRemove1 != null)
                                {
                                    contentNode.RemoveChild(nodeRemove1);
                                }

                                if (nodeRemove2 != null)
                                {
                                    contentNode.RemoveChild(nodeRemove2);
                                }

                                if (nodeRemove3 != null)
                                {
                                    contentNode.RemoveChild(nodeRemove3);
                                }

                                if (nodeRemove4 != null)
                                {
                                    contentNode.RemoveChild(nodeRemove4);
                                }

                                if (nodeRemove5 != null)
                                {
                                    contentNode.RemoveChild(nodeRemove5);
                                }

                                content = contentNode.InnerHtml.Trim();
                                content = new string(content.Where(c => !"\r\n".Contains(c)).ToArray());

                                chapter.ChineseContent = content;
                                chapter.Content = TranslatorEngine.ChineseToVietPhraseOneMeaningForBatch(content, 0, 0, true).Trim();

                                await _chapterRepository.UpdateAsync(chapter);

                            }
                        }
                        return await _chapterRepository.GetChapterByChapterConentCrawlAsync(chineseBookId, chapterIndex);
                    }

                }
                else
                {

                    return await _chapterRepository.GetChapterByChapterConentCrawlAsync(chineseBookId, chapterIndex);
                }

            }
            catch (Exception ex)
            {
                throw new Utility.NotFoundException(ex.Message);
            }
        }

        /// <summary>
        /// https://fanqienovel.com/page/7077516958534470656
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> GetBookFanqie(string uri)
        {
            try
            {
                if (!uri.Split("/")[2].Equals("fanqienovel.com"))
                {
                    return null;
                }

                if (string.IsNullOrEmpty(uri.Split("/")[4].Split(".")[0]))
                {
                    return null;
                }

                string bookIdFanqieString = uri.Split("/")[4].Split(".")[0].Split("?")[0];

                long bookIdFanqie = 0;

                if (!long.TryParse(bookIdFanqieString, out bookIdFanqie))
                {
                    return null;
                }

                BookCrawlDto bookCrawl = new BookCrawlDto();
                var chapterCrawlls = new List<ChapterCrawllDto>();

                string urlInfo = "https://fanqienovel.com/page/" + bookIdFanqieString;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlInfo);

                // Set the User-Agent header to mimic an Android device
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36";

                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

                await using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
                    {

                        var chineseBook = await _chineseBookRepository.FindSingleAsync<ChineseBook>(cb => cb.ChineseSite == urlInfo, cb => new ChineseBook { Id = cb.Id, BookId = cb.BookId });

                        int bookId = 0;
                        int chineseBookId = 0;
                        string slug = "";

                        var html = await reader.ReadToEndAsync();

                        if (html == null)
                        {
                            return null;
                        }

                        var doc = new HtmlDocument();
                        doc.LoadHtml(html);

                        if (chineseBook == null)
                        {
                            // book info
                            ///*[@id="app"]/div/div[2]/div/div[1]/div/div[2]/div[2]/div[1]
                            var title = doc.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/div/div/div/div[1]/div/div[2]/div[2]/div[1]/h1").InnerText.Trim();

                            //*[@id=\"app\"]/div/div[2]/div/div[1]/div/div[2]/div[2]/div[2]/span[1]
                            var statusString = doc.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/div/div/div/div[1]/div/div[2]/div[2]/div[2]/span[1]").InnerText.Trim();

                            short status = 0;
                            if (statusString.Equals("已完结"))
                            {
                                status = 1;
                            }

                            //*[@id="app"]/div/div[2]/div/div[2]/div/div[2]
                            var description = doc.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/div/div/div/div[2]/div/div[2]").InnerHtml;

                            // init image
                            var coverImage = "";

                            // http://p6-novel.byteimg.com/origin/novel-pic/p2o0bc29dd428f53f19278240f034540a18
                            // script image
                            var scriptNode = doc.DocumentNode.SelectSingleNode("//script[contains(., '@type\":\"NewsArticle')]");

                            if (scriptNode != null)
                            {
                                string jsonContent = scriptNode.InnerText;

                                // Lấy dữ liệu JSON bằng cách loại bỏ các phần không phải là JSON
                                int startIndex = jsonContent.IndexOf("{");
                                int endIndex = jsonContent.LastIndexOf("}") + 1;

                                if (startIndex >= 0 && endIndex > startIndex)
                                {
                                    string jsonOnly = jsonContent.Substring(startIndex, endIndex - startIndex);

                                    // Phân tích dữ liệu JSON sử dụng Newtonsoft.Json
                                    var bookInfo = JsonConvert.DeserializeObject<dynamic>(jsonOnly);

                                    coverImage = bookInfo.image[0].Value;

                                }
                            }
                           
                            //*[@id="app"]/div/div[2]/div/div[1]/div/div[2]/div[3]/div[2]/a/div/span[2]
                            var authorName = doc.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/div/div/div/div[1]/div/div[2]/div[3]/div[2]/a/div/span[2]").InnerText.Trim();

                            //*[@id="app"]/div/div[2]/div/div[1]/div/div[2]/div[2]/div[2]
                            var genresNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/div/div/div/div[1]/div/div[2]/div[2]/div[2]");

                            //var genreRemove = doc.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/div/div[2]/div/div[1]/div/div[2]/div[2]/div[2]/span[1]");

                            if (genresNode != null)
                            {
                                // Select all <span> elements with class="info-label-grey" within the genresNode
                                var genreNodes = genresNode.SelectNodes(".//span[@class='info-label-grey']");
                                bookCrawl.ChineseGenreName = new List<string>();

                                if (genreNodes != null)
                                {
                                    foreach (var node in genreNodes)
                                    {
                                        // Add the inner text of each <span> to the list
                                        bookCrawl.ChineseGenreName.Add(node.InnerText.Trim());
                                    }
                                }
                            }

                            /// set info
                            bookCrawl.ChineseSite = urlInfo;
                            bookCrawl.ChineseSiteName = "fanqie";
                            bookCrawl.ChineseAuthorName = authorName;
                            bookCrawl.ChineseTitle = title;
                            bookCrawl.ChineseDescription = new string(description.Where(c => !"\r\n".Contains(c)).ToArray());
                            bookCrawl.ChineseCoverImage = coverImage;
                            bookCrawl.ChineseStatus = status;

                            var temp = await _bookService.AddAndUpdateBookChinese(bookCrawl);
                            bookId = temp.Item1;
                            chineseBookId = temp.Item2;
                            slug = temp.Item3;
                        }
                        else
                        {
                            bookId = chineseBook.BookId;
                            chineseBookId = chineseBook.Id;

                            slug = await _bookRepository.FindSingleAsync<string>(b => b.Id == bookId, b => b.Slug);
                        }

                        // chapter info //*[@id="app"]/div/div[2]/div/div[2]/div/div[4]/div/div[2]
                        var titleChapterNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/div/div/div/div[2]/div/div[4]/div/div[2]");


                        if (titleChapterNode != null)
                        {
                            //*[@id="app"]/div/div[2]/div/div[2]/div/div[4]/div/div[2]/div[2]/a
                            var Elements = titleChapterNode.SelectNodes("//*[@class='chapter-item']");
                            if (Elements != null)
                            {
                                //foreach (var (element, index) in Elements.Select((element, index) => (element, index)))
                                //{

                                //    string titleAndChapNumber = element.SelectSingleNode("a").InnerText.Trim();

                                //    string title = RemoveChapterNumber(titleAndChapNumber);

                                //    short chapNumber = GetChapterNumber(titleAndChapNumber);


                                //    // Lấy giá trị của thuộc tính href
                                //    var hrefAttribute = element.SelectSingleNode("a").GetAttributeValue("href", "");
                                //    string href = hrefAttribute != null ? hrefAttribute : null;


                                //    var chapter = new ChapterCrawllDto
                                //    {
                                //        ChineseTitle = title,
                                //        ChapNumber = chapNumber,
                                //        ChapterIndex = (short)(index + 1), // Bắt đầu từ 1
                                //        ChineseSite = href
                                //    };

                                //    chapterCrawlls.Add(chapter);
                                //}

                                var tasks = Elements.Select(async (element, index) =>
                                {
                                    string titleAndChapNumber = element.SelectSingleNode("a").InnerText.Trim();

                                    string title = await RemoveChapterNumberAsync(titleAndChapNumber);

                                    short chapNumber = await GetChapterNumberAsync(titleAndChapNumber);

                                    if (chapNumber == 0)
                                    {
                                        chapNumber = (short)(index + 1);
                                    }

                                    // Lấy giá trị của thuộc tính href
                                    var hrefAttribute = element.SelectSingleNode("a").GetAttributeValue("href", "");
                                    string href = hrefAttribute != null ? hrefAttribute : null;


                                    var chapter = new ChapterCrawllDto
                                    {
                                        ChineseTitle = title,
                                        ChapNumber = chapNumber,
                                        ChapterIndex = (short)(index + 1), // Bắt đầu từ 1
                                        ChineseSite = "https://fanqienovel.com" + href
                                    };

                                    chapterCrawlls.Add(chapter);
                                });

                                // Chờ tất cả các nhiệm vụ bất đồng bộ hoàn thành
                                await Task.WhenAll(tasks);


                                await _chapterService.AddAndUpdateChaptersCrawl(bookId, chineseBookId, chapterCrawlls);
                            }
                        }
                        // var getBook = await _bookRepository.GetBookByIdAsync(bookId);

                        return slug;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// https://fanqienovel.com
        /// chapter content
        /// </summary>
        /// <param name="chineseBookId"></param>
        /// <param name="chapterIndex"></param>
        /// <returns></returns>
        /// <exception cref="Utility.NotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<ChapterDto> GetContentChapFanqie(string uriBook, int chineseBookId, short chapterIndex)
        {
            try
            {
                var chapter = await _chapterService.AddAndUpdateChaptersContentCrawl(chineseBookId, chapterIndex);

                if (chapter == null)
                {
                    throw new Utility.NotFoundException("Not found");
                }

                string uriChap = chapter.ChineseSite;

                if (chapter.Content == null || chapter.Content.Length < 1000)
                {
                    // book id fanqie
                    string bookIdFanqieString = uriBook.Split("/")[4].Split(".")[0].Split("?")[0];

                    long bookIdFanqie = 0;

                    if (!long.TryParse(bookIdFanqieString, out bookIdFanqie))
                    {
                        return null;
                    }

                    if (!uriChap.Split("/")[2].Equals("fanqienovel.com"))
                    {
                        throw new Utility.NotFoundException("Not found");
                    }

                    // https://fanqienovel.com/reader/7200020761023775244?enter_from=page
                    // chap id fanqie
                    string chapIdFanqieString = uriChap.Split("/")[4].Split(".")[0].Split("?")[0];
                    long chapIdFanqie = 0;

                    if (!long.TryParse(chapIdFanqieString, out chapIdFanqie))
                    {
                        return null;
                    }

                    using (HttpClient client = new HttpClient())
                    {
                        // Append the necessary query parameters to the API URL
                        string apiUrl = "https://novel.snssdk.com/api/novel/book/reader/full/v1/";
                        apiUrl += $"?device_platform=android&parent_enterfrom=novel_channel_search.tab.&aid=2329&platform_id=1&group_id={bookIdFanqie}&item_id={chapIdFanqie}";

                        HttpResponseMessage response = await client.GetAsync(apiUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            string apiContent = await response.Content.ReadAsStringAsync();

                            // Phân tích JSON để lấy nội dung
                            JsonDocument doc = JsonDocument.Parse(apiContent);
                            JsonElement root = doc.RootElement;

                            if (root.TryGetProperty("code", out JsonElement codeElement) && codeElement.GetInt32() == 0)
                            {
                                // Lấy dữ liệu từ property "data"
                                JsonElement dataElement = root.GetProperty("data");

                                if (dataElement.TryGetProperty("content", out JsonElement contentElement))
                                {
                                    string content = contentElement.GetString();

                                    // Sử dụng nội dung theo nhu cầu của bạn
                                    // ...

                                    // Ví dụ: Set nội dung cho chapter.ChineseContent và chapter.Content
                                    chapter.ChineseContent = content;
                                    chapter.Content = TranslatorEngine.ChineseToVietPhraseOneMeaningForBatch(content, 0, 0, true).Trim();

                                    await _chapterRepository.UpdateAsync(chapter);
                                }
                            }
                        }
                        else
                        {
                            throw new Utility.NotFoundException("Not found");
                        }
                    }

                    return await _chapterRepository.GetChapterByChapterConentCrawlAsync(chineseBookId, chapterIndex);
                }
                else
                {
                    return await _chapterRepository.GetChapterByChapterConentCrawlAsync(chineseBookId, chapterIndex);
                }
            }
            catch (Exception ex)
            {
                throw new Utility.NotFoundException(ex.Message);
            }
        }


        // Hàm để loại bỏ số chương
        private async Task<string> RemoveChapterNumberAsync(string input)
        {
            // Loại bỏ các ký tự số và khoảng trắng ở đầu chuỗi
            string result = input.TrimStart("第0123456789章. ".ToCharArray());

            // Loại bỏ dấu chấm cuối chuỗi nếu có
            if (result.EndsWith("."))
            {
                result = result.Substring(0, result.Length - 1);
            }

            return result.Trim(); // Loại bỏ khoảng trắng cuối chuỗi nếu có
        }

        private async Task<short> GetChapterNumberAsync(string input)
        {
            // Sử dụng biểu thức chính quy để lấy số sau "第" và trước "章" với hoặc không có khoảng trắng
            string pattern = @"第\s*(\d+|\p{IsCJKUnifiedIdeographs}+)\s*章";
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                string chapterNumberString = match.Groups[1].Value;

                // Chuyển đổi chuỗi thành kiểu short
                if (short.TryParse(chapterNumberString, out short result))
                {
                    return result;
                }
                else
                {
                    // Xử lý trường hợp không thể chuyển đổi
                    Console.WriteLine("Không thể chuyển đổi thành kiểu short.");
                }
            }

            // Mặc định trả về 0 nếu không tìm thấy số chương
            return 0;
        }
    }
}