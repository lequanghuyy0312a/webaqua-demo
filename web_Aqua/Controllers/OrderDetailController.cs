using Microsoft.AspNetCore.Mvc;
using web_Aqua.Models;
using web_Aqua.Context;

namespace web_Aqua.Controllers
{
    public class OrderDetailController : Controller
    {
        db_aquaponicsContext db_Context = new db_aquaponicsContext();
        private readonly IHttpContextAccessor _contextAccessor;

        public OrderDetailController(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            int? userID = _contextAccessor.HttpContext.Session.GetInt32("UserId");
            if (userID < 1 || userID == null) {
                return RedirectToAction("Login", "Home");
               
            }
            else
            {
                var listOrder = db_Context.Orders.Where(o => o.UserId == userID).ToList();
                listOrder = listOrder.OrderByDescending(sort => sort.OrderId).ToList();
                return View(listOrder);
            }
           
        }
        public IActionResult Detail(int orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }
            
            
            HomeModel objHomeModel = new HomeModel();
            objHomeModel.listOrder = db_Context.Orders.ToList();
            objHomeModel.listUser = db_Context.Users.ToList();
            objHomeModel.listProduct = db_Context.Products.ToList();
            objHomeModel.listOrderDetail = db_Context.OrderDetails.ToList();


            objHomeModel.listOrderDetailFull = (from d in objHomeModel.listOrderDetail

                                            join p in objHomeModel.listProduct on d.ProductId equals p.ProductId
                                            where d.ProductId == p.ProductId

                                            join o in objHomeModel.listOrder on d.OrderId equals o.OrderId
                                            where d.OrderId == o.OrderId

                                            join u in objHomeModel.listUser on o.UserId equals u.UserId
                                            where o.UserId == u.UserId
                                            select d).Where(d=>d.OrderId==orderId) .ToList();


            
           if (objHomeModel.listOrderDetailFull == null)
            {
                return NotFound();
            }

            return View(objHomeModel);
        }
    }
}
