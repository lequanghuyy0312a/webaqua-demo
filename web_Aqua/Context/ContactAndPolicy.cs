using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace web_Aqua.Context
{
    public partial class ContactAndPolicy
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Tiêu đề")]
        public string? Name { get; set; }

        [DisplayName("Nội dung")]
        public string? Content { get; set; }
    }
}
