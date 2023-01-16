using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace ArticlesApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        public CategoriesController(ApplicationDbContext context)
        {
            db = context;
        }
        public ActionResult Index()
        {
            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;
            ViewBag.Categories = categories;
            return View();
        }

        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.Categories = category;
            return View();
        }

        public ActionResult New()
        {
            var category = db.Categories;
            var articles = db.Articles.Include("Category");
            ViewBag.Category = category;
            ViewBag.Articles = articles;
            return View();
        }

        [HttpPost]
        public ActionResult New(Category cat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Categories.Add(cat);
                    db.SaveChanges();
                    TempData["symbol"] = "True";
                    TempData["classes"] = "alert";
                    TempData["message"] = "Category added";
                    return RedirectToAction("New");
                }
                else
                {
                    TempData["classes"] = "alert alertRau";
                    TempData["message"] = "Category was not added";
                    var category = db.Categories;
                    var articles = db.Articles.Include("Category");
                    ViewBag.Category = category;
                    ViewBag.Articles = articles;
                    return View(cat);
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("New");
            }
        }

        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.Category = category;
            return View(category);
        }

        [HttpPost]
        public ActionResult Edit(int id, Category requestCategory)
        {
            try
            {
                Category category = db.Categories.Find(id);
                if (ModelState.IsValid)
                {
                    category.CategoryName = requestCategory.CategoryName;
                    TempData["symbol"] = "True";
                    TempData["classes"] = "alert";
                    TempData["message"] = "Category modified";
                    db.SaveChanges();
                    return RedirectToAction("New");
                }
                else
                {
                    Category category2 = db.Categories.Find(id);
                    ViewBag.Category = category2;
                    TempData["classes"] = "alert alertRau";
                    TempData["message"] = "Category was not edited";
                }
                return View(requestCategory);
            }
            catch (Exception e)
            {
                TempData["classes"] = "alert alertRau";
                TempData["message"] = "Category was not edited";
                ViewBag.Category = requestCategory;
                return RedirectToAction("Edit", id);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            var articles = db.Articles.Include("Category").Where(art => art.CategoryId == id);
            foreach(var art in articles)
            {
                var photo = db.Photos.Where(a => a.ArticleId == art.Id);
                var comm = db.Comments.Where(comm => comm.ArticleId == art.Id);
                var carts = db.Carts.Where(a => a.ArticleId == art.Id);
                var orders = db.OrderItems.Where(a => a.ArticleId == art.Id);
                foreach (var p in photo)
                {
                    db.Photos.Remove(p);
                }
                foreach (var c in comm)
                {
                    db.Comments.Remove(c);
                }
                foreach (var c in carts)
                {
                    db.Carts.Remove(c);
                }
                foreach (var o in orders)
                {
                    db.OrderItems.Remove(o);
                }
                db.Articles.Remove(art);

            }

            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["symbol"] = "True";
            TempData["classes"] = "alert";
            TempData["message"] = "Category deleted";
            return RedirectToAction("New");
        }
    }
}
