using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Versioning;
using OnlineShop.Data;
using OnlineShop.Data.Migrations;
using OnlineShop.Models;
using System;

namespace OnlineShop.Controllers
{
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CouponsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Apply(Coupon couponRequest)
        {
            if(ModelState.IsValid)
            {
                var coupon = db.Coupons.Find(couponRequest.Id);
                if(coupon == null)
                {
                    TempData["classes"] = "alert alertRau";
                    TempData["message"] = "Coupon code is not valid";
                }
                else
                {
                    TempData["Discount"] = coupon.Discount;
                    TempData["Id"] = coupon.Id;
                    TempData["symbol"] = "True";
                    TempData["classes"] = "alert";
                    TempData["message"] = "The discount coupon has been successfully applied to the order!";
                }
            }
            return Redirect("/Cart/Index");
        }

        public ActionResult New()
        {
            var coupons = db.Coupons;
            ViewBag.Coupons= coupons;
            return View();
        }

        [HttpPost]
        public ActionResult New(Coupon coup)
        {
            try
            {
                if (ModelState.IsValid) { 
                    db.Coupons.Add(coup);
                    db.SaveChanges();
                    TempData["symbol"] = "True";
                    TempData["classes"] = "alert";
                    TempData["message"] = "Coupon added";
                }
                else
                {
                    TempData["classes"] = "alert alertRau";
                    TempData["message"] = "Coupon code was not added";
                    var coupons = db.Coupons;
                    ViewBag.Coupons = coupons;
                    return View(coup);
                }
                return RedirectToAction("New");
            }
            catch (Exception e)
            {
                TempData["classes"] = "alert alertRau";
                TempData["message"] = "Coupon code already exists";
                return RedirectToAction("New");
            }
        }

        public IActionResult Edit(string id)
        {
            var coupon = db.Coupons.Find(id);
            ViewBag.Coupon = coupon;
            return View(coupon);
        }

        [HttpPost]
        public IActionResult Edit(string id, Coupon coupon)
        {
            try
            {
                Coupon coup = db.Coupons.Find(id);
                if (ModelState.IsValid)
                {
                    coup.Discount = coupon.Discount;
                    TempData["symbol"] = "True";
                    TempData["classes"] = "alert";
                    TempData["message"] = "Coupon modified";
                    db.SaveChanges();
                    return RedirectToAction("New");
                }
                else
                {
                    var coupon2 = db.Coupons.Find(id);
                    ViewBag.Coupon = coupon2;
                    TempData["classes"] = "alert alertRau";
                    TempData["message"] = "Coupon code is not valid";
                    return View(coupon);
                }
                
            }
            catch (Exception e)
            {
                return RedirectToAction("Edit", id);
            }
        }

        [HttpPost] 
        public IActionResult Delete(string id)
        {
            Coupon coupon = db.Coupons.Find(id);
            db.Coupons.Remove(coupon);
            db.SaveChanges();
            TempData["symbol"] = "True";
            TempData["classes"] = "alert";
            TempData["message"] = "Coupon deleted";
            return RedirectToAction("New");
        }
    }
}
