using Microsoft.AspNetCore.Mvc;

namespace web_Aqua.Controllers
{
	public class BlogController : Controller
	{
		public IActionResult Blogs()
		{
			return View();
		}
	}
}
