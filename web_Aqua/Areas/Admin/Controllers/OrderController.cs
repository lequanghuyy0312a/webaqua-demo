using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Data;
using web_Aqua.Context;
using web_Aqua.Models;
using X.PagedList;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
namespace web_Aqua.Areas.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	public class OrderController : Controller
    {
        db_aquaponicsContext db_Context = new db_aquaponicsContext();
        private readonly IWebHostEnvironment _webHostEnvironment;


        public OrderController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        //load index//
        [Area("Admin")]
        public IActionResult Index(int? page, string SearchString, string currentFilter)
        {

            var listOrder = new List<Order>();
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
                listOrder = db_Context.Orders.Include(o => o.User).Where(n => n.CreatedOnUtc.Value.ToString("dd MMMM yyyy").Contains(SearchString)).ToList();
                ViewBag.listOrderCount = listOrder.Count();
            }
            else
            {
                listOrder = db_Context.Orders.Include(o=>o.User).ToList();
                ViewBag.listOrderCount = listOrder.Count();
            }
            ViewBag.CurrentFilter = SearchString;

            page = page < 1 ? 1 : page;
            int pagesize = 6;
            int pageNumber = (page ?? 1);
            listOrder = listOrder.OrderByDescending(n => n.OrderId).ToList();

            return View(listOrder.ToPagedList(pageNumber, pagesize));
        }




        /*=======================================================*/
        // START --xoá nhập hàng--
        [Area("Admin")]
        [HttpPost, ActionName("Delete")]
      //  [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int ID)
        {
           
                var objOrder = await db_Context.Orders.FindAsync(ID);

                db_Context.Orders.Remove(objOrder);
                db_Context.SaveChanges();
                TempData["Success"] = "Đã xoá: " + "[" + objOrder.OrderId + " - " + objOrder.Name + "]";
                return RedirectToAction("Index");
            
 
        
        }
        // END --Delete Order--

        /*=======================================================*/
      
        // Check hoàn thành đơn
   
        [Area("Admin")]
        [HttpPost, ActionName("Edit")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ID)
        {
            var objOrder = await db_Context.Orders.FindAsync(ID);

            if (objOrder.Status == 0)
                    objOrder.Status = 1;
                else
                    objOrder.Status = 0;
                db_Context.Entry(objOrder).State = EntityState.Modified;
                TempData["Success"] = "Đã sửa thông tin:  " + "[" + objOrder.OrderId + " - " + objOrder.Name + "]";

                db_Context.SaveChanges();
                return RedirectToAction(nameof(Index));       
        }

        [Area("Admin")]
        public IActionResult Details(int Id)
        {
            if (Id  <1)
            {
                return RedirectToAction("Index", "Order");
            }

            HomeModel objHomeModel = new HomeModel();
            objHomeModel.listOrder = db_Context.Orders.ToList();
            objHomeModel.listUser = db_Context.Users.ToList();
            objHomeModel.listProduct = db_Context.Products.ToList();
            objHomeModel.listOrderDetail = db_Context.OrderDetails.ToList();
            objHomeModel.order = db_Context.Orders.Where(o => o.OrderId == Id).FirstOrDefault();
            int? userid = objHomeModel.order.UserId;
            objHomeModel.user = db_Context.Users.Where(o => o.UserId == userid).FirstOrDefault();

            objHomeModel.listOrderDetailFull = (from d in objHomeModel.listOrderDetail

                                                join p in objHomeModel.listProduct on d.ProductId equals p.ProductId
                                                where d.ProductId == p.ProductId

                                                join o in objHomeModel.listOrder on d.OrderId equals o.OrderId
                                                where d.OrderId == o.OrderId

                                                join u in objHomeModel.listUser on o.UserId equals u.UserId
                                                where o.UserId == u.UserId
                                                select d).Where(d => d.OrderId == Id).ToList();



            if (objHomeModel.listOrderDetailFull == null)
            {
                return RedirectToAction("Index", "Order");

            }

            return View(objHomeModel);
        }



    }

}