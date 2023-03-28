using Microsoft.AspNetCore.Mvc;
using web_Aqua.Context;
using web_Aqua.Models;

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
