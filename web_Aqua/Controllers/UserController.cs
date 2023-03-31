using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using web_Aqua.Context;
using web_Aqua.Models;

namespace web_Aqua.Controllers
{
	public class UserController : Controller
	{


        db_aquaponicsContext db_Context = new db_aquaponicsContext();

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;
      
        public UserController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _contextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Profile()
		{
          
            //try
            //{
                if (_contextAccessor.HttpContext.Session.GetInt32("UserId") != null)
                {

                    var user = db_Context.Users.Where(u => u.UserId == _contextAccessor.HttpContext.Session.GetInt32("UserId")).FirstOrDefault();
                    ViewBag.countOrder = db_Context.Orders.Where(o => o.UserId == _contextAccessor.HttpContext.Session.GetInt32("UserId")).ToList().Count;

                    return View(user);
                }
                else
                    return RedirectToAction("Login", "Home");
    //    }
    //        catch
    //        {
    //            return RedirectToAction("Login", "Home");

    //}


}


        public IActionResult EditProfile()
        {
            int? id = _contextAccessor.HttpContext.Session.GetInt32("UserId");
            if (id >0)
            {                                 
                return View(db_Context.Users.Find(id));
            }
            else {
                return RedirectToAction("Login", "Home");
            }
            
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(List<IFormFile> files, User objUser)
        {
            int? UserID = _contextAccessor.HttpContext.Session.GetInt32("UserId");
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string img = objUser.Avatar;
            string oldpassword = db_Context.Users.Where(u=>u.UserId== UserID).Select(u=>u.Password) .FirstOrDefault();

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

                if (objUser.Password!=null)
                {
                    objUser.Password = GetMD5(objUser.Password);
                }
                else
                {
                    objUser.Password = oldpassword;
                }
                
                db_Context.Entry(objUser).State = EntityState.Modified;
                TempData["Success"] = "Đã sửa thông tin tài khoản: " + "[" + objUser.FirstName + " " + objUser.LastName + "]";

                await db_Context.SaveChangesAsync();
                return RedirectToAction("Profile", "User");

            
            //try
            //{

            //    if (objUser.Password != "")
            //    {
            //        objUser.Password = GetMD5(objUser.Password);
            //    }
            //    else
            //    {
            //        objUser.Password = oldpassword;
            //    }
            //    objUser.IsAdmin = false;
            //    db_Context.Entry(objUser).State = EntityState.Modified;
            //    TempData["Success"] = "Đã sửa thông tin tài khoản: " + "[" + objUser.FirstName + " " + objUser.LastName + "]";

            //    db_Context.SaveChanges();
            //    return RedirectToAction("Profile", "User",objUser.UserId);
            //}


            //catch (Exception ex)
            //{
            //    TempData["Success"] = "Kiểm tra lại thao tác !";
            //    return RedirectToAction("EditProfile");
            //}
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
