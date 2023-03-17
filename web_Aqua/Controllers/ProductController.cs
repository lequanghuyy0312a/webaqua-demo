using Microsoft.AspNetCore.Mvc;
using web_Aqua.Context;

namespace web_Aqua.Controllers
{
	public class ProductController : Controller
	{
		db_aquaponicsContext objdb_aquaponisContext = new db_aquaponicsContext();
		public IActionResult Detail(int ID)
		{
			var objProduct = objdb_aquaponisContext.Products.Where(n => n.ProductId == ID).FirstOrDefault();
			return View(objProduct);
		}
	}
}
