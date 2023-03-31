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
	public class BrandController : Controller
	{    

        db_aquaponicsContext db_Context = new db_aquaponicsContext();
        private readonly IWebHostEnvironment _webHostEnvironment;


        public BrandController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [Area("Admin")]
        public IActionResult Index(int? page, string SearchString, string currentFilter)
        {
            var listBrand = new List<Brand>();
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
                listBrand = db_Context.Brands.Where(n => n.Company.Contains(SearchString)).ToList();
                ViewBag.listBrandCount = db_Context.Brands.Where(n => n.Company.Contains(SearchString)).ToList().Count();

            }
            else
            {
                listBrand = db_Context.Brands.ToList();
                ViewBag.listBrandCount = db_Context.Brands.ToList().Count();
            }

            ViewBag.CurrentFilter = SearchString;

            page = page < 1 ? 1 : page;
            int pagesize = 4;
            int pageNumber = (page ?? 1);
            listBrand = listBrand.OrderByDescending(n => n.BrandID).ToList();


            return View(listBrand.ToPagedList(pageNumber, pagesize));
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
            var objBrand = db_Context.Brands.FirstOrDefault(n => n.BrandID == ID);
            if (objBrand == null)
            {
                return NotFound();
            }
            LoadDropMenu();

            return View(objBrand);
        }
        // END --thêm sp--

        /*=======================================================*/
        // START --xoá sản phẩm--
        [Area("Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int ID)
        {
            var objBrand = await db_Context.Brands.FindAsync(ID);

            if (objBrand.Avatar != null)
            {
                var imgPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Images", "brands", objBrand.Avatar);
                if (System.IO.File.Exists(imgPath))
                    System.IO.File.Delete(imgPath);
            }

            db_Context.Brands.Remove(objBrand);
            db_Context.SaveChanges();
            TempData["Success"] = "Đã xoá đối tác: " + "[ " + objBrand.Company + " - " + objBrand.Nation + " ]";

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
                return View(new Brand());

            }
            else
                return View(db_Context.Brands.Find(ID));
        }

        [HttpPost]
        [Area("Admin")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(List<IFormFile> files, int BrandID, Brand objBrand)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string img = objBrand.Avatar;


            var size = files.Sum(f => f.Length);
            var filePaths = new List<string>();
            foreach (var formFile in files)
            {

                string newFileName = formFile.FileName;


                newFileName = long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + "_" + formFile.FileName;

                var filePath = "";
                filePath = Path.Combine(contentRootPath, "wwwroot", "Images", "brands", newFileName);


                filePaths.Add(filePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);

                }
                if (newFileName.ToString().Trim().Length != null)
                {
                    if (img != null)
                    {
                        var imgPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Images", "brands", img);
                        if (System.IO.File.Exists(imgPath))
                            System.IO.File.Delete(imgPath);
                    }
                    objBrand.Avatar = newFileName;
                }
                else
                {
                    objBrand.Avatar = img;
                }

            }
            try
            {
                if (objBrand.BrandID != 0)
                {
                    db_Context.Entry(objBrand).State = EntityState.Modified;
                    TempData["Success"] = "Đã sửa thông tin đối tác: " + "[ " + objBrand.Company + " - " + objBrand.Nation + " ]";

                }
                else
                {
                    db_Context.Brands.Add(objBrand);
                    TempData["Success"] = "Đã thêm đối tác: " + "[ " + objBrand.Company + " - " + objBrand.Nation + " ]";

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
            List<Show> listBrandShow = new List<Show>();
            Show objBrandShow = new Show();
            objBrandShow.ID = true;
            objBrandShow.Name = "Có";
            listBrandShow.Add(objBrandShow);

            objBrandShow = new Show();
            objBrandShow.ID = false;
            objBrandShow.Name = "Không";
            listBrandShow.Add(objBrandShow);

            DataTable dtBrandShow = converter.ToDataTable(listBrandShow);
            ViewBag.listBrandShow = objCommon.ToSelectList(dtBrandShow, "ID", "Name");
         

        }

    }

}
