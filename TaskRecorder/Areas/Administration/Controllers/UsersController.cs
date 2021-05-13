using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskRecorder.Areas.Administration.Models.ViewModels;
using TaskRecorder.Data;
using TaskRecorder_DataModels.Models;

namespace TaskRecorder.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = "Admin_Schema")]
    public class UsersController : Controller
    {
        private readonly ConnectionDBContext db;
        public UsersController(ConnectionDBContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public IActionResult ListUsers()
        {
            var user = db.Users.ToList();
            return View(user);
        }
        public IActionResult SearchTerm(string SearchString)
        {
            var Datas = from u in db.Users select u;
            if (!String.IsNullOrWhiteSpace(SearchString))
            {
                Datas = Datas.Where(c => c.UserId.Contains(SearchString) || c.Email.Contains(SearchString) || c.FirstName.Contains(SearchString) || c.LastName.Contains(SearchString) || c.PhoneNumber.Contains(SearchString) );
                return View("ListUsers", Datas);
            }
            ViewBag.Message = "No Record Found";
            return View("ListUsers");
        }
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await db.Users.Where(c => c.UserId == id).FirstOrDefaultAsync();
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            var userRoles = await db.UsersRoles.Where(c => c.UserId == user.UserId).Select(c => c.Roles.RoleName).ToListAsync();
            var model = new EditUserViewModel
            {
                UserId = user.UserId,
                Email = user.Email,
                Address = user.Address,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = userRoles
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await db.Users.Where(c => c.UserId == model.UserId).FirstOrDefaultAsync();
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("Not Found");
            }
            else
            {
                user.Email = model.Email;
                user.Address = model.Address;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;

                var result = db.Users.Update(user);
                if (result != null)
                {
                    db.SaveChanges();
                    return RedirectToAction("ListUsers");
                }
                else
                {
                    ViewBag.Message = "Unable Process the Request";
                    return View(model);
                }
            }
        }
        [HttpGet]
        public IActionResult Details(string id)
        {
            var user = db.Users.Where(c => c.UserId == id).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await db.Users.Where(c => c.UserId == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("Not Found");
            }
            var model = new List<UserRoleViewModel>();
            var allUser = db.Users.ToList();
            var findUserWithRole = db.Roles;
            foreach (var item in findUserWithRole)
            {
                var userRole = new UserRoleViewModel
                {
                    RoleId = item.RoleId,
                    RoleName = item.RoleName
                };
                var data = db.UsersRoles.Where(c => c.UserId == userId && c.RoleId == item.RoleId).FirstOrDefault();
                if (data != null)
                {
                    userRole.IsSelected = true;
                }
                else
                {
                    userRole.IsSelected = false;
                }
                model.Add(userRole);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRoleViewModel> model, string userId)
        {
            var user = await db.Users.Where(c => c.UserId == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("Not Found");
            }
            var roles = await db.UsersRoles.Where(c => c.UserId == userId).Select(c => c.RoleId).FirstOrDefaultAsync();
            dynamic result = null;
            if (roles != 0)
            {
                UsersRoles userRole = new UsersRoles();
                userRole.RoleId = roles;
                userRole.UserId = userId;

                if (userRole != null)
                {
                    result = db.UsersRoles.Remove(userRole);
                    if (result == null)
                    {
                        ModelState.AddModelError("", "Cannot remove user existing roles");
                        return View(model);
                    }
                    db.SaveChanges();
                }
                else
                {
                }
            }
            else { }
            UsersRoles addUserRole = new UsersRoles();
            var getId = 0;

            for (int i = 0; i < model.Count; i++)
            {
                if (model[i].IsSelected && db.UsersRoles.Where(c => c.UserId == userId && c.RoleId == model[i].RoleId).FirstOrDefault() == null)
                {
                    if (db.UsersRoles.Count() != 0)
                    {
                        getId = db.UsersRoles.Max(x => x.Id);
                    }
                    else
                    {
                        getId = db.UsersRoles.Count();
                    }

                    addUserRole.Id = getId + 1;
                    addUserRole.UserId = userId;
                    addUserRole.RoleId = model[i].RoleId;
                    result = db.UsersRoles.Add(addUserRole);
                    await db.SaveChangesAsync();
                }
                else
                {
                    continue;
                }

            }
            return RedirectToAction("EditUser", new { Id = userId });
        }
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await db.Users.Where(c => c.UserId == Id).FirstOrDefaultAsync();
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = db.Users.Remove(user);

                var users = db.Users.ToList();
                if (result != null)
                {
                    db.SaveChanges();
                    return RedirectToAction("ListUsers",users);
                }
                else
                {
                    ViewBag.Message = "Unable Process the Request";
                    return View("ListUsers",users);
                }
            }
        }
    }
}