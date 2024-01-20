using System;
namespace Models.Dto
{
	public class ApplicationUserUpdateDto
	{
        public string? FullName { get; set; }

        public string Email { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string? Avatar { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime BirthDay { get; set; }
    }
}

