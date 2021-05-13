using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskRecorder.Data;

namespace TaskRecorder.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User", AuthenticationSchemes = "User_Schema")]
    public class UsersTaskController : Controller
    {
        private readonly ConnectionDBContext db;
        public UsersTaskController(ConnectionDBContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public IActionResult ListAssignedTask()
        {
            var email = User.Identity.Name;
            var userId = db.Users.Where(c => c.Email == email).Select(c => c.UserId).FirstOrDefault();
            var getMyTask = db.Tasks.Where(c => c.UserId == userId).ToList();
            if (getMyTask != null)
            {
                return View(getMyTask);
            }
            else
            {
                ViewBag.Message = "Not Found";
                return View();
            }
        }
        [HttpGet]
        public IActionResult Completed(string taskId)
        {
            var email = User.Identity.Name;
            var userId = db.Users.Where(c => c.Email == email).Select(c => c.UserId).FirstOrDefault();
            if (taskId != null)
            {
                var find = db.Tasks.Where(c => c.TaskId == taskId && c.UserId == userId).FirstOrDefault();
                find.Status = "Task Completed";
                db.Tasks.Update(find);
                db.SaveChanges();
                var getMyTask = db.Tasks.Where(c => c.UserId == userId).ToList();
                return View("ListAssignedTask", getMyTask);
            }
            return NotFound();
        }
    }
}