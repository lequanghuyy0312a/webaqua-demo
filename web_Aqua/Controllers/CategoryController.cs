using Microsoft.AspNetCore.Mvc;
using web_Aqua.Context;
using web_Aqua.Models;

namespace web_Aqua.Controllers
{
	public class CategoryController : Controller
	{
		db_aquaponicsContext objdb_aquaponicsContext = new db_aquaponicsContext();

		//danh sách loại sản phẩm
		public IActionResult Index()
		{
			var listCategory = objdb_aquaponicsContext.Categories.ToList();	
			return View(listCategory);
		}
		//danh sách sản phẩm theo loại
		public IActionResult ProductCategory(int ID)
		{

			HomeModel objHomeModel = new HomeModel();
			objHomeModel.listCategory = objdb_aquaponicsContext.Categories.ToList();


			objHomeModel.listProductCategory = objdb_aquaponicsContext.Products.Where(n => n.CategoryId == ID).ToList();


			return View(objHomeModel);
		}
	}
}
