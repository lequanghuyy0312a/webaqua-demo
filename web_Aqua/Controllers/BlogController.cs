using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Linq;
using web_Aqua.Context;
using web_Aqua.Models;
using X.PagedList;

namespace web_Aqua.Controllers
{
	public class BlogController : Controller
	{
		db_aquaponicsContext db_Context = new db_aquaponicsContext();
		private readonly IWebHostEnvironment _webHostEnvironment;


		public BlogController(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}

        public IActionResult Index(int? page, int? categoryBlog)
		{
			//var listBlog = new List<Blog>();

			//if (categoryBlog > 0)
			//{
			//	listBlog = db_Context.Blogs.Include(c => c.Category).Include(u => u.User).Where(ca => ca.CategoryID == categoryBlog).ToList();
			//	ViewBag.listBlogCount = listBlog.Count;
			//}
			//else
			//{
			//	listBlog = db_Context.Blogs.Include(c => c.Category).Include(u => u.User).ToList();
			//	ViewBag.listBlogCount = listBlog.Count;
			//}

			////page = page < 1 ? 1 : page;
			////int pagesize = 5;
			////int pageNumber = (page ?? 1);
			//listBlog = listBlog.OrderByDescending(n => n.BlogID).ToList();

			////return View(listBlog.ToPagedList(pageNumber, pagesize));
			//return View(listBlog);
			HomeModel objHomeModel = new HomeModel();
			objHomeModel.blog = db_Context.Blogs.Include(n => n.Category).Include(u => u.User).FirstOrDefault();
			objHomeModel.listBlog = db_Context.Blogs.Include(n => n.Category).Include(u => u.User).ToList();
            objHomeModel.listCategory = db_Context.Categories.ToList();

            objHomeModel.listProduct = db_Context.Products.Include(p => p.Category).ToList();

			return View(objHomeModel);
		}

		public IActionResult Detail(int? ID, int categoryID)
		{
			HomeModel objHomeModel = new HomeModel();
			objHomeModel.blog = db_Context.Blogs.Include(n => n.Category).Include(u => u.User).Where(n => n.BlogID == ID).FirstOrDefault();
            objHomeModel.listBlog = db_Context.Blogs.Include(n => n.Category).Include(u => u.User).ToList();
            objHomeModel.listCategory = db_Context.Categories.Include(n => n.Products).Include(u => u.Blogs).ToList();

            objHomeModel.listProduct = db_Context.Products.ToList();

            objHomeModel.listProductFull = (from p in objHomeModel.listProduct
                                            join c in objHomeModel.listCategory on p.CategoryId equals c.CategoryId

                                            select p).ToList();

            objHomeModel.listProduct = db_Context.Products.Include(p=>p.Category).ToList();
			if (ID > 0)
			{
				return View(objHomeModel);

			}
			else
			{
				return RedirectToAction("Index", "Blog");
			}


		}


	
	}
}
