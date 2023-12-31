using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Dto
{
	public class AuthorCreateDto
	{
        [Required(ErrorMessage = "Tên tác giả không được để trống!")]
        public required string Name { get; set; }

        public AuthorCreateDto()
        {

        }
    }
}

