using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Dto
{
	public class ApplicationUserDto
	{
        public string Id { get; set; }

        public string? UserName { get; set; }

		public string? FullName { get; set; }

        public string Email { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public string Avatar { get; set; }

        public string PhoneNumber { get; set; }

        public DateTimeOffset BirthDay { get; set; }
    }
}

