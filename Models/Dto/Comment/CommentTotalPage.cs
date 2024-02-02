using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.Comment
{
    public class CommentTotalPage
    {
        public IEnumerable<CommentDto> Comments { get; set; }

        public int TotalPage { get; set; }

        public CommentTotalPage()
        {
            Comments = new List<CommentDto>();
        }
    }
}
