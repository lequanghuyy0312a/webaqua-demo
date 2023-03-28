using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_Aqua.Context
{
    public partial class Brand
    {
        public Brand()
        {
            ImportProducts = new HashSet<ImportProduct>();
            Products = new HashSet<Product>();
        }
        [DisplayName("ID")]
        public int BrandID { get; set; }
        [DisplayName("Logo")]
        public string? Avatar { get; set; }
        [DisplayName("Hiển thị")]
        public bool? ShowOnHomePage { get; set; }


        [DisplayName("Quốc gia")]
        public string? Nation { get; set; }


        [DisplayName("Công ty")]
        public string? Company { get; set; }

        [NotMapped] public List<IFormFile> ImageUpload { get; set; }

        public virtual ICollection<ImportProduct> ImportProducts { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
