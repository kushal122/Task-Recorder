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
    public class RoleController : Controller
    {
        private readonly ConnectionDBContext db;
        public RoleController(ConnectionDBContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public IActionResult ListRoles()
        {
            var listRoles = db.Roles.ToList();
            return View(listRoles);
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                Roles data = new Roles();
                data.RoleName = model.RoleName;
                if (model.RoleName != null)
                {
                    db.Roles.Add(data);
                    await db.SaveChangesAsync();
                    return View();
                }
                else
                {
                    ViewBag.Message = "Unable Process the Request";
                    return View(model);
                }
            }
            ViewBag.Message = "Unable Process the Request";
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(int id)
        {
            var role = await db.Roles.FindAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id ={id} Cannot be Found";
                return View("NotFound");
            }
            var userEmail = await db.UsersRoles.Where(c => c.RoleId == role.RoleId).Select(c => c.Users.Email).ToListAsync();
            var model = new EditRoleViewModel
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Users = userEmail

            };
            var findUserInRole = await db.UsersRoles.Where(c => c.RoleId == id).ToListAsync();
            if (findUserInRole != null)
            {
                return View(model);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await db.Roles.FindAsync(model.RoleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={model.RoleId} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.RoleName = model.RoleName;
                var result = db.Roles.Update(role);
                if (result != null)
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("ListRoles");
                }
                else
                {
                    ViewBag.Message = "Unable Process the Request";
                    return View(model);
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(int roleId)
        {
            ViewBag.roleId = roleId;
            ViewBag.roleName = await db.Roles.Where(c => c.RoleId == roleId).Select(c => c.RoleName).FirstOrDefaultAsync();
            var checkRole = await db.Roles.Where(c => c.RoleId == roleId).FirstOrDefaultAsync();
            if (checkRole == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={roleId} cannot be found";
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();
            var allUser = db.Users.ToList();
            var findUserWithRole = db.Users;
            foreach (var item in findUserWithRole)
            {
                var userRole = new UserRoleViewModel
                {
                    UserId = item.UserId,
                    RoleId = item.UsersRoles.Select(c => c.RoleId).FirstOrDefault(),
                    Email = item.Email
                };
                var data = db.UsersRoles.Where(c => c.UserId == item.UserId && c.RoleId == roleId).FirstOrDefault();
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
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, int roleId)
        {
            var role = await db.Roles.Where(c => c.RoleId == roleId).FirstOrDefaultAsync();
            if (role == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {roleId} cannot be found";
                return View("Not Found");
            }
            var users = await db.UsersRoles.Where(c => c.RoleId == roleId).Select(c => c.UserId).FirstOrDefaultAsync();
            dynamic result = null;
            if (users != null)
            {
                UsersRoles usersRoles = new UsersRoles();
                usersRoles.RoleId = roleId;
                usersRoles.UserId = users;
                if (usersRoles != null)
                {
                    result = db.UsersRoles.Remove(usersRoles);
                    if (result == null)
                    {
                        ModelState.AddModelError("", "Cannot remove user existing roles");
                        return View(model);
                    }
                    db.SaveChanges();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
            UsersRoles addUserRole = new UsersRoles();
            var getId = 0;
            for (int i = 0; i < model.Count; i++)
            {
                if (model[i].IsSelected && db.UsersRoles.Where(c => c.RoleId == roleId && c.UserId == model[i].UserId).FirstOrDefault() == null)
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
                    addUserRole.UserId = model[i].UserId;
                    addUserRole.RoleId = roleId;
                    result = db.UsersRoles.Add(addUserRole);
                    await db.SaveChangesAsync();
                }
                else
                {
                    continue;
                }
            }
            return RedirectToAction("EditRole", new { Id = roleId });
        }
        public IActionResult DeleteRole(int? id)
        {
            var findId = db.Roles.Where(c => c.RoleId == id).FirstOrDefault();
            var listRoles = db.Roles.ToList();
            if (id == null)
            {
                ViewBag.ErrorMessage = $"Task with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = db.Roles.Remove(findId);
                db.SaveChanges();
                return View("ListRoles", listRoles);
            }
        }
    }
}