
//using System.Net.Http.Headers;
//using System.Text;
//using DataAccess.Services.IServices;
//using Google.Apis.Auth.OAuth2;
//using Google.Apis.PhotosLibrary.v1;
//using Google.Apis.PhotosLibrary.v1.Data;
//using Google.Apis.Util.Store;
//using Newtonsoft.Json;

//namespace DataAccess.Services
//{
//	public class PhotoService : IPhotoService
//	{
//        // Sử dụng HttpClient để gọi API
//        private readonly HttpClient _httpClient;

//        public async Task UploadAvatarAsync(string userId, Stream imageStream)
//        {
//            // Lấy access token
//            var token = await GetGooglePhotosAccessToken();

//            // Thiết lập authorization header
//            _httpClient.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", token);

//            // Đọc stream sang byte array
//            var bytes = new byte[imageStream.Length];
//            await imageStream.ReadAsync(bytes, 0, (int)imageStream.Length);

//            // Tạo metadata cho ảnh
//            var metadata = new
//            {
//                title = $"{userId} avatar",
//                description = "User avatar image"
//            };
//            var json = JsonConvert.SerializeObject(metadata);
//            var content = new StringContent(json, Encoding.UTF8, "application/json");

//            // Upload ảnh
//            var url = $"/v1/uploads";
//            using var avatarContent = new ByteArrayContent(bytes);
//            var response = await _httpClient.PostAsync(url, avatarContent);

//            // Upload metadata
//            url = $"/v1/mediaItems:batchCreate";
//            var createRequest = new BatchCreateMediaItemsRequest()
//            {
//                NewMediaItems = new[] {
//            new NewMediaItem{
//                SimpleMediaItem = new SimpleMediaItem{
//                    UploadToken = response.Headers.GetValues("Location").First()
//                }
//            }
//        }
//            };
//            var createContent = new StringContent(JsonConvert.SerializeObject(createRequest), Encoding.UTF8, "application/json");

//            // Thêm metadata vào ảnh
//            await _httpClient.PostAsync(url, createContent);
//        }


//        // Lấy ảnh avatar cho user
//        public async Task<Stream> GetAvatarAsync(string userId)
//        {

//            // Lấy token
//            var token = await GetGooglePhotosAccessToken();
//            _httpClient.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", token);

//            // Tìm ảnh dựa vào metadata
//            var searchUrl = $"/v1/mediaItems:search";
//            var searchRequest = new SearchMediaItemsRequest()
//            {
//                Filters = new Filters
//                {
//                    ContentFilter = new ContentFilter
//                    {
//                        IncludedContentCategories = new[] { "PHOTO" }
//                    }
//                },
//                PageSize = 100
//            };

//            var searchContent = new StringContent(JsonConvert.SerializeObject(searchRequest), Encoding.UTF8, "application/json");

//            var response = await _httpClient.PostAsync(searchUrl, searchContent);
//            var results = JsonConvert.DeserializeObject<SearchMediaItemsResponse>(await response.Content.ReadAsStringAsync());

//            var avatar = results.MediaItems.FirstOrDefault(i =>
//                i.Filename == $"{userId} avatar");

//            if (avatar == null)
//                return null;

//            // Download ảnh
//            var getUrl = $"/v1/mediaItems/{avatar.Id}/mediaData";
//            response = await _httpClient.GetAsync(getUrl);
//            return await response.Content.ReadAsStreamAsync();
//        }

//        [Obsolete]
//        public async Task<string> GetGooglePhotosAccessToken()
//        {


//            UserCredential credential;
//            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
//            {
//                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
//                    GoogleClientSecrets.Load(stream).Secrets,
//                     new[] { PhotosLibraryService.Scope.Photoslibrary },
//                    "user",
//                    CancellationToken.None, new FileDataStore("credentials"));
//            }

//            var token = credential.Token.AccessToken;
//            return token;
//        }
//    }
//}

