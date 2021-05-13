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
    public class UserDashboardController : Controller
    {
        private readonly ConnectionDBContext db;
        public UserDashboardController(ConnectionDBContext db)
        {
            this.db = db;
        }
        [Route("user/dashboard")]
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Details()
        {
            var email = User.Identity.Name;
            var find = db.Users.Where(c => c.Email == email).FirstOrDefault();
            if (find != null)
            {
                return View(find);
            }
            else
            {
                return NotFound();
            }
        }
    }
}