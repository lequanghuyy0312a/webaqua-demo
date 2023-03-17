using System;
using System.Collections.Generic;

namespace web_Aqua.Context
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public string? Name { get; set; }
        public int? ProductId { get; set; }
        public int? UserId { get; set; }
        public double? TotalPrice { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedOnUtc { get; set; }

        public virtual Product? Product { get; set; }
        public virtual User? User { get; set; }
    }
}
