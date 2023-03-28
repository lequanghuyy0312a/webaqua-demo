using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace web_Aqua.Context
{
    public partial class OrderDetail
    {
        [DisplayName("ID")]
        public int OrderDetailId { get; set; }

        [DisplayName("Mã đơn hàng")]
        public int? OrderId { get; set; }

        [DisplayName("Sản Phẩm")]

        public int? ProductId { get; set; }

        [DisplayName("Số lượng")]
        public int? Quantity { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}
