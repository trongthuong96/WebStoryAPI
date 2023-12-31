using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Dto
{
	public class ApplicationUserDto
	{
        public string Id { get; set; }

        public string? UserName { get; set; }

		public string? FullName { get; set; }
    }
}

