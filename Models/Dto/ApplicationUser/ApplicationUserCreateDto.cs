using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Dto
{
	public class ApplicationUserCreateDto
	{
		[Required(ErrorMessage = "Email không được để trống!")]
		public required string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        public required string Password { get; set; }
    }
}

