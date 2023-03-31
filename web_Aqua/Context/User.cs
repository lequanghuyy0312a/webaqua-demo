using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace web_Aqua.Context
{
    public partial class User
    {
        public User()
        {
            Blogs = new HashSet<Blog>();
            Orders = new HashSet<Order>();
        }
        [DisplayName("ID")]
        public int UserId { get; set; }

        [DisplayName("Họ")]
        public string? FirstName { get; set; }

        [DisplayName("Tên")]
        public string? LastName { get; set; }

        [Phone]
        [DisplayName("Số điện thoại")]
        public string? Phone { get; set; }


        [EmailAddress]
        [DisplayName("Gmail")]
        public string? Email { get; set; }

        [DisplayName("Mật khẩu")]
        public string? Password { get; set; }

        [DisplayName("Vai trò")]
        public bool? IsAdmin { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime? CreateOnUtc { get; set; }

        [DisplayName("hình ảnh")]
        public string? Avatar { get; set; }
		[DisplayName("Địa chỉ")]
		public string? Address { get; set; }
		[NotMapped] public List<IFormFile> ImageUpload { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
