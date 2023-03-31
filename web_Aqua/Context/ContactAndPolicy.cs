using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_Aqua.Context
{
    public partial class ContactAndPolicy
    {
      
        [DisplayName("ID")]
        public int Id { get; set; }
        [DisplayName("Nội dung")]
        public string? Content { get; set; }


        [DisplayName("Tiêu đề")]
        public string? Name { get; set; }

        [DisplayName("Từ khoá")]
        /// Contact or Policy
        public string? KeyWord { get; set; }
        [NotMapped] public List<IFormFile> ImageUpload { get; set; }

    }
}
