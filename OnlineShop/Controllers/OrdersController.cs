using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{

    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {

            var orders = db.Orders.Include("OrderItems").Include("User").Include("Coupon").Where(ord => ord.UserId == _userManager.GetUserId(User));
            ViewBag.Orders = orders;
            var orderItems = db.OrderItems.Include("Order");
            var photos = db.Photos.Include("Article").Where(poza => poza.isProfilePhoto == true);
            ViewBag.Articles = db.Articles.Include("Category");
            ViewBag.Photos = photos;
            ViewBag.OrderItems = orderItems;
            ViewBag.Coupons = db.Coupons;

            return View();
        }

        public IActionResult PlaceOrder(Order requestOrder)
        {
            if(ModelState.IsValid)
            {
                Order order = new Order
                {
                    Date = DateTime.Now,
                    UserId = _userManager.GetUserId(User),
                    FirstName = requestOrder.FirstName,
                    LastName = requestOrder.LastName,
                    Address= requestOrder.Address,
                    City= requestOrder.City,
                    Country = requestOrder.Country,
                    PhoneNumber = requestOrder.PhoneNumber,
                    DeliveryMethod= requestOrder.DeliveryMethod,
                    CouponId= requestOrder.CouponId
                };
                db.Orders.Add(order);
                db.SaveChanges();

                var cartItems = db.Carts.Include("User").Where(cart => cart.UserId == order.UserId);

                foreach(Cart item in cartItems)
                {
                    OrderItem orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ArticleId = item.ArticleId,
                        ArticleCount = item.ArticleCount
                    };
                    db.OrderItems.Add(orderItem);
                    db.Carts.Remove(item);
                }

                db.SaveChanges();
                TempData["symbol"] = "True";
                TempData["classes"] = "alert";
                TempData["message"] = "Order placed";
            }
            return Redirect("/Articles/Index");
        }

        public IActionResult New()
        {
            return View();
        }
    }
}
