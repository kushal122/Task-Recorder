using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskRecorder.Data;
using TaskRecorder.Helper.Mail;
using TaskRecorder.Helper.TokenHelper;
using TaskRecorder_DataModels.Models;

namespace TaskRecorder.Areas.Administration.Controllers
{
    [Area("Administration")]   
    public class TaskController : Controller
    {
        private readonly ConnectionDBContext db;
        readonly MailHelper mailHelper = new MailHelper();
        public TaskController(ConnectionDBContext db)
        {
            this.db = db;
        }
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Admin_Schema")]
        [HttpGet]
        public IActionResult ListTask()
        {
            var listTask = db.Tasks.ToList();
            return View(listTask);
        }
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Admin_Schema")]
        public IActionResult SearchTerm(string SearchString)
        {
            var Datas = from u in db.Tasks select u;
            if (!String.IsNullOrWhiteSpace(SearchString))
            {
                Datas = Datas.Where(c => c.UserId.Contains(SearchString) || c.Title.Contains(SearchString) || c.Status.Contains(SearchString) || c.Description.Contains(SearchString));
                return View("ListTask", Datas);
            }
            ViewBag.Message = "No Record Found";
            return View("ListTask");
        }
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Admin_Schema")]
        [HttpGet]
        public IActionResult CreateTask()
        {
            ViewData["users"] = new SelectList(db.Users.ToList(), "UserId", "Email");
            return View();
        }
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Admin_Schema")]
        [HttpPost]
        public IActionResult CreateTask(TasksModel task)
        {
            if (ModelState.IsValid)
            {
                task.TaskId = Guid.NewGuid().ToString("n");
                if (task.AssignedTo != null)
                {
                    var taskId = task.TaskId;
                    var find = db.Users.Where(c => c.UserId == task.AssignedTo).FirstOrDefault();

                    TaskRequest model = new TaskRequest();
                    model.TaskId = taskId;
                    model.UserId = find.UserId;
                    model.RequestDate = DateTime.UtcNow;
                    model.DeadLine = task.DeadLine;
                    model.Status = "Not Confirmed";
                    model.Description = task.Description;
                    db.TaskRequests.Add(model);
                    db.SaveChanges();                    
                    
                    var token = JwtToken.GenerateToken(find.UserId, taskId, find.Email);
                    var confirmationLink = Url.Action("ConfirmTask", "Task", new { Areas = "Administration", userId = find.UserId, token = token }, Request.Scheme);
                    var To = find.Email;
                    mailHelper.Send(To, confirmationLink);
                }
                db.Tasks.Add(task);
                db.SaveChanges();
                ViewBag.Message = "Added Succesfully";
                return RedirectToAction(nameof(ListTask));
            }
            ViewBag.Message = "Cannot Process. Try Again";
            return View();
        }
        [Authorize(Roles = "Admin,User", AuthenticationSchemes = "Admin_Schema,User_Schema")]
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
                        confirmTask.AdminResponse = true;
                        db.TaskRequests.Update(confirmTask);
                        ViewBag.Message = "Thank You For Confirming";
                        db.SaveChanges();

                        var findTask = db.Tasks.Where(c => c.TaskId == taskId ).FirstOrDefault();
                        if (findTask != null)
                        {
                            var emails = db.Users.Where(c => c.UserId == userId).Select(c => c.Email).FirstOrDefault();
                            findTask.UserId = userId;
                            findTask.AssignedTo = emails;
                            findTask.IsConfirmed = true;
                            db.Tasks.Update(findTask);
                            db.SaveChanges();
                        }
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
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Admin_Schema")]
        [HttpGet]
        public IActionResult EditTask(string taskId)
        {
            if (taskId == null)
            {
                return NotFound();
            }
            var findTask = db.Tasks.Where(c => c.TaskId == taskId).Single();
            if (findTask == null)
            {
                return NotFound();
            }
            return View(findTask);

        }
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Admin_Schema")]
        [HttpPost]
        public IActionResult EditTask(TasksModel task)
        {
            if (ModelState.IsValid)
            {
                db.Tasks.Update(task);
                db.SaveChanges();
                return RedirectToAction(nameof(ListTask));
            }
            ViewBag.Message = "Cannot Process. Try Again";
            return View(task);
        }
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Admin_Schema")]
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
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Admin_Schema")]
        public async Task<IActionResult> DeleteTask(string taskId)
        {
            var task = await db.Tasks.Where(c => c.TaskId == taskId).FirstOrDefaultAsync();
            if (task == null)
            {
                ViewBag.ErrorMessage = $"Task with Id = {taskId} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = db.Tasks.Remove(task);
                if (result != null)
                {
                    db.SaveChanges();
                    return RedirectToAction("ListTask");
                }
                else
                {
                    ViewBag.Message = "Unable Process the Request";
                    return View("ListTask");
                }
            }
        }
    }
}