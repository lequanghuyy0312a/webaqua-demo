
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using System.Data;
using web_Aqua.Context;
using X.PagedList;
using static web_Aqua.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace web_Aqua.Areas.Admin.Controllers
{
	[Authorize(Roles = "Admin")]

	public class BlogsController : Controller
    {
        db_aquaponicsContext db_Context = new db_aquaponicsContext();
        private readonly IWebHostEnvironment _webHostEnvironment;


        public BlogsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [Area("Admin")]
        public IActionResult Index(int? page, string SearchString, string currentFilter)
        {
            var listBlog = new List<Blog>();
            LoadDropMenu();
            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            if (!string.IsNullOrEmpty(SearchString))
            {
                listBlog = db_Context.Blogs.Include(c => c.Category).OrderByDescending(n => n.BlogID).Where(n => n.Title.Contains(SearchString)).ToList();
                ViewBag.listBlogCount = listBlog.Count;
            }
            else
            {
                listBlog = db_Context.Blogs.Include(c => c.Category).OrderByDescending(n => n.BlogID).ToList();

                //listProduct = db_Context.Products.ToList();
                ViewBag.listBlogCount = listBlog.Count;

            }

                ViewBag.CurrentFilter = SearchString;
             

                page = page < 1 ? 1 : page;
                int pagesize = 5;
                int pageNumber = (page ?? 1);
                listBlog = listBlog.OrderByDescending(n => n.BlogID).ToList();


                return View(listBlog.ToPagedList(pageNumber, pagesize));

            }



        // START --xoá--
        [Area("Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int ID)
        {
            var objBlog = await db_Context.Blogs.FindAsync(ID);

            db_Context.Blogs.Remove(objBlog);
            db_Context.SaveChanges();
            TempData["Success"] = "Đã xoá Bài viết: " + "[" + objBlog.BlogID + " - " + objBlog.Title + "]";
            return RedirectToAction("Index");
        }
        // START --Thêm hoặc sửa--
        // get
        [Area("Admin")]
        public IActionResult AddOrEdit(int ID = 0)
        {
            LoadDropMenu();
            if (ID == 0)
            {
                return View(new Blog());

            }
            else
                return View(db_Context.Blogs.Find(ID));
        }
        //post
        [HttpPost]
        [Area("Admin")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(IFormFile? files, int BlogID, Blog objBlog, string setchange)
        {

            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string img = objBlog.Thumbnail;
            string newFileName = "";

            try
            {
                    newFileName = long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + "_" + files.FileName;

                    var filePath = "";
                    filePath = Path.Combine(contentRootPath, "wwwroot", "Images", "Thumbnails", newFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await files.CopyToAsync(stream);

                    }
                    objBlog.Thumbnail = newFileName;


                if (newFileName.ToString().Trim().Length > 0)
                {
                    objBlog.Thumbnail = newFileName;
                }
                else
                {
                    objBlog.Thumbnail = img;
                }
            }

            catch (Exception ex)
            {
            }
            if (objBlog.BlogID != 0)
            {
                //  objBlog.CreateOnUtc = saveDatetime;
                db_Context.Entry(objBlog).State = EntityState.Modified;
                TempData["Success"] = "Đã sửa thông tin bài viết: " + "[" + objBlog.BlogID + " - " + objBlog.Title + "]";

            }
            else
            {
                objBlog.CreateOnUtc = DateTime.Now;
                db_Context.Blogs.Add(objBlog);
                TempData["Success"] = "Đã thêm bài viết: " + "[ "+objBlog.Title+" ]";
            }

            db_Context.SaveChanges();
            return RedirectToAction(nameof(Index));         

        }



        [HttpPost]
        [Area("Admin")]
        [Route("/imageupload", Name = "imageupload")]
        public JsonResult UploadFile(List<IFormFile> uploadedFiles)
        {
            string webRootPath1 = _webHostEnvironment.WebRootPath;
            string contentRootPath1 = _webHostEnvironment.ContentRootPath;

            string returnImagePath1 = string.Empty;
            string fileName1;


            var filePath1s = new List<string>();
            string newFileName1 = "";

            foreach (var formFile1 in uploadedFiles)
            {
                var filePath1 = "";
                newFileName1 = long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + "_" + formFile1.FileName;

                filePath1 = Path.Combine(contentRootPath1, "wwwroot", "Images", "blogs", newFileName1);
                filePath1s.Add(filePath1);

                using (var stream = new FileStream(filePath1, FileMode.Create))
                {
                    formFile1.CopyTo(stream);

                }
                returnImagePath1 = filePath1;
            }
            return Json("/images/blogs/"+ newFileName1);
        }
        public void LoadDropMenu()
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            Common objCommon = new Common();


            //drop menu ẩn:0 / hiện:1
            List<Show> listBlogShow = new List<Show>();
            Show objBlogShow = new Show();
            objBlogShow.ID = true;
            objBlogShow.Name = "Có";
            listBlogShow.Add(objBlogShow);

            objBlogShow = new Show();
            objBlogShow.ID = false;
            objBlogShow.Name = "Không";
            listBlogShow.Add(objBlogShow);

            DataTable dtBlogShow = converter.ToDataTable(listBlogShow);
            ViewBag.listBlogShow = objCommon.ToSelectList(dtBlogShow, "ID", "Name");

            //drop khu vực 1: trong nước / 0: quốc tế
            List<Show> listBlogZone = new List<Show>();
            Show objBlogZone = new Show();
            objBlogZone.ID = true;
            objBlogZone.Name = "Trong nước";
            listBlogZone.Add(objBlogZone);

            objBlogZone = new Show();
            objBlogZone.ID = false;
            objBlogZone.Name = "Quốc tế";
            listBlogZone.Add(objBlogZone);

            DataTable dtBlogZone = converter.ToDataTable(listBlogZone);
            ViewBag.listBlogZone = objCommon.ToSelectList(dtBlogZone, "ID", "Name");

            //drop menu Category
            var listCate = db_Context.Categories.ToList(); 

            DataTable dtCategory = converter.ToDataTable(listCate);
            ViewBag.listCategory = objCommon.ToSelectList(dtCategory, "CategoryID", "Name");

        }
    }
}
