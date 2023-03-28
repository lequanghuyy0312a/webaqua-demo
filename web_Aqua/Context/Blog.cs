using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace web_Aqua.Context
{
    public partial class Blog
    {

        [DisplayName("ID")]
        public int BlogID { get; set; }

        [StringLength(200)]
        [DisplayName("Tiêu đề")]
        public string? Title { get; set; }

        [StringLength(300)]
        [DisplayName("Mô tả")]
        public string? Description { get; set; }

        [DisplayName("Nội dung")]
        public string? BlogContent { get; set; }

        [DisplayName("Ảnh bìa")]
        public string? Thumbnail { get; set; }

        [DisplayName("danh mục")]
        public int? CategoryID { get; set; }

        [DisplayName("ngày tạo")]
        public DateTime? CreateOnUtc { get; set; }

        [DisplayName("người tạo")]
        public int? UserID { get; set; }

        [DisplayName("khu vực")]
        public bool? Country { get; set; }

        [DisplayName("Hiển thị")]
        public bool? ShowOnHomePage { get; set; }
        [NotMapped] public List<IFormFile> ImageUpload { get; set; }
        [NotMapped] public List<IFormFile> uploadedFiles { get; set; }

        public virtual Category? Category { get; set; }
        public virtual User? User { get; set; }
    }
}
