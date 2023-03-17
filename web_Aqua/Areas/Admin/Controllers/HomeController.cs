using Microsoft.AspNetCore.Mvc;

namespace web_Aqua.Areas.Admin.Controllers
{

	public class HomeController : Controller
	{
		[Area("Admin")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
