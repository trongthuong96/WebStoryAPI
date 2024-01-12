using System.Globalization;
using System.Security.Cryptography;
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
        public const string XAPPSOURCE = "CfDJ8MwMmTfyyCZNlUEZuFwp4KWu4RiUPEMJTn-wnDzIC3Dq0mouXvZgLtfdwaGVaz891YZ4qBqFsFJQ5laOyFJofH8mwo2G6AjAOYo6crVoAyZZd-D4AfjzmX5oKMhySPRyY4YijkELuvcqpR1yhQubIV0";
        public const string SHAREDSECRETKEY = "85d1a303708926b1c1598289020565e9eb3d9918ae87847f158833862171dee3";
        
        //public static async Task<string> ToTitleCaseAsync(string input)
        //{
        //    return await Task.Run(() =>
        //    {
        //        TextInfo textInfo = new CultureInfo("vi-VN", false).TextInfo;
        //        return textInfo.ToTitleCase(input.ToLower());
        //    });
        //}

        public static async Task<string> ToTitleCaseAsync(string input)
        {
            return await Task.Run(async () =>
            {
                TextInfo textInfo = new CultureInfo("vi-VN", false).TextInfo;
                string[] words = input.Split(' ');

                for (int i = 0; i < words.Length; i++)
                {
                    string word = words[i];

                    if (word.ToUpper() == word) // Chữ viết tắt, đổi hết thành viết hoa
                    {
                        words[i] = word.ToUpper();
                    }
                    else if (i == 0 || !await IsShortWordAsync(word)) // Chữ bình thường, hoa đầu từ
                    {
                        words[i] = textInfo.ToTitleCase(word.ToLower());
                    }
                }

                return string.Join(" ", words);
            });
        }

        private static async Task<bool> IsShortWordAsync(string word)
        {
            // Các từ viết tắt có thể định nghĩa tại đây
            string[] shortWords = { "EQ", "IQ", "SSS" };
            return await Task.FromResult(shortWords.Contains(word.ToUpper()));
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

        public static async Task<string> ComputeHMACSHA256Async(string input, string key)
        {
            // Convert key and input string to byte arrays
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
            {
                using (MemoryStream stream = new MemoryStream(inputBytes))
                {
                    // Compute HMAC asynchronously
                    byte[] hashBytes = await hmac.ComputeHashAsync(stream);

                    // Convert the byte array to a hexadecimal string
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("x2"));
                    }

                    return sb.ToString();
                }
            }
        }

        public static bool VerifyHMACSHA256(string inputString, string secretKey, string hmacHexString)
        {
            // Validation
            if (string.IsNullOrEmpty(inputString) || string.IsNullOrEmpty(secretKey))
                throw new ArgumentException("Invalid parameters");

            // Convert hex string to bytes
            byte[] hmacBytes = HexStringToByteArray(hmacHexString);

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputString));

                // Compare byte arrays        
                return hashBytes.SequenceEqual(hmacBytes);
            }
        }

        private static byte[] HexStringToByteArray(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        private static byte[] key = {12, 15, 0, 2, 4, 5, 9, 7, 8, 56, 11, 12, 13, 14, 45, 16, 47, 48, 12, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 51, 31, 32}; // Khóa từ chuỗi cụ thể
                                                                                                                                                               //private static byte[] key = GenerateKeyFromSpecificString(SD.SHAREDSECRETKEY);


        //private static byte[] key = { 12, 15, 0, 2, 4, 5, 9, 7, 8, 56, 11, 12, 13, 14, 45, 16, 47, 48, 12, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 51, 31, 32 }; // Khóa từ chuỗi cụ thể

        public static async Task<string> DecryptAsync(string encryptedText)
        {
            // Thay thế key và iv bằng giá trị thực tế của bạn
            byte[] key = Convert.FromBase64String("DA8AAgQFCQcIOAsMDQ4tEC8wDBQVFhcYGRobHB0zHyA=");
            byte[] iv = Encoding.UTF8.GetBytes("1234567890123456");

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] cipherText = Convert.FromBase64String(encryptedText);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return await srDecrypt.ReadToEndAsync();
                        }
                    }
                }
            }
        }

        public static async Task<string> EncryptAsync(string plainText)
        {
            // Thay thế key và iv bằng giá trị thực tế của bạn
            byte[] key = Convert.FromBase64String("DA8AAgQFCQcIOAsMDQ4tEC8wDBQVFhcYGRobHB0zHyA=");
            byte[] iv = Encoding.UTF8.GetBytes("1234567890123456");

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            await swEncrypt.WriteAsync(plainText);
                        }
                    }

                    byte[] encryptedBytes = msEncrypt.ToArray();
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }


        private static byte[] GenerateKeyFromSpecificString(string specificString)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(specificString));
                return hashedBytes;
            }
        }

    }

}

