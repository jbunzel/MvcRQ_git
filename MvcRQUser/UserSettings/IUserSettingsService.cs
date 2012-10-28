using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace MvcRQUser.UserSettings
{
  public interface IUserSettingsService
  {
      bool GetIncludeExternal();

      bool ChangeIncludeExternal();

      string[] GetAllExternalDatabases();

      string[] GetExternalDatabasesForUser();

      void AddRemoveDatabaseForUser(string databasename, bool included);

    //MembershipUserCollection GetAllUsers(int page, int size, out int totalRecords);
    //MembershipUserCollection GetAllUsers();

    //void DeleteUser(string username);
    //MembershipUser CreateUser(string username, string password, string email, out MembershipCreateStatus createStatus);

    //string[] GetAllRoles();

    //string[] GetRolesForUser(string username);

    //void AddUserToRoles(MembershipUser user, string[] roles);

    //void CreateRole(string roleName);

    //bool DeleteRole(string roleName, bool throwOnPopulatedRole);

    //bool UnlockUser(string userName);

    //void AddRemoveRoleForUser(string username, string rolename, bool isInRole);
  }
}
