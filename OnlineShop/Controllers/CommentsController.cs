using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace ArticlesApp.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        public CommentsController(ApplicationDbContext context)
        {
            db = context;
        }


        // Adaugarea unui comentariu asociat unui articol in baza de date
        [HttpPost]
        public IActionResult New(Comment comm)
        {
            comm.Date = DateTime.Now;

            try
            {
                db.Comments.Add(comm);
                db.SaveChanges();
                TempData["message"] = "Comment added";
                return Redirect("/Articles/Show/" + comm.ArticleId);
            }

            catch (Exception)
            {
                return Redirect("/Articles/Show/" + comm.ArticleId);
            }

        }

        // Stergerea unui comentariu asociat unui articol din baza de date
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            var articleId = comm.ArticleId;
            db.Comments.Remove(comm);
            db.SaveChanges();

            var comments = db.Comments.Where(comm => comm.ArticleId == articleId);
            var medieRating = 0;
            foreach (var comentariu in comments)
            {
                medieRating += comentariu.Rating;
            }
            if (comments.Count() > 0)
            {
                medieRating /= comments.Count();
            }
            var articol = db.Articles.Find(articleId);
            articol.Rating = medieRating;
            db.SaveChanges();

            TempData["message"] = "Comment deleted";
            return Redirect("/Articles/Show/" + comm.ArticleId);
        }

        // In acest moment vom implementa editarea intr-o pagina View separata
        // Se editeaza un comentariu existent

        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            ViewBag.Comment = comm;
            return View(comm);
        }

        [HttpPost]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);
            try
            {
                if (ModelState.IsValid)
                {
                    comm.Content = requestComment.Content;
                    comm.Rating = requestComment.Rating;
                    comm.Date = DateTime.Now;
                    db.SaveChanges();

                    var comments = db.Comments.Where(com => com.ArticleId == comm.ArticleId);
                    var medieRating = 0;
                    foreach (var comentariu in comments)
                    {
                        medieRating += comentariu.Rating;
                    }
                    medieRating = medieRating / comments.Count();
                    var articol = db.Articles.Find(comm.ArticleId);
                    articol.Rating = medieRating;
                    db.SaveChanges();

                    TempData["symbol"] = "True";
                    TempData["classes"] = "alert";
                    TempData["message"] = "Comment modified";

                    return Redirect("/Articles/Show/" + comm.ArticleId);
                }
                else
                {
                    Comment comm2 = db.Comments.Find(id);
                    ViewBag.Comment = comm2;
                    TempData["classes"] = "alert alertRau";
                    TempData["message"] = "Comment was not modified";
                }
                return View(requestComment);
            }
            catch (Exception e)
            {
                TempData["classes"] = "alert alertRau";
                TempData["message"] = "Comment was not modified";
                return RedirectToAction("Edit", id);
            }

        }
    }
}