using Mvc5RQ.Models;
using Mvc5RQ.Areas.UserManagement.Models;
using Mvc5RQ.Exceptions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mvc5RQ.Areas.UserManagement.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserManagementController : Mvc5RQ.Controllers.BaseController
    {
        private UserAccountService _userAccountService;

        public UserManagementController()
        {
            _userAccountService = new UserAccountService(this);
        }

        public UserManagementController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            _userAccountService = new UserAccountService(this);
        }

        //
        // GET: /Users/
        public async Task<ActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// Returns a list of all current users
        /// </summary>
        /// <returns>All known membership users</returns>
        public async Task<JsonResult> GetAllUsers()
        {
            List<ApplicationUser> result = await _userAccountService.GetAllUsers();

            if (result.Count <= 0)
                throw new Exception(RQResources.Views.Shared.SharedStrings.um_no_users_found);
            else
                return Json(result);
        }

        /// <summary>
        /// Gets a list of roles including the information wether the user is in that role or not.
        /// </summary>
        /// <param name="username">The user which role information should be gathered.</param>
        /// <returns>A list of roles including the information wether the user is in that role or not.</returns>
        public JsonResult GetUserRoleStatus(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                throw new ArgumentException("No user id specified in request");

            var allRoles = _userAccountService.GetAllRoles();
            var userRoles = _userAccountService.GetRolesForUser(Id);

            var result = new MyJsonResult()
            {
                data = from SelectListItem role in allRoles
                       select new
                       {
                           rolename = role.Text,
                           isInRole = userRoles.Contains(role.Text)
                       },
                isSuccess = true
            };
            return Json(result);
        }

        /// <summary>
        /// Adds or removes a role for a user account.
        /// </summary>
        /// <param name="username">The user which roles should be modified.</param>
        /// <param name="rolename">The role which should be added or removed.</param>
        /// <param name="isInRole">The new role status for the user account. If false, the role will be deleted for the user account.</param>
        /// <returns></returns>
        public JsonResult AddRemoveRoleForUser(string userId, string rolename, bool isInRole)
        {
            MyJsonResult result;
            IdentityResult res = _userAccountService.AddRemoveRoleForUser(userId, rolename, isInRole);
            
            if (res.Succeeded)
            {
                var action = isInRole ? "added" : "removed";
                var msg = string.Format("The role {0} has been {1} for user {2}.", rolename, action, userId);

                result = MyJsonResult.CreateSuccess(msg);
            }
            else
            {
                ModelState.AddModelError("", res.Errors.First());
                result = MyJsonResult.CreateError("Could not remove role for user: " + res.Errors);
            }
            return Json(result);
        }

        /// <summary>
        /// Creates a new user account
        /// </summary>
        /// <param name="username">A unique username</param>
        /// <param name="password">A hopefully secure password</param>
        /// <param name="email">A unique email address</param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public JsonResult Create(CreateUserViewModel userViewModel)
        {
            MyJsonResult result;

            if (ModelState.IsValid)
            {
                ApplicationUser user; 
                IdentityResult res = _userAccountService.CreateUser(userViewModel.Email, userViewModel.Password, userViewModel.roles, out user);

                if (res.Succeeded)
                {
                    result = new MyJsonResult()
                    {
                        data = user,
                        isSuccess = true
                    };
                }
                else
                {
                    ModelState.AddModelError("", res.Errors.First());
                    result = MyJsonResult.CreateError(string.Format("Error on adding user {0} to the database: ", userViewModel.Email) + res.Errors);
                }
            }
            else
                result = MyJsonResult.CreateError(string.Format("Error on adding user {0} to the database: ", userViewModel.Email) + "Invalid user model.");
            return Json(result);
        }

        /// <summary>
        /// Deletes a user from through the membership service.
        /// </summary>
        /// <param name="userId">The id of the user account which should be deleted.</param>
        /// <returns>Result info for the user account deletion action.</returns>
        public JsonResult Delete(string username)
        {
            MyJsonResult result;
            IdentityResult res = _userAccountService.DeleteUser(username);
            
            if (res.Succeeded)
                result = MyJsonResult.CreateSuccess(string.Format("User {0} has been deleted.", username));
            else
                result = MyJsonResult.CreateError(string.Format("Error on deleting user {0} from the database: ", username) + res.Errors);
            return Json(result);
        }

        ////
        //// GET: /Users/Details/5
        //public async Task<ActionResult> Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var user = await UserManager.FindByIdAsync(id);

        //    ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

        //    return View(user);
        //}

        ////
        //// GET: /Users/Create
        //public async Task<ActionResult> Create()
        //{
        //    //Get the list of Roles
        //    ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
        //    return View();
        //}

        ////
        //// POST: /Users/Create
        //[HttpPost]
        //public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
        //        var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

        //        //Add User to the selected Roles 
        //        if (adminresult.Succeeded)
        //        {
        //            if (selectedRoles != null)
        //            {
        //                var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
        //                if (!result.Succeeded)
        //                {
        //                    ModelState.AddModelError("", result.Errors.First());
        //                    ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
        //                    return View();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", adminresult.Errors.First());
        //            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
        //            return View();

        //        }
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
        //    return View();
        //}

        ////
        //// GET: /Users/Edit/1
        //public async Task<ActionResult> Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var user = await UserManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    var userRoles = await UserManager.GetRolesAsync(user.Id);

        //    return View(new EditUserViewModel()
        //    {
        //        Id = user.Id,
        //        Email = user.Email,
        //        RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
        //        {
        //            Selected = userRoles.Contains(x.Name),
        //            Text = x.Name,
        //            Value = x.Name
        //        })
        //    });
        //}

        ////
        //// POST: /Users/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Email,Id")] EditUserViewModel editUser, params string[] selectedRole)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByIdAsync(editUser.Id);
        //        if (user == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        user.UserName = editUser.Email;
        //        user.Email = editUser.Email;

        //        var userRoles = await UserManager.GetRolesAsync(user.Id);

        //        selectedRole = selectedRole ?? new string[] { };

        //        var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

        //        if (!result.Succeeded)
        //        {
        //            ModelState.AddModelError("", result.Errors.First());
        //            return View();
        //        }
        //        result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

        //        if (!result.Succeeded)
        //        {
        //            ModelState.AddModelError("", result.Errors.First());
        //            return View();
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    ModelState.AddModelError("", "Something failed.");
        //    return View();
        //}

        ////
        //// GET: /Users/Delete/5
        //public async Task<ActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var user = await UserManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        ////
        //// POST: /Users/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(string id)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }

        //        var user = await UserManager.FindByIdAsync(id);
        //        if (user == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        var result = await UserManager.DeleteAsync(user);
        //        if (!result.Succeeded)
        //        {
        //            ModelState.AddModelError("", result.Errors.First());
        //            return View();
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}
    }
}
