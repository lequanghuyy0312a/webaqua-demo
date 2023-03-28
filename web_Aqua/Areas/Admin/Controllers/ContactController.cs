using Microsoft.AspNetCore.Mvc;

namespace web_Aqua.Areas.Admin.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
