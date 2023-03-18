using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Drawing;
using web_Aqua.Context;

namespace web_Aqua.Areas.Admin.Controllers
{

    public class ProductController : Controller
	{

        db_aquaponicsContext objdb_aquaponisContext = new db_aquaponicsContext();
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [Area("Admin")]
        public IActionResult Index()
		{
			var listProduct = objdb_aquaponisContext.Products.ToList();	
			return View(listProduct);         
		}

		
		[HttpGet]
        [Area("Admin")]
        public IActionResult Create()
		{

			return View();
		}


        [HttpPost("Product")]
        [Area("Admin")]
        [Route("Admin")]
        public async Task<IActionResult> Create(List<IFormFile> files, Product objProduct)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;

            if (objProduct.ImageUpload == null && objProduct.Name == null)
            {
                return View("Create");
            }
            else
            {
                var size = files.Sum(f => f.Length);
                var filePaths = new List<string>();
                foreach (var formFile in files)
                {
                    string newFileName = formFile.FileName;

                    newFileName = long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + "_" + formFile.FileName;

                    var filePath = "";
                    filePath = Path.Combine(contentRootPath, "wwwroot", "Images", "avatars", newFileName);

                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);

                    }
                    objProduct.Avatar = newFileName;
                }
                objdb_aquaponisContext.Products.Add(objProduct);
                objdb_aquaponisContext.SaveChanges();
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        [Area("Admin")]
        public IActionResult Details(int ID)
        {
            var objProduct = objdb_aquaponisContext.Products.Where (n=>n.ProductId ==  ID).FirstOrDefault();
            return View(objProduct);
        }

    }    

    }
