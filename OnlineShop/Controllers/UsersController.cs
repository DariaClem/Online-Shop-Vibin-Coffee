using OnlineShop.Data;
using OnlineShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ArticlesApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = db.Users.OrderBy(user => user.UserName);
            var articole = db.Articles.Include("User");
            var roluri = db.Roles;

            ViewBag.UserCurentName = _userManager.GetUserName(User);
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.Roles = roluri;
            ViewBag.Articles = articole;
            ViewBag.UsersList = users;
            return View();
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = db.Users
                         .Include("Articles")
                         .Include("Comments")
                         .Include("Carts")
                         .Include("Orders")
                         .Where(u => u.Id == id)
                         .First();
            var orders = db.Orders.Include("User").Where(ord => ord.UserId == id);

            foreach(var orditm in orders)
            {
                var orditems = db.OrderItems.Include("Order").Where(item => item.OrderId == orditm.Id);
                foreach (var item in orditems)
                {
                    db.OrderItems.Remove(item);
                }

                db.Orders.Remove(orditm);
            }
            db.SaveChanges();

            if (user.Carts.Count > 0)
            {
                foreach (var cart in user.Carts)
                {
                    db.Carts.Remove(cart);
                }
            }

            if (user.Comments.Count > 0)
            {
                foreach (var comment in user.Comments)
                {
                    db.Comments.Remove(comment);
                }
            }

            var articlesPost = db.Articles.Include("User").Where(art => art.UserId == id);
            
            foreach(var articol in articlesPost)
            {
                var photos = db.Photos.Include("Article").Where(photo => photo.ArticleId == articol.Id);
                foreach(var photo in photos)
                {
                    db.Photos.Remove(photo);
                   
                }

                var cartItems = db.Carts.Include("Article").Where(cart => cart.ArticleId == articol.Id);
                foreach(var cart in cartItems)
                {
                    db.Carts.Remove(cart);
                   
                }

                var comments = db.Comments.Include("Article").Where(comment => comment.ArticleId == articol.Id);
                foreach(var comm in comments)
                {
                    db.Comments.Remove(comm);
                }

                var orderItems = db.OrderItems.Include("Article").Where(ord => ord.ArticleId == articol.Id);
                foreach(var item in orderItems)
                {
                    db.OrderItems.Remove(item);
                }

                db.Articles.Remove(articol);
            }

            db.ApplicationUsers.Remove(user);
            db.SaveChanges();

            TempData["symbol"] = "True";
            TempData["classes"] = "alert";
            TempData["message"] = "User deleted";

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);


            user.AllRoles = GetAllRoles();
            ViewBag.Roles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user); // Lista de nume de roluri

            // Cautam ID-ul rolului in baza de date
            var currentUserRole = _roleManager.Roles
                                              .Where(r => roleNames.Contains(r.Name))
                                              .Select(r => r.Id)
                                              .First(); // Selectam 1 singur rol
            ViewBag.UserRole = currentUserRole;

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, [FromForm] string newRole)
        {
            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();


            if (ModelState.IsValid)
            {

                // Cautam toate rolurile din baza de date
                var roles = db.Roles.ToList();

                foreach (var role in roles)
                {
                    // Scoatem userul din rolurile anterioare
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                // Adaugam noul rol selectat
                var roleName = await _roleManager.FindByIdAsync(newRole);
                await _userManager.AddToRoleAsync(user, roleName.ToString());

                db.SaveChanges();

                TempData["symbol"] = "True";
                TempData["classes"] = "alert";
                TempData["message"] = "Role edited";

                return RedirectToAction("Index");
            }
            else
            {
                TempData["classes"] = "alert alertRau";
                TempData["message"] = "Role was not edited";
                return View(newRole);
            }
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
    }
}
