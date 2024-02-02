using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.Comment
{
    public class CommentDto
    {
        public long Id { get; set; }

        public string Content { get; set; }

        public int BookId { get; set; }

        public long? ParentId { get; set; }

        public string UserId { get; set; }

        public string FullName { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public int Count { get; set; }

        public string Avatar { get; set; }
    }
}
