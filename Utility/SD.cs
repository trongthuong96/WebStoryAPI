using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Slugify;

namespace Utility
{
	public static class SD
	{

        public const string ADMIN = "admin";
        public const string MOD = "mod";
        public const string CONVERT = "convert";
        public const string AUTHOR = "author";
        public const string USER = "user";
        public const string LINK69SHU = "www.69shuba.com";
        public const string LINKFANQIE = "fanqienovel.com";
        public const string LINK69XINSHU = "www.69xinshu.com";
        public const string CORSNAME = "ClientPolicy";

        public static async Task<string> ToTitleCaseAsync(string input)
        {
            return await Task.Run(() =>
            {
                TextInfo textInfo = new CultureInfo("vi-VN", false).TextInfo;
                return textInfo.ToTitleCase(input.ToLower());
            });
        }

        public static string ConvertToSlug(string text)
        {
            // Sử dụng UniDings để loại bỏ dấu thanh
            var unmarkedText = RemoveVietnameseSigns(text);

            // Sử dụng Slugify.Core để chuyển đổi thành slug
            var slugMaker = new SlugHelper();
            var slug = slugMaker.GenerateSlug(unmarkedText);

            return slug;
        }

        public static string ExtractTitleChapter(string title_chap)
        {
            string pattern1 = @"Chương (\d+) (.*)";
            string pattern2 = @"Chương (\d+): (.*)";
            string pattern3 = @"Chương (\d+). (.*)";

            Regex regex1 = new Regex(pattern1);
            Regex regex2 = new Regex(pattern2);
            Regex regex3 = new Regex(pattern3);

            Match match1 = regex1.Match(title_chap);
            Match match2 = regex2.Match(title_chap);
            Match match3 = regex3.Match(title_chap);

            if (match1.Success)
            {
                return match1.Groups[2].Value;
            }
            else if (match2.Success)
            {
                return match2.Groups[2].Value;
            }
            else if (match3.Success)
            {
                return match3.Groups[2].Value;
            }
            else
            {
                // Trả về giá trị mặc định hoặc xử lý theo yêu cầu của bạn
                return "Không có tiêu đề!";
            }
        }

        public static short ChangeGenreStringToShort(string name)
        {
            short genreValue;
            switch (name.ToLower())
            {
                case "tiên hiệp":
                    genreValue = 1;
                    break;
                case "võ hiệp":
                case "kiếm hiệp":
                    genreValue = 2;
                    break;
                case "huyền huyễn":
                    genreValue = 3;
                    break;
                case "đồng nhân":
                    genreValue = 4;
                    break;
                case "đô thị":
                    genreValue = 5;
                    break;
                case "cạnh kỹ":
                    genreValue = 6;
                    break;
                case "ngôn tình":
                    genreValue = 7;
                    break;
                case "võng du":
                    genreValue = 8;
                    break;
                case "khoa huyễn":
                case "huyền nghi":
                    genreValue = 9;
                    break;
                case "lịch sử":
                case "dã sử":
                    genreValue = 10;
                    break;
                case "linh dị":
                    genreValue = 11;
                    break;
                case "kỳ ảo":
                    genreValue = 12;
                    break;
                default:
                    // Xử lý trường hợp không khớp với bất kỳ thể loại nào
                    throw new ArgumentException("Invalid genre");
            }

            // genreValue bây giờ chứa giá trị tương ứng với thể loại
            return genreValue;
        }

        private static string RemoveVietnameseSigns(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (c == 'đ') stringBuilder.Append('d');
                    else if (c == 'Đ') stringBuilder.Append('d');
                    else stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static bool ApproximatelyEquals(this string source, string target, double threshold)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                return false;
            }

            int maxLength = Math.Max(source.Length, target.Length);
            int distance = LevenshteinDistance(source, target);

            double similarity = 1.0 - (double)distance / maxLength;

            return similarity >= threshold;
        }

        // Hàm tính khoảng cách Levenshtein giữa hai chuỗi
        private static int LevenshteinDistance(string source, string target)
        {
            int[,] distance = new int[source.Length + 1, target.Length + 1];

            for (int i = 0; i <= source.Length; i++)
            {
                for (int j = 0; j <= target.Length; j++)
                {
                    if (i == 0)
                    {
                        distance[i, j] = j;
                    }
                    else if (j == 0)
                    {
                        distance[i, j] = i;
                    }
                    else
                    {
                        distance[i, j] = Math.Min(
                            Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                            distance[i - 1, j - 1] + (source[i - 1] == target[j - 1] ? 0 : 1)
                        );
                    }
                }
            }

            return distance[source.Length, target.Length];
        }
    }

}

