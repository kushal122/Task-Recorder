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

namespace TaskRecorder.Areas.User.Controllers
{
    [Area("User")]
    public class UserLoginController : Controller
    {
        private readonly ConnectionDBContext db;
        private readonly SecurityManager securityManager = new SecurityManager();
        public UserLoginController(ConnectionDBContext db)
        {
            this.db = db;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Users
                {
                    UserId = Guid.NewGuid().ToString("n"),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = PasswordHelper.Encrypt(model.Password),
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address
                };
                if (user != null)
                {
                    db.Users.Add(user);
                    var getId = 0;
                    if (db.UsersRoles.Count() != 0)
                    {
                        getId = db.UsersRoles.Count();
                    }
                    else
                    {
                        getId = 0;
                    }
                    UsersRoles userRole = new UsersRoles();
                    userRole.Id =getId + 1;
                    userRole.UserId = user.UserId;
                    userRole.RoleId = 2;
                    try
                    {
                        db.UsersRoles.Add(userRole);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {

                        ViewBag.Message = e.InnerException.Message;
                        return View();
                    }

                    await db.SaveChangesAsync();
                    ModelState.Clear();
                    ViewBag.Message = "Registration Successful";
                    return View();
                }
                else
                {
                    ViewBag.Message = "Error While Registering. Please Try Again";
                    return View();
                }
            }
            return View(model);
        }
        [AllowAnonymous]
        [Route("user/login")]
        [HttpGet]
        public IActionResult SignIn()
        {
            var check = User.IsInRole("User");
            if (check == true)
            {
                if (User.IsInRole("User"))
                {
                    String url = string.Format("/user/dashboard");
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
        [Route("user/login")]
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
                    securityManager.SignIn(this.HttpContext, check, "User_Schema");
                    if (!string.IsNullOrWhiteSpace(returnurl) && Url.IsLocalUrl(returnurl))
                    {
                        String url = string.Format(returnurl);
                        return Redirect(url);
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "UserDashboard",new { Areas="User"});
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
        [Route("user/signout")]
        [Authorize]
        public IActionResult SignOut()
        {
            var principle = User as ClaimsPrincipal;
            var check = User.Identity.IsAuthenticated;
            if (check == true)
            {
                securityManager.SignOut(this.HttpContext, "User_Schema");
                String url = string.Format("/Home/Home/Index");
                return Redirect(url);
            }
            else
            {
                return View("AccessDenied");
            }
        }
        [AcceptVerbs("Get","Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await db.Users.Where(c => c.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");
            }
        }
    }
}