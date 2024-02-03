using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.Rating
{
    public class RatingCreateDto
    {
        public short RatingValue { get; set; }

        public string? Content { get; set; }

        public int BookId { get; set; }
    }
}
