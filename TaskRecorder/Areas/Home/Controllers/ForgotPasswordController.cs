using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskRecorder.Areas.Home.Models;
using TaskRecorder.Data;
using TaskRecorder.Helper.Mail;
using TaskRecorder.Helper.Password;
using TaskRecorder.Helper.TokenHelper;

namespace TaskRecorder.Areas.Home.Controllers
{
    [Area("Home")]
    [AllowAnonymous]
    public class ForgotPasswordController : Controller
    {
        private readonly ConnectionDBContext db;
        private readonly MailHelper MailHelper = new MailHelper();
        public ForgotPasswordController(ConnectionDBContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var find = db.Users.Where(c => c.Email == model.Email).FirstOrDefault();
                if (find != null)
                {
                    var token = JwtToken.GenerateToken(find.UserId,find.UserId,model.Email);
                    var confirmationLink = Url.Action("ResetPassword", "ForgotPassword", new { Areas = "Home", userId = find.UserId, token = token }, Request.Scheme);
                    var To = model.Email;
                    MailHelper.Send(To, confirmationLink);
                    ViewBag.Message = $"If the {model.Email} is registered then you will recieve Confirmation link and follow the link.";
                }
                else
                {
                    ViewBag.Message = $"{model.Email} could not be found";
                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (token == null)
            {
                ModelState.AddModelError("", "Invalid Password Reset Token");
            }
            var handeler = new JwtSecurityTokenHandler();
            var jwtToken = handeler.ReadToken(token) as JwtSecurityToken;
            var getEmail = jwtToken.Claims.First(Claim => Claim.Type == "email").Value;
            ViewBag.Email = getEmail;
            if (getEmail != null)
            {
              var find=  db.Users.Where(c => c.Email == getEmail).FirstOrDefault();
                if (find!= null)
                {
                    find.Password = null;
                    db.SaveChanges();
                    return View();
                }
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var find = db.Users.Where(c => c.Email == model.Email).FirstOrDefault();
                find.Password = PasswordHelper.Encrypt(model.NewPassword);
                db.SaveChanges();
                ViewBag.Message = "Password Changed ";
                return View();
            }
            return NotFound();
        }
    }
}