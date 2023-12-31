using System;
namespace Models.Dto.Crawling.Translate
{
    // DTO cho phản hồi dịch
    public class TranslationResponse
    {
        public string OriginalText { get; set; }
        public string TranslatedText { get; set; }
    }
}

