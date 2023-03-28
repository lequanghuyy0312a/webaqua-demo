using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_Aqua.Context
{
    public partial class ImportProduct
    {
        [DisplayName("ID")]
        public int ImportProductId { get; set; }


        [DisplayName("Tiêu đề")]
        public string? Title { get; set; }


        [DisplayName("Nội dung")]
        public string? Description { get; set; }

        [DisplayName("Mã danh mục")]
        public int? CategoryId { get; set; }

        [DisplayName("Xuất xứ")]
        public int? BrandId { get; set; }

        [DisplayName("hình ảnh")]
        public string? Avatar { get; set; }

        [DisplayName("ngày nhập")]
        public DateTime? CreateOnUtc { get; set; }


        [DisplayName("số lượng")]
        public int? Quantity { get; set; }
        [NotMapped] public List<IFormFile> ImageUpload { get; set; }

        public virtual Brand? Brand { get; set; }
        public virtual Category? Category { get; set; }
    }
}
