using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Crawling.Translate;
using Utility;

namespace webstory.Controllers
{
    //[EnableCors(SD.CORSNAME)]
    [Route("api/[controller]")]
    public class TranslateController : Controller
    {
        public TranslateController()
        {
            TranslatorEngine.LoadDictionaries();
        }

        // POST api/translate
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TranslationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ChineseText))
            {
                return BadRequest("Text to translate is empty.");
            }

            string translatedText = await Task.Run(() =>
            {
                lock (TranslatorEngine.LastTranslatedWord_VietPhrase)
                {
                    if (!string.IsNullOrEmpty(request.ChineseText))
                    {
                        return TranslatorEngine.ChineseToVietPhraseOneMeaningForBatch(request.ChineseText, 0, 0, true);
                    }
                    return string.Empty;
                }
            });

            var response = new TranslationResponse
            {
                TranslatedText = translatedText
            };

            // Trả về phản hồi JSON
            return Ok(response);
        }

    }
}
