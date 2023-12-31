using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Models.Dto.Crawling.Translate
{
    [Serializable]
    public class Configuration
    {
        private const int VERSION = 1;

        public bool BrowserPanel_DisableImages { get; set; }

        public bool BrowserPanel_DisablePopups { get; set; }

        public int VietPhrase_Wrap { get; set; } = 1;

        public int VietPhraseOneMeaning_Wrap { get; set; } = 1;

        public char HotKeys_CopyHighlightedHanViet { get; set; } = '0';

        public char HotKeys_CopyMeaning1 { get; set; } = '1';

        public char HotKeys_CopyMeaning2 { get; set; } = '2';

        public char HotKeys_CopyMeaning3 { get; set; } = '3';

        public char HotKeys_CopyMeaning4 { get; set; } = '4';

        public char HotKeys_CopyMeaning5 { get; set; } = '5';

        public char HotKeys_CopyMeaning6 { get; set; } = '6';

        public char HotKeys_MoveDownOneLine { get; set; } = 'M';

        public char HotKeys_MoveDownOneParagraph { get; set; } = 'N';

        public char HotKeys_MoveLeftOneWord { get; set; } = 'J';

        public char HotKeys_MoveRightOneWord { get; set; } = 'K';

        public char HotKeys_MoveUpOneLine { get; set; } = 'I';

        public char HotKeys_MoveUpOneParagraph { get; set; } = 'U';

        public string HotKeys_F1 { get; set; } = "";

        public string HotKeys_F2 { get; set; } = "";

        public string HotKeys_F3 { get; set; } = "";

        public string HotKeys_F4 { get; set; } = "";

        public string HotKeys_F5 { get; set; } = "";

        public string HotKeys_F6 { get; set; } = "";

        public string HotKeys_F7 { get; set; } = "";

        public string HotKeys_F8 { get; set; } = "";

        public string HotKeys_F9 { get; set; } = "";

        public bool Layout_VietPhrase { get; set; } = true;

        public bool Layout_VietPhraseOneMeaning { get; set; } = true;

        public Font Style_TrungFont { get; set; } = new Font("Arial Unicode MS", 14f);

        public Font Style_HanVietFont { get; set; } = new Font("Arial Unicode MS", 12f);

        public Font Style_VietPhraseFont { get; set; } = new Font("Arial Unicode MS", 12f);

        public Font Style_VietPhraseOneMeaningFont { get; set; } = new Font("Arial Unicode MS", 12f);

        public Font Style_VietFont { get; set; } = new Font("Arial Unicode MS", 12f);

        public Font Style_NghiaFont { get; set; } = new Font("Arial Unicode MS", 12f);

        public Color Style_TrungForeColor { get; set; }

        public Color Style_HanVietForeColor { get; set; }

        public Color Style_VietPhraseForeColor { get; set; }

        public Color Style_VietPhraseOneMeaningForeColor { get; set; }

        public Color Style_VietForeColor { get; set; }

        public Color Style_NghiaForeColor { get; set; }

        public Color Style_TrungBackColor { get; set; }

        public Color Style_HanVietBackColor { get; set; }

        public Color Style_VietPhraseBackColor { get; set; }

        public Color Style_VietPhraseOneMeaningBackColor { get; set; }

        public Color Style_VietBackColor { get; set; }

        public Color Style_NghiaBackColor { get; set; }

        public int TranslationAlgorithm { get; set; }

        public bool PrioritizedName { get; set; } = true;

        public bool AlwaysFocusInViet { get; set; } = true;

        //public static Configuration LoadFromFile(string configFilePath)
        //{
        //    if (!File.Exists(configFilePath))
        //    {
        //        return new Configuration();
        //    }

        //    Configuration result = null;

        //    using (Stream stream = new FileStream(configFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
        //    {
        //        IFormatter formatter = new BinaryFormatter();
        //        int version = (int)formatter.Deserialize(stream);
        //        result = (Configuration)formatter.Deserialize(stream);
        //    }

        //    return result;
        //}

        //public void SaveToFile(string configFilePath)
        //{
        //    using (Stream stream = new FileStream(configFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        //    {
        //        IFormatter formatter = new BinaryFormatter();
        //        formatter.Serialize(stream, 1);
        //        formatter.Serialize(stream, this);
        //    }
        //}
    }
}
