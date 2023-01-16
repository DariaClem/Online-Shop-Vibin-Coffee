using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Data.Migrations;
using OnlineShop.Models;
using System;

namespace OnlineShop.Controllers
{

    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext db;
        
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private IWebHostEnvironment _env;
        public ArticlesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager, IWebHostEnvironment env)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }

        public IActionResult Index(string filter)
        {
            // Proprietate
            var articles = db.Articles.Include("Category").Include("User").Where(art => art.Accepted == true);
            var photos = db.Photos.Include("Article");
            var requests = db.Articles;
            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
            }

            List<int> articleIds = db.Articles.Where(art => art.Title.Contains(search) || art.Content.Contains(search)).Select(a => a.Id).ToList();

            if (search != "")
            {
                articles = db.Articles.Where(article => articleIds.Contains(article.Id)).Include("Category").Include("User").OrderBy(a => a.Date).Where(art => art.Accepted == true);
            }

            ViewBag.PriceUp = "price_up";
            ViewBag.PriceDown = "price_down";
            ViewBag.RatingUp = "rating_up";
            ViewBag.RatingDown = "rating_down";
            ViewBag.DefaultOrder = "default";

            switch (filter)
            {
                case "price_up":
                    articles = articles.OrderBy(art => art.Price);
                    break;
                case "price_down":
                    articles = articles.OrderByDescending(art => art.Price);
                    break;
                case "rating_up":
                    articles = articles.OrderBy(art => art.Rating);
                    break;
                case "rating_down":
                    articles = articles.OrderByDescending(art => art.Rating);
                    break;
                default:
                    articles = articles.OrderBy(a => a.Date);
                    break;
            }

            ViewBag.Search = search;
            ViewBag.Articles = articles;
            ViewBag.Photos = photos;
            ViewBag.Requests = requests;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        public IActionResult Show(int id)
        {
            Article article = db.Articles.Include("Category").Include("User").Include("Comments").Include("Comments.User").Where(art => art.Id == id).First();
            var photo = db.Photos.Include("Article").Where(art => art.ArticleId == id).Where(art => art.isProfilePhoto == false);
            ViewBag.Articles = article;
            ViewBag.Category = article.Category;
            ViewBag.Photo = photo;
            SetAccessRights();
            return View(article);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                TempData["symbol"] = "True";
                TempData["classes"] = "alert";
                TempData["message"] = "Comment added";
                db.Comments.Add(comment);
                db.SaveChanges();
                var comments = db.Comments.Where(comm => comm.ArticleId == comment.ArticleId);
                var medieRating = 0;
                foreach(var comentariu in comments)
                {
                    medieRating += comentariu.Rating; 
                }
                medieRating = medieRating / comments.Count();
                var articol = db.Articles.Find(comment.ArticleId);
                articol.Rating = medieRating;
                db.SaveChanges();
                return Redirect("/Articles/Show/" + comment.ArticleId);
            }
            else
            {
                TempData["classes"] = "alert alertRau";
                TempData["message"] = "Comment was not added";
                Article art = db.Articles.Include("Category")
                                         .Include("User")
                                         .Include("Comments")
                                         .Include("Comments.User")
                                         .Where(art => art.Id == comment.ArticleId)
                                         .First();
                var photo = db.Photos.Include("Article").Where(art => art.ArticleId == comment.ArticleId).Where(art => art.isProfilePhoto == false);
                ViewBag.Articles = art;
                ViewBag.Category = art.Category;
                ViewBag.Photo = photo;
                SetAccessRights();

                return View(art);
            }
        }

        //Date articol nou
        [Authorize(Roles = "Colaborator,Admin")]
        public IActionResult New()
        {
            var categories = from categ in db.Categories select categ;
            ViewBag.Categories = categories;
            return View();
        }

        public async Task PhotoSetup(IFormFile photo, bool profilePhoto, int id)
        {
            var storagePath = Path.Combine(_env.WebRootPath, "Poze", photo.FileName);
               
            var databaseFileName = "/Poze/" + photo.FileName;
            using (var fileStream = new FileStream(storagePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }
            Photo photoAdd = new Photo()
            {
                Id = databaseFileName,
                ArticleId = id,
                isProfilePhoto = profilePhoto
            };

            Console.WriteLine(photoAdd.Id);
            db.Photos.Add(photoAdd);

            db.Update(photoAdd);
            await db.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<IActionResult> New(Article article, IFormFile photoProfile, IFormFile photo1, IFormFile photo2, IFormFile photo3)
        {

            if (ModelState.IsValid)
            {
                article.Date = DateTime.Now;
                article.UserId = _userManager.GetUserId(User);

                db.Articles.Add(article);
                db.SaveChanges();
                TempData["symbol"] = "True";
                TempData["classes"] = "alert";
                TempData["message"] = "Request sent";

                var storagePath = Path.Combine(_env.WebRootPath, "Poze", photoProfile.FileName);

                var databaseFileName = "/Poze/" + photoProfile.FileName;
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await photoProfile.CopyToAsync(fileStream);
                }
                Photo photoAdd = new Photo()
                {
                    Id = databaseFileName,
                    ArticleId = article.Id,
                    isProfilePhoto = true
                };

                db.Photos.Add(photoAdd);


                var storagePath1 = Path.Combine(_env.WebRootPath, "Poze", photo1.FileName);

                var databaseFileName1 = "/Poze/" + photo1.FileName;
                using (var fileStream = new FileStream(storagePath1, FileMode.Create))
                {
                    await photo1.CopyToAsync(fileStream);
                }
                Photo photoAdd1 = new Photo()
                {
                    Id = databaseFileName1,
                    ArticleId = article.Id,
                    isProfilePhoto = false
                };

                db.Photos.Add(photoAdd1);


                var storagePath2 = Path.Combine(_env.WebRootPath, "Poze", photo2.FileName);

                var databaseFileName2 = "/Poze/" + photo2.FileName;
                using (var fileStream = new FileStream(storagePath2, FileMode.Create))
                {
                    await photo2.CopyToAsync(fileStream);
                }
                Photo photoAdd2 = new Photo()
                {
                    Id = databaseFileName2,
                    ArticleId = article.Id,
                    isProfilePhoto = false
                };

                db.Photos.Add(photoAdd2);


                var storagePath3 = Path.Combine(_env.WebRootPath, "Poze", photo3.FileName);

                var databaseFileName3 = "/Poze/" + photo3.FileName;
                using (var fileStream = new FileStream(storagePath3, FileMode.Create))
                {
                    await photo3.CopyToAsync(fileStream);
                }
                Photo photoAdd3 = new Photo()
                {
                    Id = databaseFileName3,
                    ArticleId = article.Id,
                    isProfilePhoto = false
                };

                db.Photos.Add(photoAdd3);

                db.SaveChanges();

                return RedirectToAction("Pending");
            }
            else
            {
                var categories = from categ in db.Categories select categ;
                ViewBag.Categories = categories;
                TempData["classes"] = "alert alertRau";
                TempData["message"] = "The product was not added";
            }

            return View(article);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            Article article = db.Articles.Include("Category").Where(art => art.Id == id).First();
            
            ViewBag.Articles = article;
            ViewBag.Category = article.Category;
            
            var categories = from categ in db.Categories select categ;
            
            ViewBag.Categories = categories;
            
            return View(article);
        }

        [HttpPost]

        public IActionResult Edit(int id, Article requestArticle)
        {
            Article article = db.Articles.Find(id);

            try
            {
                if (ModelState.IsValid)
                {
                    article.Title = requestArticle.Title;
                    article.Content = requestArticle.Content;
                    article.CategoryId = requestArticle.CategoryId;
                    article.Date = requestArticle.Date;
                    article.Price = requestArticle.Price;
                    article.Weight= requestArticle.Weight;

                    db.SaveChanges();

                    TempData["symbol"] = "True";
                    TempData["classes"] = "alert";
                    TempData["message"] = "Article modified";

                    return Redirect("/Articles/Show/" + id);
                }
                else
                {
                    Article article2 = db.Articles.Include("Category").Where(art => art.Id == id).First();

                    ViewBag.Articles = article2;
                    ViewBag.Category = article2.Category;

                    var categories = from categ in db.Categories select categ;

                    ViewBag.Categories = categories;
                    TempData["classes"] = "alert alertRau";
                    TempData["message"] = "Article was not modified";
                }
                return View(requestArticle);
            }
            catch(Exception)
            {
                TempData["classes"] = "alert alertRau";
                TempData["message"] = "Article was not modified";
                return RedirectToAction("Edit", id);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            Article article = db.Articles.Find(id);
            var photo = db.Photos.Where(art => art.ArticleId == id);
            var comm = db.Comments.Where(comm=> comm.ArticleId == id);
            var carts = db.Carts.Where(art => art.ArticleId == id);
            var orders = db.OrderItems.Where(art => art.ArticleId == id);
            foreach(var p in photo)
            {
                db.Photos.Remove(p);
            }
            foreach(var c in comm)
            {
                db.Comments.Remove(c);
            }
            foreach(var c in carts)
            {
                db.Carts.Remove(c);
            }
            foreach(var o in orders)
            {
                db.OrderItems.Remove(o);
            }
            db.Articles.Remove(article);
            db.SaveChanges();
            TempData["symbol"] = "True";
            TempData["classes"] = "alert";
            TempData["message"] = "Article deleted";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult AddToCart(int id)
        {
            if (ModelState.IsValid)
            {
                Article art1 = db.Articles.Find(id);
                Cart cart1 = db.Carts.Find(id, _userManager.GetUserId(User));
                if (cart1 == null)
                {
                    Cart cart2 = new Cart();
                    cart2.ArticleId = id;
                    cart2.UserId = _userManager.GetUserId(User);
                    cart2.ArticleCount = 1;
                    db.Carts.Add(cart2);
                }
                else 
                {
                    cart1.ArticleCount++;
                }
                db.SaveChanges();
                TempData["symbol"] = "True";
                TempData["classes"] = "alert";
                TempData["message"] = "Added to cart";
            }
            else
            {
                TempData["classes"] = "alert alertRau";
                TempData["message"] = "The product was not added to cart";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Pending()
        {
            var articles = db.Articles.Include("Category").Include("User");
            var photos = db.Photos.Include("Article");
            ViewBag.Articles = articles;
            ViewBag.Photos = photos;
            SetAccessRights();
            return View();
        }

        [HttpPost]
        public IActionResult Pending(int id)
        {
            if (ModelState.IsValid)
            {
                var article = db.Articles.Find(id);
                article.Accepted = true;
                db.SaveChanges();
                TempData["symbol"] = "True";
                TempData["classes"] = "alert";
                TempData["message"] = "Accepted product";
            }
            else
            {
                TempData["classes"] = "alert alertRau";
                TempData["message"] = "The product was not accepted";
            }
            return RedirectToAction("Pending");
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Colaborator"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}
