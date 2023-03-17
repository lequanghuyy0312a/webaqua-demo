using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using web_Aqua.Context;
using web_Aqua.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace web_Aqua.Controllers
{
   
    public class HomeController : Controller
    {


        db_aquaponicsContext objdb_aquaponicsContext = new db_aquaponicsContext();

       // private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }


        //hiển thị data lên trang chủ từ sql server
        public IActionResult Index(int ID)
        {
            HomeModel objHomeModel = new HomeModel();
			objHomeModel.listCategory = objdb_aquaponicsContext.Categories.ToList();

			objHomeModel.listProductCategory = objdb_aquaponicsContext.Products.Where(n => n.CategoryId == ID).ToList();

			objHomeModel.listProduct = objdb_aquaponicsContext.Products.ToList();

			return View(objHomeModel);
		}


        [HttpGet]

     //   [Route("Register")]

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User _user)
        {
            if (ModelState.IsValid)
            {
                var check = objdb_aquaponicsContext.Users.FirstOrDefault(s => s.Email == _user.Email);
                if (check == null)
                {
                    _user.Password = GetMD5(_user.Password);
                    objdb_aquaponicsContext.ConfigureAwait(false);
                    objdb_aquaponicsContext.Users.Add(_user);
                    objdb_aquaponicsContext.SaveChanges();

					//add session
					_contextAccessor.HttpContext.Session.SetString("FullName", _user.FirstName + " " + _user.LastName);
					_contextAccessor.HttpContext.Session.SetString("Email", _user.Email);
					_contextAccessor.HttpContext.Session.SetInt32("UserId", _user.UserId);

					return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Đăng ký thất bại";

                    return View();
                }
            }
            return View();
        }
        //tạo mã hoá MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

        //đăng nhập

        [HttpGet]
       // [Route("Login")]

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {


                var f_password = GetMD5(password);
                var data = objdb_aquaponicsContext.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    _contextAccessor.HttpContext.Session.SetString("FullName", data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName);
                    _contextAccessor.HttpContext.Session.SetString("Email", data.FirstOrDefault().Email);
                    _contextAccessor.HttpContext.Session.SetInt32("UserId", data.FirstOrDefault().UserId);

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Đăng nhập thất bại";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        public IActionResult Logout()
        {
			_contextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


	}
}