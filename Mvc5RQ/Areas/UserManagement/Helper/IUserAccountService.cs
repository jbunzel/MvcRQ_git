using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using Mvc5RQ.Models;
using System;
//using System.Linq;
//using System.Text;

namespace Mvc5RQ.Areas.UserManagement
{
  public interface IUserAccountService
  {
    List<ApplicationUser> GetAllUsers(int page, int size, out int totalRecords);
    
    Task<List<ApplicationUser>> GetAllUsers();

    IdentityResult DeleteUser(string username);
    
    IdentityResult CreateUser(string username, string password, string[] roles, out ApplicationUser user);

    Array GetAllRoles();

    IList<string> GetRolesForUser(string username);

    IdentityResult AddUserToRoles(ApplicationUser user, string[] roles);

    IdentityResult CreateRole(string roleName);

    IdentityResult DeleteRole(string roleName);

    IdentityResult UnlockUser(string userName);

    IdentityResult AddRemoveRoleForUser(string username, string rolename, bool isInRole);
  }
}
