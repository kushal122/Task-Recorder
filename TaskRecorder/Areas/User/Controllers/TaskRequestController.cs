using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskRecorder.Data;
using TaskRecorder.Helper.Mail;
using TaskRecorder.Helper.TokenHelper;
using TaskRecorder_DataModels.Models;

namespace TaskRecorder.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User", AuthenticationSchemes = "User_Schema")]
    public class TaskRequestController : Controller
    {
        private readonly ConnectionDBContext db;
        readonly MailHelper mailHelper = new MailHelper();
        public TaskRequestController(ConnectionDBContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public IActionResult ListTask()
        {
            var listTask = db.Tasks.ToList();
            return View(listTask);
        }
        [HttpGet]
        public IActionResult ListMyRequestTask()
        {
            var findEmail = User.Identity.Name;
            var userId = db.Users.Where(c => c.Email == findEmail).Select(c => c.UserId).FirstOrDefault();
            var listTask = db.TaskRequests.Where(c => c.UserId == userId).ToList();
            return View(listTask);
        }
        [HttpGet]
        public IActionResult Details(string taskId)
        {
            if (taskId == null)
            {
                return NotFound();
            }
            var task = db.Tasks.Where(c => c.TaskId == taskId).Single();
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }
        [HttpGet]
        public IActionResult TaskRequest(string taskId)
        {
            if (taskId == null)
            {
                return NotFound();
            }
            var data = db.Tasks.Where(c => c.TaskId == taskId).FirstOrDefault();
            if (data == null)
            {
                ViewBag.Message = "Cannot Process the Request";
                return NotFound();
            }

            var findEmail = User.Identity.Name;
            if (findEmail == null)
            {
                ViewBag.Message = "Cannot Process the Request";
                return NotFound();
            }
            var userId = db.Users.Where(c => c.Email == findEmail).Select(c => c.UserId).AsNoTracking().FirstOrDefault();
            TaskRequest model = new TaskRequest();
            model.TaskId = taskId;
            model.UserId = userId;
            model.RequestDate = DateTime.UtcNow;
            model.DeadLine = data.DeadLine;
            model.Status = "Not Confirmed";
            model.Description = data.Description;
            db.TaskRequests.Add(model);
            db.SaveChanges();

            var email = db.Users.Where(c => c.UserId == userId).Select(c => c.Email).FirstOrDefault();
            if (email != null)
            {
                var token = JwtToken.GenerateToken(userId, taskId, email);
                var confirmationLink = Url.Action("ConfirmTask", "TaskRequest", new { Areas = "User", userId = userId, token = token }, Request.Scheme);
                var To = email;
                mailHelper.Send(To,confirmationLink);

            return RedirectToAction(nameof(ListMyRequestTask));
            }
            ViewBag.Error = "Could not send the message.Please Contact Respective Administration.";
            return RedirectToAction(nameof(ListMyRequestTask));
        }
        public async Task<IActionResult> ConfirmTask(string token)
        {
            var email = User.Identity.Name;
            var userId = db.Users.Where(c => c.Email == email).Select(c => c.UserId).FirstOrDefault();

            var handeler = new JwtSecurityTokenHandler();
            var jwtToken = handeler.ReadToken(token) as JwtSecurityToken;
            var getEmail = jwtToken.Claims.First(Claim => Claim.Type == "email").Value;
            var getUserId = jwtToken.Claims.First(Claim => Claim.Type == "unique_name").Value;
            var taskId = jwtToken.Claims.First(Claim => Claim.Type == "given_name").Value;
            if (email == getEmail)
            {
                if (token == null)
                {
                    String url1 = string.Format("/Home/Home/Index");
                    return Redirect(url1);
                }
                else if (userId == null)
                {
                    String url1 = string.Format("/User/UserLogin/SignIn");
                    return Redirect(url1);
                }
                var check = JwtToken.ValidateCurrentToken(token);
                if (check == true)
                {
                    var confirmTask = await db.TaskRequests.Where(c => c.UserId == getUserId && c.TaskId == taskId && c.RequestConfirmed == false).FirstOrDefaultAsync();
                    if (confirmTask != null)
                    {
                        confirmTask.RequestConfirmed = true;
                        db.TaskRequests.Update(confirmTask);
                        ViewBag.Message = "Thank You For Confirming";
                        db.SaveChanges();
                    }
                    else
                    {
                        ViewBag.Message = "Error Occured. Please Contact Us For Further Detail. ";
                    }
                }
                else
                {
                    ViewBag.Message = "Error Occured. Please Contact Us For Further Detail. ";
                }
            }
            else
            {
                ViewBag.Message = "Error Occured. Email Do not Match ";
            }
            return View();
        }
    }
}