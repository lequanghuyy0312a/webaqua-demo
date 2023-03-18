using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_Aqua.Context
{
    public partial class Product
    {
        public Product()
        {
            Orders = new HashSet<Order>();
        }

        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public int? CategoryId { get; set; }
        public string? ShortDiscription { get; set; }
        public string? FullDiscription { get; set; }
        public int? TypeId { get; set; }
        public int? BrandId { get; set; }
        public bool? ShowOnHomePage { get; set; }
        public int? DisplayOrder { get; set; }
        public int? Quantity { get; set; }
        public string? Price { get; set; }
        public int? Inventory { get; set; }

        public virtual Brand? Brand { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        [NotMapped]
        public IFormFile ImageUpload { get; set; }  
    }
}
