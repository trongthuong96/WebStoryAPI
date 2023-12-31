using System;
namespace Models.Dto.Crawling.shuba
{
	public class BookInfoDto
	{
        public int pageType { get; set; }
        public string pageVer { get; set; }
        public string articleid { get; set; }
        public string articlename { get; set; }
        public string siteName { get; set; }
        public string site { get; set; }
        public string sortName { get; set; }
        public string sortUrl { get; set; }
        public string author { get; set; }
        public string tags { get; set; }
        public string description { get; set; }
    }
}

