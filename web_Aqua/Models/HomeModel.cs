using System.Collections.Generic;
using web_Aqua.Context;

namespace web_Aqua.Models
{
	public class HomeModel
	{
		public List<Product> listProduct { get; set; }
		public List<Product> listProductCategory { get; set; }
		public List<Category> listCategory { get; set; }

	}
}
