using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Models
{
	public class ApplicationUser : IdentityUser
    {
        [MinLength(5, ErrorMessage = "Tên ít nhất chứa 5 ký tự!")]
		public string? Fullname { get; set; }

        public ICollection<Book>? Books { get; set; }

        public ICollection<Rating>? Ratings { get; set; }

        public ICollection<Notification>? Notifications { get; set; }

        // Danh sách các comment của cuốn sách
        public ICollection<Comment>? Comments { get; set; }

        // Thuộc tính ảo để lấy danh sách user
        [NotMapped]
        public virtual ICollection<Book>? Books_Comment
        {
            get
            {
                return Comments?.Select(bbt => bbt.Book).ToList();
            }
        }

        // Danh sách các Bookmark của cuốn sách
        public ICollection<UserBookmark>? UserBookmarks { get; set; }

        // Thuộc tính ảo để lấy danh sách book
        [NotMapped]
        public virtual ICollection<Book>? Books_UserBookmark
        {
            get
            {
                return UserBookmarks?.Select(bbt => bbt.Book).ToList();
            }
        }

        
    }
}

