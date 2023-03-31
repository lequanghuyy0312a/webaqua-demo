using Microsoft.AspNetCore.Mvc;
using System;
using Newtonsoft.Json;
using web_Aqua.Helpers;
using web_Aqua.Models;
using web_Aqua.Context;

using Microsoft.CodeAnalysis;

namespace web_Aqua.Controllers
{

    public class CartController : Controller
	{
		db_aquaponicsContext db_context = new db_aquaponicsContext();
        private readonly IHttpContextAccessor _contextAccessor;

        public CartController(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }
        public const string CARTKEY = "cart";

		List<CartModel> GetCartItems()
		{
			var session = HttpContext.Session;
			string jsoncart = session.GetString(CARTKEY);
			if (jsoncart != null)
			{
				return JsonConvert.DeserializeObject<List<CartModel>>(jsoncart);
			}
			
			return new List<CartModel>();
		}
		// lưu giỏ hàng
		void SaveCartSession(List<CartModel> listCart)
		{
			var session = HttpContext.Session;
			string jsoncart = JsonConvert.SerializeObject(listCart);
			session.SetString(CARTKEY, jsoncart);
		}

		public IActionResult Index()
		{
			ViewBag.countCart = GetCartItems().Count;
			return View(GetCartItems());

		}

		public IActionResult AddToCart(int id, int quantity = 1)
		{
			try
			{
				var product = db_context.Products.Where(p => p.ProductId == id).FirstOrDefault();
				if (product == null)
				{
					return NotFound("Không có sản phẩm");
				}
				var cart = GetCartItems();
				var cartItem = cart.Find(p => p._shoppingProduct.ProductId == id);
				if (cartItem != null)
				{

					cartItem._shoppingQuantity++;
				}
				else
				{

					cart.Add(new CartModel() { _shoppingQuantity = 1, _shoppingProduct = product });
				}
				SaveCartSession(cart);
				TempData["Success"] = "Đã thêm sản phẩm";
				return Redirect(Request.Headers["Referer"].ToString());

			}
			catch
			{
				return Redirect(Request.Headers["Referer"].ToString());
			}
		
		}

		// xoá 1 sản phẩm trong giỏ hàng
		[Route("/removecart/{productid:int}", Name = "removecart")]
		public IActionResult RemoveCart([FromRoute] int productid)
		{
			try
			{
				var cart = GetCartItems();
				var cartitem = cart.Find(p => p._shoppingProduct.ProductId == productid);
				if (cartitem != null)
				{
					cart.Remove(cartitem);
				}

				SaveCartSession(cart);
				return RedirectToAction("Index");
			}
			catch
			{
				return RedirectToAction("Index");

			}

		}

		// cập nhật giá khi thay đổi số lượng
		[Route("/updatecart", Name = "updatecart")]
		[HttpPost]
		public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
		{
			var cart = GetCartItems();
			var cartitem = cart.Find(p => p._shoppingProduct.ProductId == productid);
			if (cartitem != null)
			{
				cartitem._shoppingQuantity = quantity;
			}
			SaveCartSession(cart);
			return Ok();
		}


		// thanh toán
		public IActionResult CheckOut()
		{
            var cart = GetCartItems();

			try
			{
				int? userID = _contextAccessor.HttpContext.Session.GetInt32("UserId");
                if (userID == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                { 
					var checkphone = db_context.Users.Where(u=>u.UserId == userID).FirstOrDefault();
					if (checkphone.Address.Length > 10 )
					{
                        if (cart.Count > 0)
                        {
                            var listCartCheckOut = GetCartItems();
                            Order objOder = new Order();
                            objOder.Name = "DonHang" + DateTime.Now.ToString("yyyyMMddHHmmss");
                            objOder.UserId = userID;
                            objOder.CreatedOnUtc = DateTime.Now;
                            objOder.Status = 1;

                            db_context.Orders.Add(objOder);
                            db_context.SaveChanges(); //lưu vào db

                            int OrderID = objOder.OrderId;   //lấy id đơn hàng đưa vào bảng OrderDetail
                            List<OrderDetail> listOD = new List<OrderDetail>();

                            foreach (var item in listCartCheckOut)
                            {
                                OrderDetail objOD = new OrderDetail();
                                objOD.Quantity = item._shoppingQuantity;
                                objOD.OrderId = OrderID;
                                objOD.ProductId = item._shoppingProduct.ProductId;
                                listOD.Add(objOD);//lưu qua bảng OD
                            }

                            db_context.OrderDetails.AddRange(listOD);
                            db_context.SaveChanges();
                            ClearCart();
                        }
                        else
                        {
                            return RedirectToAction("Index", "Cart");
                        }
                    }
					else
					{
						TempData["Error"] = "Vui lòng cập nhật địa chỉ và số điện thoại";
                        return RedirectToAction("EditProfile", "User");
                    }
                }
                return RedirectToAction("Index", "OrderDetail");
			}
			catch
			{
				return RedirectToAction("Index", "Cart");

			}

			//return RedirectToAction("Cart", "Payment");
		}
        //xoá toàn bộ sản phẩm trong giỏ hàng
        void ClearCart()
        {
            _contextAccessor.HttpContext.Session.Remove(CARTKEY);
        }
    }
}

