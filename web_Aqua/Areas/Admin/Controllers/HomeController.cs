using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using web_Aqua.Context;
using web_Aqua.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace web_Aqua.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {
        db_aquaponicsContext db_Context = new db_aquaponicsContext();

        [Area("Admin")]
        public IActionResult Index()
        {

            HomeModel objHomeModel = new HomeModel();

            objHomeModel.listOrder = db_Context.Orders.ToList();



            var listOrder1 = db_Context.Orders.Where(o => o.CreatedOnUtc > DateTime.Today).ToList();
            ViewBag.today = listOrder1.Count;


			var listOrder2 = db_Context.Orders.Where(o=>o.CreatedOnUtc.Value.Date == DateTime.Today.AddDays(-1).Date).ToList();
            ViewBag.yesterday = listOrder2.Count;

            var listOrder3 = db_Context.Orders.Where(o => o.CreatedOnUtc.Value.Date > DateTime.Today.AddDays(-30).Date).ToList();
            ViewBag.month = listOrder3.Count;




            objHomeModel.listProduct = db_Context.Products.ToList();
            objHomeModel.listCategory = db_Context.Categories.ToList();
            objHomeModel.listUser = db_Context.Users.ToList();
            objHomeModel.listBlog = db_Context.Blogs.ToList();



            objHomeModel.listOrder = db_Context.Orders.Include(o => o.User).ToList();


            return View(objHomeModel);
        }

	}
}
