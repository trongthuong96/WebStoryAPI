using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Dto.ApplicationUser
{
	public class LoginDto
	{
        [Required(ErrorMessage = "Không được để trống!")]
        public required string EmailOrUserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        public required string Password { get; set; }
    }
}

