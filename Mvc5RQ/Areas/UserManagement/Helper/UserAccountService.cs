using Mvc5RQ.Models;
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

namespace Mvc5RQ.Areas.UserManagement
{
    public class UserAccountService : IUserAccountService
    {
        Controller _controller;

        public UserAccountService()
            : this(null)
        {

        }

        public UserAccountService(Controller controller)
        {
            _controller = controller;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? _controller.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? _controller.HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public async Task<List<ApplicationUser>> GetAllUsers()
        {
            return await UserManager.Users.ToListAsync();
        }

        /// <summary>
        /// Returns all registered users.
        /// </summary>
        /// <returns>All registered membership users.</returns>
        public List<ApplicationUser> GetAllUsers(int page, int size, out int totalRecords)
        {
            Task<List<ApplicationUser>> res =  GetAllUsers();

            totalRecords = res.Result.Count;
            return res.Result;
        }

        /// <summary>
        /// Deletes a user all his membership information by User ID.
        /// </summary>
        /// <param name="userId">The users ProviderUserKey</param>
        public IdentityResult DeleteUser(string  username)
        {
            ApplicationUser user = UserManager.FindByEmail(username);

            return UserManager.Delete(user);
        }

        public IdentityResult CreateUser(string email, string password, string[] roles, out ApplicationUser user) //, out MembershipCreateStatus createStatus)
        {
            user = new ApplicationUser {UserName = email, Email = email, RegisterDate= DateTime.Now, LastActivityDate = DateTime.Now};

            IdentityResult res = UserManager.Create(user, password);
            if ((res.Succeeded) && (roles != null))
                return UserManager.AddToRoles(user.Id, roles);
            else
                return res;
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>  A string array containing the names of all the roles stored in the data source
        ///    for the configured applicationName.</returns>
        public Array GetAllRoles()
        {
            return new SelectList(RoleManager.Roles.ToList(), "Id", "Name").ToArray();
        }

        /// <summary>
        ///  Gets a list of the roles that a specified user is in for the configured applicationName.
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns> A string array containing the names of all the roles that the specified user
        ///    is in for the configured applicationName.</returns>
        public IList<string> GetRolesForUser(string username)
        {
            return UserManager.GetRoles(username);
        }

        public IdentityResult AddUserToRoles(ApplicationUser user, string[] roles)
        {
            //_roleProvider.AddUsersToRoles(new string[] { user.UserName }, roles);
            return null;
        }

        public IdentityResult CreateRole(string roleName)
        {
            var role = new IdentityRole(roleName);

            return RoleManager.Create(role);
        }

        public IdentityResult DeleteRole(string roleName)
        {
            IdentityRole role = RoleManager.FindByName(roleName);

            if (role != null)
                return RoleManager.Delete(role);
            else
                return null;
        }

        public IdentityResult UnlockUser(string userName)
        {
            //return _membership.UnlockUser(userName);
            return null;
        }

        public IdentityResult AddRemoveRoleForUser(string userId, string rolename, bool isInRole)
        {
            string roleId = RoleManager.FindByName(rolename).Id;

            if (isInRole)
                return UserManager.AddToRole(userId, rolename); //(new string[] { userId }, new string[] { rolename });
            else
                return UserManager.RemoveFromRole(userId, rolename); //(new string[] { userId }, new string[] { rolename });
        }
    }
}
