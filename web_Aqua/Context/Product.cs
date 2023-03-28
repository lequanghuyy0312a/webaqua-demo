using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace web_Aqua.Context
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }
        [DisplayName("ID")]
        public int ProductId { get; set; }

        [DisplayName("Tên SP")]
        public string? Name { get; set; }

        [DisplayName("Hình ảnh")]
        public string? Avatar { get; set; }

        [DisplayName("Loại SP")]
        public int? CategoryId { get; set; }

        [DisplayName("Tóm tắt")]
        public string? ShortDescription { get; set; }

        [DisplayName("Mô tả chi tiết")]
        public string? FullDescription { get; set; }

        [DisplayName("Xuất xứ")]
        public int? BrandId { get; set; }

        [DisplayName("Hiển thị")]
        public bool? ShowOnHomePage { get; set; }


        [DisplayName("Số lượng")]
        public int? Quantity { get; set; }


        [DisplayName("Giá")]
        public string? Price { get; set; }


        [DisplayName("Tồn kho")]
        public int? Inventory { get; set; }

        [DisplayName("Đề xuất")]
        public bool? Recommend { get; set; }
        [NotMapped] public List<IFormFile> ImageUpload { get; set; }

        public virtual Brand? Brand { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
