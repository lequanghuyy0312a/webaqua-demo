using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static web_Aqua.Common;
using System.Data;
using web_Aqua.Context;
using X.PagedList;

namespace web_Aqua.Areas.Admin.Controllers
{
    public class ImportProductController : Controller
    {
        db_aquaponicsContext db_Context = new db_aquaponicsContext();
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ImportProductController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        //load index//
        [Area("Admin")]
        public IActionResult Index(int? page, string SearchString, string currentFilter)
        {
            LoadDropMenu();
            var listImportProduct = new List<ImportProduct>();
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
                listImportProduct = db_Context.ImportProducts.Where(n => n.Title.Contains(SearchString)).ToList();
                ViewBag.listImportProductCount = listImportProduct.Count();

            }
            else
            {
                listImportProduct = db_Context.ImportProducts.ToList();
                ViewBag.listImportProductCount = listImportProduct.Count();
            }
            ViewBag.CurrentFilter = SearchString;

            page = page < 1 ? 1 : page;
            int pagesize = 4;
            int pageNumber = (page ?? 1);
            listImportProduct = listImportProduct.OrderByDescending(n => n.ImportProductId).ToList();

            


            return View(listImportProduct.ToPagedList(pageNumber, pagesize));
        }


        /*=======================================================*/

        // START --xem 01 ImportProduct--
        [HttpGet]
        [Area("Admin")]
        public IActionResult Details(int ID)
        {
            if (ID == null)
            {
                return NotFound();
            }
            var objImportProduct = db_Context.ImportProducts.FirstOrDefault(n => n.ImportProductId == ID);
            if (objImportProduct == null)
            {
                return NotFound();
            }
            LoadDropMenu();

            return View(objImportProduct);
        }
        // END --thêm ImportProduct--

        /*=======================================================*/
        // START --xoá nhập hàng--
        [Area("Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int ID)
        {
            var objImportProduct = await db_Context.ImportProducts.FindAsync(ID);

            if (objImportProduct.Avatar != null)
            {
                var imgPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Images", "importproducts", objImportProduct.Avatar);
                if (System.IO.File.Exists(imgPath))
                    System.IO.File.Delete(imgPath);
            }

            db_Context.ImportProducts.Remove(objImportProduct);
            db_Context.SaveChanges();
            TempData["Success"] = "Đã xoá: " + "[" + objImportProduct.ImportProductId + " - " + objImportProduct.Title + "]";
            return RedirectToAction("Index");
        }
        // END --Delete ImportProduct--

        /*=======================================================*/
        // START --Add Or Edit ImportProduct--
        [Area("Admin")]
        public IActionResult AddOrEdit(int ID = 0)
        {
            LoadDropMenu();

            if (ID == 0)
            {
                return View(new ImportProduct());

            }
            else
                return View(db_Context.ImportProducts.Find(ID));
        }

        [HttpPost]
        [Area("Admin")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(List<IFormFile> files, int ImportProductID, ImportProduct objImportProduct)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string img = objImportProduct.Avatar;
            DateTime saveDatetime = objImportProduct.CreateOnUtc.GetValueOrDefault();


            var size = files.Sum(f => f.Length);
            var filePaths = new List<string>();
            foreach (var formFile in files)
            {

                string newFileName = formFile.FileName;

                
                newFileName = long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + "_" + formFile.FileName;

                var filePath = "";
                filePath = Path.Combine(contentRootPath, "wwwroot", "Images", "importproducts", newFileName);


                filePaths.Add(filePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);

                }
                if (newFileName.ToString().Trim().Length != null)
                {
                    if (img != null)
                    {
                        var imgPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Images", "importproducts", img);
                        if (System.IO.File.Exists(imgPath))
                            System.IO.File.Delete(imgPath);
                    }
                    objImportProduct.Avatar = newFileName;
                }
                else
                {
                    objImportProduct.Avatar = img;
                }

            }
         
                if (objImportProduct.ImportProductId >0)
                {
               
                // objImportProduct.CreateOnUtc = saveDatetime;
                db_Context.Entry(objImportProduct).State = EntityState.Modified;
                    TempData["Success"] = "Đã sửa thông tin:  " + "[" + objImportProduct.ImportProductId + " - " + objImportProduct.Title + "]";

                }
                else
                {
                    objImportProduct.CreateOnUtc = DateTime.Now;
                    db_Context.ImportProducts.Add(objImportProduct);
                    TempData["Success"] = "Đã thêm đơn nhập hàng:  " + "[" + objImportProduct.Title + "]";
                }

                db_Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            
        }

        // END --Add Or Editnhập hàng--

        public void LoadDropMenu()
        {
               Common objCommon = new Common();
            var listCate = db_Context.Categories.ToList();
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            //drop menu Category
            DataTable dtCategory = converter.ToDataTable(listCate);
            ViewBag.listCategory = objCommon.ToSelectList(dtCategory, "CategoryID", "Name");

            //drop menu Brand
            var listBrand = db_Context.Brands.ToList();
            DataTable dtBrand = converter.ToDataTable(listBrand);
            ViewBag.listBrand = objCommon.ToSelectList(dtBrand, "BrandID", "Company");
        }

    }

}