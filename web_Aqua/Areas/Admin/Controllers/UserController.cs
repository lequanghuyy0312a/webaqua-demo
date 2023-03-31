using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using web_Aqua.Context;
using web_Aqua.Models;
using X.PagedList;
using static web_Aqua.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;


namespace web_Aqua.Areas.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	public class UserController : Controller
    {

        db_aquaponicsContext db_Context = new db_aquaponicsContext();
        private readonly IWebHostEnvironment _webHostEnvironment;


        public UserController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        //load index//
        [Area("Admin")]
        public IActionResult Index(int? page, string SearchString, string currentFilter)
        {
            var listUser = new List<User>();
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
                listUser = db_Context.Users.Where(n => n.Email.Contains(SearchString)).ToList();
                ViewBag.listUserCount = db_Context.Users.Where(n => n.Email.Contains(SearchString)).ToList().Count();

            }
            else
            {
                listUser = db_Context.Users.ToList();
                ViewBag.listUserCount = db_Context.Users.ToList().Count();
            }

            ViewBag.CurrentFilter = SearchString;

            page = page < 1 ? 1 : page;
            int pagesize = 4;
            int pageNumber = (page ?? 1);
            listUser = listUser.OrderByDescending(n => n.UserId).ToList();


            return View(listUser.ToPagedList(pageNumber, pagesize));
        }


        /*=======================================================*/

        // START --xem 01 user--
        [HttpGet]
        [Area("Admin")]
        public IActionResult Details(int ID)
        {
            if (ID == null)
            {
                return NotFound();
            }
            var objUser = db_Context.Users.FirstOrDefault(n => n.UserId == ID);
            if (objUser == null)
            {
                return NotFound();
            }
            LoadDropMenu();

            return View(objUser);
        }
        // END --thêm user--

        /*=======================================================*/
        // START --xoá sản phẩm--
        [Area("Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int ID)
        {
            var objUser = await db_Context.Users.FindAsync(ID);
            if (objUser.Avatar != null)
            {
                var imgPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Images", "users", objUser.Avatar);
                if (System.IO.File.Exists(imgPath))
                    System.IO.File.Delete(imgPath);
            }

            db_Context.Users.Remove(objUser);
            db_Context.SaveChanges();
            TempData["Success"] = "Đã xoá tài khoản: " + "[" + objUser.FirstName + " " + objUser.LastName + "]";
            return RedirectToAction("Index");
        }
        // END --Delete user--

        /*=======================================================*/
        // START --Add Or Edit user--
        [Area("Admin")]
        public IActionResult AddOrEdit(int ID = 0)
        {
            LoadDropMenu();

            if (ID == 0)
            {
                return View(new User());

            }
            else
                return View(db_Context.Users.Find(ID));
        }

        [HttpPost]
        [Area("Admin")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(List<IFormFile> files, int UserID, User objUser)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string img = objUser.Avatar;

            var size = files.Sum(f => f.Length);
            var filePaths = new List<string>();
            foreach (var formFile in files)
            {

                string newFileName = formFile.FileName;


                newFileName = long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + "_" + formFile.FileName;

                var filePath = "";
                filePath = Path.Combine(contentRootPath, "wwwroot", "Images", "users", newFileName);


                filePaths.Add(filePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);

                }
                if (newFileName.ToString().Trim().Length != null)
                {
                    if (img != null)
                    {
                        var imgPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Images", "users", img);
                        if (System.IO.File.Exists(imgPath))
                            System.IO.File.Delete(imgPath);
                    }

                    objUser.Avatar = newFileName;
                }
                else
                {
                    objUser.Avatar = img;
                }

            }
            try
            {
                if (objUser.UserId != 0)
                {
                  //  objUser.CreateOnUtc = saveDatetime;
                    db_Context.Entry(objUser).State = EntityState.Modified;
                    TempData["Success"] = "Đã sửa thông tin tài khoản: " +"[" + objUser.FirstName + " " + objUser.LastName + "]";

                }
                else
                {
                    objUser.Password = GetMD5("123");
                    objUser.CreateOnUtc = DateTime.Now;                    
                    db_Context.Users.Add(objUser);
                    TempData["Success"] = "Đã thêm tài khoản: " + "[" + objUser.FirstName + " " + objUser.LastName + "]";
                }

                db_Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            catch (Exception ex)
            {
                TempData["Success"] = "Kiểm tra lại thao tác !";
                return RedirectToAction("AddOrEdit");
            }
        }

        // END --Add Or Edit sản phẩm--

        public void LoadDropMenu()
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            Common objCommon = new Common();


            //drop menu ẩn:0 / hiện:1
            List<Show> listUserShow = new List<Show>();
            Show objUserShow = new Show();
 
            objUserShow.ID = false;
            objUserShow.Name = "Khách hàng";
            listUserShow.Add(objUserShow);

            objUserShow = new Show();
            objUserShow.ID = true;
            objUserShow.Name = "Admin";
            listUserShow.Add(objUserShow);
            DataTable dtUserShow = converter.ToDataTable(listUserShow);
            ViewBag.listUserShow = objCommon.ToSelectList(dtUserShow, "ID", "Name");


        }
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

	}

}
