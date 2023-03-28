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

    public class ProductController : Controller
    {

        db_aquaponicsContext db_Context = new db_aquaponicsContext();
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }
        [Area("Admin")]
        public IActionResult Index(int? page, string SearchString, string currentFilter)
        {

            var listProduct = new List<Product>();
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
                listProduct = db_Context.Products.Include(c => c.Category).Include(b => b.Brand).Where(n => n.Name.Contains(SearchString)).ToList();
                ViewBag.listProductCount = listProduct.Count;

            }         
            else
            {
                listProduct = db_Context.Products.Include(c => c.Category).Include(b => b.Brand).ToList();

               //listProduct = db_Context.Products.ToList();
                ViewBag.listProductCount = listProduct.Count;

            }

            ViewBag.CurrentFilter = SearchString;
           

            page = page < 1 ? 1 : page;
            int pagesize = 5;
            int pageNumber = (page ?? 1);
            listProduct = listProduct.OrderByDescending(n => n.ProductId).ToList();


            return View(listProduct.ToPagedList(pageNumber, pagesize));
        }


        // START --xem sp--
        [HttpGet]
        [Area("Admin")]
        public IActionResult Details(int ID)
        {
            if (ID == null)
            {
                return NotFound();
            }
            var objProduct = db_Context.Products.FirstOrDefault(n => n.ProductId == ID);
            if (objProduct == null)
            {
                return NotFound();
            }
            LoadDropMenu();
            return View(objProduct);
        }
        // END --thêm sp--

        /*=======================================================*/
        // START --xoá sản phẩm--
        [Area("Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var objProduct = await db_Context.Products.FindAsync(id);
            if (objProduct.Avatar != null)
            {
                var imgPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Images", "items", objProduct.Avatar);
                if (System.IO.File.Exists(imgPath))
                    System.IO.File.Delete(imgPath);
            }

            db_Context.Products.Remove(objProduct);
            db_Context.SaveChanges();
            TempData["Success"] = "Đã xoá sản phẩm: " + "[ " + objProduct.ProductId + " - " + objProduct.Name + " ]";

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
                return View(new Product());

            }
            else
                return View(db_Context.Products.Find(ID));
        }

        [HttpPost]
        [Area("Admin")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(List<IFormFile> files, int productID, Product objProduct)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string img = objProduct.Avatar;


            var size = files.Sum(f => f.Length);
            var filePaths = new List<string>();
            foreach (var formFile in files)
            {

                string newFileName = formFile.FileName;


                newFileName = long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + "_" + formFile.FileName;

                var filePath = "";
                filePath = Path.Combine(contentRootPath, "wwwroot", "Images", "items", newFileName);


                filePaths.Add(filePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);

                }
                if (newFileName.ToString().Trim().Length != null)
                {
                    if (img != null)
                    {
                        var imgPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Images", "items", img);
                        if (System.IO.File.Exists(imgPath))
                            System.IO.File.Delete(imgPath);
                    }
                    objProduct.Avatar = newFileName;
                }
                else
                {
                    objProduct.Avatar = img;
                }

            }
            try
            {
                if (objProduct.ProductId != 0)
                {
                    db_Context.Entry(objProduct).State = EntityState.Modified;
                    TempData["Success"] = "Đã sửa thông tin sản phẩm: " + "[ " + objProduct.ProductId + " - " + objProduct.Name + " ]";

                }
                else
                {
                    db_Context.Products.Add(objProduct);
                    TempData["Success"] = "Đã thêm sản phẩm: " + "[ " + objProduct.ProductId + " - " + objProduct.Name + " ]";
                }

                    db_Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            catch (Exception ex)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thao tác !";
                return View();
            }
        }

        // END --Add Or Edit sản phẩm--


        /*=======================================================*/
        // START Load Drop
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

            //drop menu đề xuất:0 / giảm giá:1
            List<Show> listProductType = new List<Show>();
            Show objProductType = new Show();
            objProductType.ID = true;
            objProductType.Name = "Giảm giá sốc";
            listProductType.Add(objProductType);

            objProductType = new Show();
            objProductType.ID = false;
            objProductType.Name = "Đề xuất";
            listProductType.Add(objProductType);

            DataTable dtProductType = converter.ToDataTable(listProductType);
            ViewBag.ProductType = objCommon.ToSelectList(dtProductType, "ID", "Name");

            //drop menu hiển thị lên website, có: 1, không: 0
            List<Show> listProductShow = new List<Show>();
            Show objProductShow = new Show();
            objProductShow.ID = true;
            objProductShow.Name = "Có";
            listProductShow.Add(objProductShow);

            objProductShow = new Show();
            objProductShow.ID = false;
            objProductShow.Name = "Không";
            listProductShow.Add(objProductShow);

            DataTable dtProductShow = converter.ToDataTable(listProductShow);
            ViewBag.ProductShow = objCommon.ToSelectList(dtProductShow, "ID", "Name");


        }

        [Route("/searchwithcate", Name = "searchwithcate")]      
        [HttpPost]
        public IActionResult SearchCategory(int cate)
        {
            var listProductWithCate = db_Context.Products.Where(n => n.CategoryId == cate).ToList();

            return Ok(listProductWithCate);
        }
    }
    

}
