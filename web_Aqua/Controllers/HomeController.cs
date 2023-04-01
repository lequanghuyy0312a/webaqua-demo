using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using web_Aqua.Context;
using web_Aqua.Models;

using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace web_Aqua.Controllers
{

	public class HomeController : Controller
	{


		db_aquaponicsContext db_Context = new db_aquaponicsContext();


		private readonly IHttpContextAccessor _contextAccessor;

		public HomeController(IHttpContextAccessor httpContextAccessor)
		{
			_contextAccessor = httpContextAccessor;
		}



		//hiển thị data lên trang chủ từ sql server
		public IActionResult Index(int ID)
		{
			HomeModel objHomeModel = new HomeModel();
			objHomeModel.listCategory = db_Context.Categories.OrderBy(n => n.Name).ToList();
			objHomeModel.listProductCategory = db_Context.Products.Where(n => n.CategoryId == ID).ToList();
			objHomeModel.listProduct = db_Context.Products.ToList();
			objHomeModel.listBlog = db_Context.Blogs.Include(n => n.Category).Include(u => u.User).ToList();
			objHomeModel.user = db_Context.Users.Where(u => u.UserId == _contextAccessor.HttpContext.Session.GetInt32("UserId")).FirstOrDefault();

			ViewBag.Image = db_Context.Users.Where(u => u.UserId == _contextAccessor.HttpContext.Session.GetInt32("UserId")).Select(u => u.Avatar).FirstOrDefault();

			if (objHomeModel.user != null)
			{
				ViewBag.IsAdmin = objHomeModel.user.IsAdmin;
			}
			return View(objHomeModel);
		}


		//get -- đăng ký
		[HttpGet]
		public IActionResult Register()
		{

			ClaimsPrincipal claimsPrincipal = HttpContext.User;
			if (claimsPrincipal.Identity.IsAuthenticated == false && _contextAccessor.HttpContext.Session.GetInt32("UserId") < 1)
				return View();
			else
			{
				Logout();
				return View();
			}



		}
		//post -- đăng ký
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Register(User _user)
		{
			try
			{
				var check = db_Context.Users.Where(s => s.Email == _user.Email).FirstOrDefault();
				if (check == null)
				{
					_user.Password = GetMD5(_user.Password);
					//  db_Context.ConfigureAwait(false);
					_user.IsAdmin = false;
					db_Context.Users.Add(_user);
					db_Context.SaveChanges();

					return RedirectToAction("Login");
				}
				else
				{
					TempData["Error"] = "Gmail đã tồn tại.";
					return View();
				}
			}
			catch
			{
				return View();
			}
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

		//get -- đăng nhập

		[HttpGet]
		public ActionResult Login()
		{
			ClaimsPrincipal claimsPrincipal = HttpContext.User;
			if (claimsPrincipal.Identity.IsAuthenticated && _contextAccessor.HttpContext.Session.GetInt32("UserId")>0)
				return RedirectToAction("Index", "Home");

			return View();
		}
		// post -- đăng nhập
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(string email, string password)
		{
			if (_contextAccessor.HttpContext.Session.GetInt32("UserId") == null)
			{
				if (ModelState.IsValid)
				{
					User user = db_Context.Users.SingleOrDefault(x => x.Email.ToLower() == email.ToLower().Trim());


					var f_password = GetMD5(password);
					var data = db_Context.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();



					if (data.Count() > 0)
					{
						//add session
						_contextAccessor.HttpContext.Session.SetString("FirstName", data.FirstOrDefault().FirstName);
						_contextAccessor.HttpContext.Session.SetString("LastName", data.FirstOrDefault().LastName);

						_contextAccessor.HttpContext.Session.SetString("Email", data.FirstOrDefault().Email);
						_contextAccessor.HttpContext.Session.SetInt32("UserId", data.FirstOrDefault().UserId);
						string role = "";
						if (user.IsAdmin == true)
							role = "Admin";
						else
							role = "User";

						var userClaims = new List<Claim>
						{

							new Claim(ClaimTypes.Name, user.LastName),
							new Claim(ClaimTypes.Email, user.Email),
							new Claim("UserId", user.UserId.ToString()),
							new Claim(ClaimTypes.Role, role),
						};

						ClaimsIdentity claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
						AuthenticationProperties properties = new AuthenticationProperties()
						{
							AllowRefresh = true
						};
						await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);


						return RedirectToAction("Index");
					}
					else
					{
						TempData["Error"] = "Đăng nhập thất bại";
						return RedirectToAction("Login");
					}
				}
				return View();
			}
			else
			{
				return RedirectToAction("Index");
			}

		}

		// đăng xuất
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			_contextAccessor.HttpContext.Session.Clear();
			return RedirectToAction("Login");
		}

		

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

	}
}