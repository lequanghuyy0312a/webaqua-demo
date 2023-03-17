using Microsoft.AspNetCore.Mvc;

namespace web_Aqua.Controllers
{
	public class CartController : Controller
	{
		public IActionResult ShoppingCart()
		{
			return View();
		}
	}
}
