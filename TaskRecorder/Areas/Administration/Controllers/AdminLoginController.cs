using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskRecorder.Areas.User.Models.ViewModels;
using TaskRecorder.Data;
using TaskRecorder.Helper.Password;
using TaskRecorder.Security;
using TaskRecorder_DataModels.Models;

namespace TaskRecorder.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class AdminLoginController : Controller
    {
        private readonly ConnectionDBContext db;
        private readonly SecurityManager securityManager = new SecurityManager();
        public AdminLoginController(ConnectionDBContext db)
        {
            this.db = db;
        }
        [AllowAnonymous]
        [Route("admin/login")]
        public IActionResult SignIn()
        {
            var check = User.IsInRole("Admin");
            if (check == true)
            {
                if (User.IsInRole("Admin"))
                {
                    String url = string.Format("/admin/dashboard");
                    return Redirect(url);
                }
                else
                {
                    return View();
                }
            }
            return View();
        }
        [AllowAnonymous]
        [Route("admin/login")]
        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel model, string returnurl = "")
        {
            if (ModelState.IsValid)
            {
                var find = await db.Users.Where(c => c.Email == model.Email).SingleOrDefaultAsync();
                var check = processLogin(model);
                if (check != null)
                {
                    var userRoles = await db.UsersRoles.Where(c => c.UserId == find.UserId).Select(c => c.Roles.RoleName).ToListAsync();
                    securityManager.SignIn(this.HttpContext, check, "Admin_Schema");
                    if (!string.IsNullOrWhiteSpace(returnurl) && Url.IsLocalUrl(returnurl))
                    {
                        String url = string.Format(returnurl);
                        return Redirect(url);
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "AdminDashboard", new { Area = "Administration" });
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid Login";
                    return View();
                }
            }
            return View();
        }
        private Users processLogin(LoginViewModel model)
        {
            var data = db.Users.Where(c => c.Email == model.Email).Select(c => c.Password).FirstOrDefault();
            var check = db.Users.Where(c => c.Email == model.Email && c.Password == PasswordHelper.Encrypt(model.Password)).SingleOrDefault();
            if (check != null)
            {
                return check;
            }
            return null;
        }
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
        [Route("admin/signout")]
        [Authorize]
        public IActionResult SignOut()
        {
            var principle = User as ClaimsPrincipal;
            var check = User.Identity.IsAuthenticated;
            if (check == true)
            {
                securityManager.SignOut(this.HttpContext, "Admin_Schema");
                String url = string.Format("/Home/Home/Index");
                return Redirect(url);
            }
            else
            {
                return View("AccessDenied");
            }
        }
    }
}