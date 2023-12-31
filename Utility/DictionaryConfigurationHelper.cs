using System.Reflection;
namespace Utility
{
    public class DictionaryConfigurationHelper
    {
        private static string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string thuatToanNhan = string.Empty;

        public static bool IsNhanByPronouns
        {
            get
            {
                if (string.IsNullOrEmpty(DictionaryConfigurationHelper.thuatToanNhan))
                {
                    DictionaryConfigurationHelper.ReadThuatToanNhan();
                }
                return DictionaryConfigurationHelper.thuatToanNhan == "1";
            }
        }

        public static bool IsNhanByPronounsAndNames
        {
            get
            {
                if (string.IsNullOrEmpty(DictionaryConfigurationHelper.thuatToanNhan))
                {
                    DictionaryConfigurationHelper.ReadThuatToanNhan();
                }
                return DictionaryConfigurationHelper.thuatToanNhan == "2";
            }
        }

        public static bool IsNhanByPronounsAndNamesAndVietPhrase
        {
            get
            {
                if (string.IsNullOrEmpty(DictionaryConfigurationHelper.thuatToanNhan))
                {
                    DictionaryConfigurationHelper.ReadThuatToanNhan();
                }
                return DictionaryConfigurationHelper.thuatToanNhan == "3";
            }
        }

        public static string GetNamesPhuDictionaryPath()
        {
            return DictionaryConfigurationHelper.GetDictionaryPathByKey("NamesPhu");
        }

        public static string GetNamesDictionaryPath()
        {
            return DictionaryConfigurationHelper.GetDictionaryPathByKey("Names");
        }

        public static string GetNamesDictionaryHistoryPath()
        {
            return Path.Combine(Path.GetDirectoryName(DictionaryConfigurationHelper.GetNamesDictionaryPath()), Path.GetFileNameWithoutExtension(DictionaryConfigurationHelper.GetNamesDictionaryPath()) + "History" + Path.GetExtension(DictionaryConfigurationHelper.GetNamesDictionaryPath()));
        }

        public static string GetNamesPhuDictionaryHistoryPath()
        {
            return Path.Combine(Path.GetDirectoryName(DictionaryConfigurationHelper.GetNamesPhuDictionaryPath()), Path.GetFileNameWithoutExtension(DictionaryConfigurationHelper.GetNamesPhuDictionaryPath()) + "History" + Path.GetExtension(DictionaryConfigurationHelper.GetNamesPhuDictionaryPath()));
        }

        public static string GetVietPhraseDictionaryPath()
        {
            return DictionaryConfigurationHelper.GetDictionaryPathByKey("VietPhrase");
        }

        public static string GetVietPhraseDictionaryHistoryPath()
        {
            return Path.Combine(Path.GetDirectoryName(DictionaryConfigurationHelper.GetVietPhraseDictionaryPath()), Path.GetFileNameWithoutExtension(DictionaryConfigurationHelper.GetVietPhraseDictionaryPath()) + "History" + Path.GetExtension(DictionaryConfigurationHelper.GetVietPhraseDictionaryPath()));
        }

        public static string GetChinesePhienAmWordsDictionaryPath()
        {
            return DictionaryConfigurationHelper.GetDictionaryPathByKey("ChinesePhienAmWords");
        }

        public static string GetChinesePhienAmWordsDictionaryHistoryPath()
        {
            return Path.Combine(Path.GetDirectoryName(DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryPath()), Path.GetFileNameWithoutExtension(DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryPath()) + "History" + Path.GetExtension(DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryPath()));
        }

        public static string GetChinesePhienAmEnglishWordsDictionaryPath()
        {
            return DictionaryConfigurationHelper.GetDictionaryPathByKey("ChinesePhienAmEnglishWords");
        }

        public static string GetCEDictDictionaryPath()
        {
            return DictionaryConfigurationHelper.GetDictionaryPathByKey("CEDict");
        }

        public static string GetBabylonDictionaryPath()
        {
            return DictionaryConfigurationHelper.GetDictionaryPathByKey("Babylon");
        }

        public static string GetLacVietDictionaryPath()
        {
            return DictionaryConfigurationHelper.GetDictionaryPathByKey("LacViet");
        }

        public static string GetThieuChuuDictionaryPath()
        {
            return DictionaryConfigurationHelper.GetDictionaryPathByKey("ThieuChuu");
        }

        public static string GetIgnoredChinesePhraseListPath()
        {
            return DictionaryConfigurationHelper.GetDictionaryPathByKey("IgnoredChinesePhrases");
        }

        public static string GetLuatNhanDictionaryPath()
        {
            return DictionaryConfigurationHelper.GetDictionaryPathByKey("LuatNhan");
        }

        public static string GetPronounsDictionaryPath()
        {
            return DictionaryConfigurationHelper.GetDictionaryPathByKey("Pronouns");
        }

        private static string GetDictionaryPathByKey(string dictionaryKey)
        {
            string[] lines = File.ReadAllLines(Path.Combine(DictionaryConfigurationHelper.directoryPath, "Dictionaries.config"));
            string path = string.Empty;

            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line) && !line.StartsWith("#") && line.StartsWith(dictionaryKey + "="))
                {
                    path = line.Split('=')[1];
                    break;
                }
            }

            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(DictionaryConfigurationHelper.directoryPath, path);
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Dictionary Not Found: " + path);
            }

            return path;
        }

        private static void ReadThuatToanNhan()
        {
            string[] lines = File.ReadAllLines(Path.Combine(DictionaryConfigurationHelper.directoryPath, "Dictionaries.config"));

            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line) && !line.StartsWith("#") && line.StartsWith("ThuatToanNhan="))
                {
                    DictionaryConfigurationHelper.thuatToanNhan = line.Split('=')[1];
                    return;
                }
            }
        }
    }
}


