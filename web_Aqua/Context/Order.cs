using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace web_Aqua.Context
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }


 
        [DisplayName("ID")]
        public int OrderId { get; set; }
        [DisplayName("Nội dung")]
        public string? Name { get; set; }
        [DisplayName("Sản Phẩm")]
        public int? ProductId { get; set; }


        [DisplayName("Người mua")]
        public int? UserId { get; set; }

        [DisplayName("Trạng thái")]
        public int? Status { get; set; }
        [DisplayName("Ngày tạo")]
        public DateTime? CreatedOnUtc { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
