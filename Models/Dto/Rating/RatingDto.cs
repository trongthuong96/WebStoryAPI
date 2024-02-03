using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.Rating
{
    public class RatingDto
    {
        public long Id { get; set; }

        public short RatingValue { get; set; }

        public string? Content { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string UserId { get; set; }

        public string FullName { get; set; }

        public string Avatar { get; set; }

        public int BookId { get; set; }
    }
}
