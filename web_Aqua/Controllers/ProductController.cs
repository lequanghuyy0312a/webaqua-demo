using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_Aqua.Context;
using web_Aqua.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace web_Aqua.Controllers
{
	
	public class ProductController : Controller
	{
		db_aquaponicsContext db_Context = new db_aquaponicsContext();
		private readonly IHttpContextAccessor _contextAccessor;
		public ProductController(IHttpContextAccessor httpContextAccessor)
		{
			_contextAccessor = httpContextAccessor;
		}
		public IActionResult Index(int? ID)
		{

			HomeModel objHomeModel = new HomeModel();
			objHomeModel.listCategory = db_Context.Categories.ToList();
			objHomeModel.listBrand = db_Context.Brands.ToList();
			objHomeModel.listProduct = db_Context.Products.ToList();

			objHomeModel.listProductFull = (from p in objHomeModel.listProduct
											join c in objHomeModel.listCategory on p.CategoryId equals c.CategoryId
											where p.CategoryId == c.CategoryId

											join b in objHomeModel.listBrand on p.BrandId equals b.BrandID
											where p.BrandId == b.BrandID
											select p).ToList();
			if(ID != null)
			{
				objHomeModel.listProductFull = objHomeModel.listProductFull.Where(p => p.CategoryId == ID).ToList();
				objHomeModel.category = db_Context.Categories.Where(c => c.CategoryId == ID).FirstOrDefault();
				ViewBag.categoryName = objHomeModel.category.Name;
			}
			ViewBag.CountProduct = objHomeModel.listProductFull.Count;

			return View(objHomeModel);

		}
		public IActionResult Detail(int ID)
		{
			HomeModel objHomeModel = new HomeModel();

			objHomeModel.listBlog = db_Context.Blogs.Include(n => n.Category).Include(u => u.User).ToList();
            objHomeModel.product = db_Context.Products.Include(p=>p.Category).Include(b=>b.Brand).Where(n => n.ProductId == ID).FirstOrDefault();
            objHomeModel.listCategory = db_Context.Categories.Include(n => n.Products).Include(u => u.Blogs).ToList();
            if (ID > 0)
            {
                return View(objHomeModel);

            }
            else
            {
                return RedirectToAction("Index", "Product");
            }
		}
	
	}
}
