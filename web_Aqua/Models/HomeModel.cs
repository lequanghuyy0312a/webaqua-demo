using System.Collections.Generic;
using web_Aqua.Context;

namespace web_Aqua.Models
{
	public class HomeModel
	{


        public List<Product> listProduct { get; set; }
        public List<Product> listProductCategory { get; set; }
		public List<Category> listCategory { get; set; }
        public List<Brand> listBrand { get; set; }

        public List<Product> listProductFull { get; set; }

        public List<OrderDetail> listOrderDetail { get; set; }
        public List<OrderDetail> listOrderDetailFull { get; set; }
        public List<Order> listOrder { get; set; }

        public List<User> listUser { get; set; }
        public List<Blog> listBlog { get; set; }



		public List<Product> listProductShowPage { get; set; }
        public Blog blog { get; set; }
		public Category category { get; set; }
        public Product product { get; set; }

        internal string? ToPagedList(int pageNumber, int pagesize)
        {
            throw new NotImplementedException();
        }
    }
}
