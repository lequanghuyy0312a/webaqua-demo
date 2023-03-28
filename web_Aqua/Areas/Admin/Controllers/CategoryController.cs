using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using web_Aqua.Context;
using web_Aqua.Models;
using X.PagedList;
using static web_Aqua.Common;


namespace web_Aqua.Areas.Admin.Controllers
{
	public class CategoryController : Controller
	{

        db_aquaponicsContext db_Context = new db_aquaponicsContext();
        private readonly IWebHostEnvironment _webHostEnvironment;


        public CategoryController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [Area("Admin")]
        public IActionResult Index(int? page, string SearchString, string currentFilter)
        {
            var listCategory = new List<Category>();
            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            if (!string.IsNullOrEmpty(SearchString) )
            {
                listCategory = db_Context.Categories .Where(n => n.Name.Contains(SearchString)).ToList();
                ViewBag.listCategoryCount = db_Context.Categories.Where(n => n.Name.Contains(SearchString)).ToList().Count();

            }
            else
            {
                listCategory = db_Context.Categories.ToList();
                ViewBag.listCategoryCount = db_Context.Categories.ToList().Count();
            }

            ViewBag.CurrentFilter = SearchString;

            page = page < 1 ? 1 : page;
            int pagesize = 4;
            int pageNumber = (page ?? 1);
            listCategory = listCategory.OrderByDescending(n => n.CategoryId).ToList();


            return View(listCategory.ToPagedList(pageNumber, pagesize));
        }


        /*=======================================================*/

        // START --xem sp--
        [HttpGet]
        [Area("Admin")]
        public IActionResult Details(int ID)
        {
            if (ID == null)
            {
                return NotFound();
            }
            var objCategory = db_Context.Categories.FirstOrDefault(n => n.CategoryId == ID);
            if (objCategory == null)
            {
                return NotFound();
            }
            LoadDropMenu();

            return View(objCategory);
        }
        // END --thêm sp--

        /*=======================================================*/
        // START --xoá sản phẩm--
        [Area("Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var objCategory = await db_Context.Categories.FindAsync(id);

      
                if (objCategory.Avatar != null) {
                    var imgPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Images", "categories", objCategory.Avatar);
                    if (System.IO.File.Exists(imgPath))
                        System.IO.File.Delete(imgPath);
                }
                

                db_Context.Categories.Remove(objCategory);
                db_Context.SaveChanges();
                TempData["Success"] = "Đã xoá danh mục: " + "[ " + objCategory.CategoryId + " - " + objCategory.Name + " ]";
            
    
          
            return RedirectToAction("Index");
        }
        // END --Delete sản phẩm--

        /*=======================================================*/
        // START --Add Or Edit sản phẩm--
        [Area("Admin")]
        public IActionResult AddOrEdit(int ID = 0)
        {
            LoadDropMenu();

            if (ID == 0)
            {
                return View(new Category());

            }
            else
                return View(db_Context.Categories.Find(ID));
        }

        [HttpPost]
        [Area("Admin")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(List<IFormFile> files, int categoryID, Category objCategory)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string img = objCategory.Avatar;


            var size = files.Sum(f => f.Length);
            var filePaths = new List<string>();
            foreach (var formFile in files)
            {

                string newFileName = formFile.FileName;


                newFileName = long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + "_" + formFile.FileName;

                var filePath = "";
                filePath = Path.Combine(contentRootPath, "wwwroot", "Images", "categories", newFileName);


                filePaths.Add(filePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);

                }
                if (newFileName.ToString().Trim().Length != null)
                {
                    if (img != null)
                    {
                        var imgPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Images", "categories", img);
                        if (System.IO.File.Exists(imgPath))
                            System.IO.File.Delete(imgPath);
                    }

                    objCategory.Avatar = newFileName;
                }
                else
                {
                    objCategory.Avatar = img;
                }

            }
            try
            {
                if (objCategory.CategoryId != 0)
                {
                     
                    db_Context.Entry(objCategory).State = EntityState.Modified;
                    TempData["Success"] = "Đã sửa thông tin danh mục: " + "[ " + objCategory.CategoryId + " - " + objCategory.Name + " ]";
                }
                else
                {

                    db_Context.Categories.Add(objCategory);
                    TempData["Success"] = "Đã thêm danh mục: " + "[ " + objCategory.CategoryId +" - "+ objCategory.Name + " ]";
                }

                db_Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            catch (Exception ex)
            {
                TempData["Error"] = "Kiểm tra lại thao tác";
                return RedirectToAction("AddOrEdit");
            }
        }

        // END --Add Or Edit sản phẩm--

        public void LoadDropMenu()
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            Common objCommon = new Common();
            

            //drop menu ẩn:0 / hiện:1
            List<Show> listCategoryShow = new List<Show>();
            Show objCategoryShow = new Show();
            objCategoryShow.ID = true;
            objCategoryShow.Name = "Có";
            listCategoryShow.Add(objCategoryShow);

            objCategoryShow = new Show();
            objCategoryShow.ID = false;
            objCategoryShow.Name = "Không";
            listCategoryShow.Add(objCategoryShow);

            DataTable dtCategoryShow = converter.ToDataTable(listCategoryShow);
            ViewBag.listCategoryShow = objCommon.ToSelectList(dtCategoryShow, "ID", "Name");

            //drop menu hiển thị lên website, có: 1, không: 0
            List<Show> listCategoryFor = new List<Show>();
            Show objCategoryFor = new Show();
            objCategoryFor.ID = true;
            objCategoryFor.Name = "Cây Trồng";
            listCategoryFor.Add(objCategoryFor);

            objCategoryFor = new Show();
            objCategoryFor.ID = false;
            objCategoryFor.Name = "Nhà lưới";
            listCategoryFor.Add(objCategoryFor);

            DataTable dtCategoryFor = converter.ToDataTable(listCategoryFor);
            ViewBag.listCategoryFor = objCommon.ToSelectList(dtCategoryFor, "ID", "Name");


        }

    }

}
