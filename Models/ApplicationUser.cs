using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Models
{
	public class ApplicationUser : IdentityUser
    {
        [MinLength(5, ErrorMessage = "Tên ít nhất chứa 5 ký tự!")]
		public string? FullName { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public DateTimeOffset BirthDay { get; set; }

        public string? Avatar { get; set; }

        public ICollection<Book>? Books { get; set; }

        public ICollection<Rating>? Ratings { get; set; }

        public ICollection<Notification>? Notifications { get; set; }

        // Danh sách các comment của cuốn sách
        public ICollection<Comment>? Comments { get; set; }

        // Danh sách các Bookmark của cuốn sách
        public ICollection<UserBookmark>? UserBookmarks { get; set; }
    }
}

