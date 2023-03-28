using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_Aqua.Context
{
    public partial class Category
    {
        public Category()
        {
            Blogs = new HashSet<Blog>();
            ImportProducts = new HashSet<ImportProduct>();
            Products = new HashSet<Product>();
        }

        [DisplayName("ID")]
        public int CategoryId { get; set; }

        [DisplayName("Tên danh mục")]
        public string? Name { get; set; }
        [DisplayName("Hình ảnh")]
        public string? Avatar { get; set; }
        [DisplayName("Hiển thị")]
        public bool? ShowOnHomePage { get; set; }

        [DisplayName("Dành cho")]
        public bool? Inside { get; set; }
        [NotMapped] public List<IFormFile> ImageUpload { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<ImportProduct> ImportProducts { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
