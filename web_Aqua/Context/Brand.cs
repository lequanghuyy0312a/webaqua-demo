using System;
using System.Collections.Generic;

namespace web_Aqua.Context
{
    public partial class Brand
    {
        public Brand()
        {
            Products = new HashSet<Product>();
        }

        public int BrandId { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? Slug { get; set; }
        public bool? ShowOnHomePage { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
        public DateTime? UpdateOnUtc { get; set; }
        public bool? Deleted { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
