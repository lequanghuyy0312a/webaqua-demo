
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_Aqua.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
namespace web_Aqua.Areas.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	public class ContactAndPolicyController : Controller
    {
        db_aquaponicsContext db_Context = new db_aquaponicsContext();
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ContactAndPolicyController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [Area("Admin")]
        public IActionResult Index()
        {
            var list = db_Context.ContactAndPolicies.ToList();
            return View(list);
        }

        [Area("Admin")]
        public IActionResult AddOrEdit(int ID = 0)
        {
         

            if (ID == 0)
            {
                return View(new ContactAndPolicy());

            }
            else
                return View(db_Context.ContactAndPolicies.Find(ID));
        }

        [Area("Admin")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int ContactAndPolicyID, ContactAndPolicy objContactAndPolicy)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;

            
            //try
            //{
                if (objContactAndPolicy.Id != 0)
                {
                    //  objContactAndPolicy.CreateOnUtc = saveDatetime;
                    db_Context.Entry(objContactAndPolicy).State = EntityState.Modified;
                    TempData["Success"] = "Đã sửa thông tin : " + "[" + objContactAndPolicy.Name  + "]";

                }
                else
                {
                  
                    db_Context.ContactAndPolicies.Add(objContactAndPolicy);
                    TempData["Success"] = "Đã thêm : " + "[" + objContactAndPolicy.Name  + "]";
                }

                db_Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            //}

            //catch (Exception ex)
            //{
            //    TempData["Success"] = "Kiểm tra lại thao tác !";
            //    return RedirectToAction("AddOrEdit");
            //}
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

                filePath1 = Path.Combine(contentRootPath1, "wwwroot", "Images", "contact", newFileName1);
                filePath1s.Add(filePath1);

                using (var stream = new FileStream(filePath1, FileMode.Create))
                {
                    formFile1.CopyTo(stream);

                }
                returnImagePath1 = filePath1;
            }
            return Json("/images/contact/" + newFileName1);
        }




        [Area("Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int ID)
        {
            var objCAP = await db_Context.ContactAndPolicies.FindAsync(ID);
           

            db_Context.ContactAndPolicies.Remove(objCAP);
            db_Context.SaveChanges();
            TempData["Success"] = "Đã xoá : " + "[" + objCAP.Name +  "]";
            return RedirectToAction("Index");
        }
    }
}
