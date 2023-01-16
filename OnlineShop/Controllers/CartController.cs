using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using System.Data.Common;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var cartItems = db.Carts.Include("User").Include("Article").Where(cart => cart.UserId == _userManager.GetUserId(User));
            var articles = db.Articles.Include("Category");
            var photos = db.Photos.Include("Article").Where(photo => photo.isProfilePhoto == true);
            float? total = 0;
            float? livrare = 0;
            foreach(var item in cartItems) 
            {
                var quantity = item.ArticleCount;
                float? price = db.Articles.Find(item.ArticleId).Price;
                total += quantity * price;
            }
            livrare = 50 - total;
            if (TempData["Discount"] != null)
            {
                ViewBag.Discount = Convert.ToInt32(TempData["Discount"]);
                ViewBag.Id = TempData["Id"];
            }
            ViewBag.Total = total;
            ViewBag.Livrare = livrare;
            ViewBag.CartItems = cartItems;
            ViewBag.Articles = articles;
            ViewBag.Photos = photos;
            return View();
        }

        public IActionResult CountUp(int id)
        {
            if (ModelState.IsValid)
            {
                var articolAdaugat = db.Carts.Find(id, _userManager.GetUserId(User));
                articolAdaugat.ArticleCount++;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult CountDown(int id)
        {
            if (ModelState.IsValid)
            {
                var articolAdaugat = db.Carts.Find(id, _userManager.GetUserId(User));
                articolAdaugat.ArticleCount--;
                if(articolAdaugat.ArticleCount == 0)
                {
                    db.Carts.Remove(articolAdaugat);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var articolAdaugat = db.Carts.Find(id, _userManager.GetUserId(User));
                db.Carts.Remove(articolAdaugat);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
