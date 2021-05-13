using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskRecorder.Data;

namespace TaskRecorder.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = "Admin_Schema")]
    public class TaskRequestsController : Controller
    {
        private readonly ConnectionDBContext db;
        public TaskRequestsController(ConnectionDBContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public IActionResult ListTaskRequests()
        {
            var data = db.TaskRequests.ToList();
            return View(data);
        }
        [HttpGet]
        public IActionResult Details(int requestId)
        {
            var find = db.TaskRequests.Where(c => c.RequestId == requestId).FirstOrDefault();
            return View(find);
        }
        [HttpGet]
        public IActionResult ConfirmRequest(int requestId, string taskId, string userId)
        {
            var find = db.TaskRequests.Where(c => c.RequestId == requestId).FirstOrDefault();
            find.AdminResponse = true;
            find.Status = "Pending";
            db.TaskRequests.Update(find);

            var findTask = db.Tasks.Where(c => c.TaskId == taskId && c.AssignedTo == null).FirstOrDefault();
            if (findTask != null)
            {
                var email = db.Users.Where(c => c.UserId == userId).Select(c => c.Email).FirstOrDefault();
                findTask.UserId = userId;
                findTask.AssignedTo = email;
                findTask.IsConfirmed = true;
                db.Tasks.Update(findTask);
                db.SaveChanges();
            }
            else
            {
                return NotFound();
            }
          
            return View("ListTaskRequests",db.TaskRequests.ToList());
        }
        [HttpGet]
        public IActionResult RejectRequest(int requestId)
        {
            var find = db.TaskRequests.Where(c => c.RequestId == requestId).FirstOrDefault();
            find.Status = "Rejected";
            db.TaskRequests.Update(find);
            db.SaveChanges();
            return View(nameof(ListTaskRequests));
        }
        public IActionResult UserDetails(string userId)
        {
            if (userId != null)
            {
                var find = db.Users.Where(c => c.UserId == userId).Include(c => c.UsersRoles).FirstOrDefault();
                return View(find);
            }
            else
            {
                ViewBag.Message = "No Such Record Found";
                return View();
            }
        }
    }
}