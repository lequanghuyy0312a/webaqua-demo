using Microsoft.AspNetCore.Mvc;
using web_Aqua.Context;
using web_Aqua.Models;

namespace web_Aqua.Controllers
{
	public class CategoryController : Controller
	{
		db_aquaponicsContext objdb_aquaponicsContext = new db_aquaponicsContext();

		//danh sách loại sản phẩm

		public IActionResult Index(int? ID)
		{

			HomeModel objHomeModel = new HomeModel();
			objHomeModel.listCategory = objdb_aquaponicsContext.Categories.OrderBy(x => x.Name).ToList();
			objHomeModel.listBrand = objdb_aquaponicsContext.Brands.ToList();
			objHomeModel.listProduct = objdb_aquaponicsContext.Products.ToList();



			objHomeModel.listProductCategory = (from c in objHomeModel.listCategory
												join p in objHomeModel.listProduct on c.CategoryId equals p.CategoryId
				
												join b in objHomeModel.listBrand on p.BrandId equals b.BrandID			
												select p).OrderBy(x=>x.Name).ToList();

			if (ID == null)
			{
                objHomeModel.listProductCategory = objHomeModel.listProductCategory.ToList();
            }
			else
                objHomeModel.listProductCategory = objHomeModel.listProductCategory.Where(c => c.CategoryId == ID).ToList();


            ViewBag.listProductCount = objHomeModel.listProductCategory.Count();

			return View(objHomeModel);
		}

	}
}
