using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Mvc5RQ.Models;

namespace Mvc5RQ.Helpers
{
    public class AccessRightsResolver
    {
        public static string DecodeAccessRights(string code)
        {
            //Access rights (coded syntax) NOTE: FieldLength = 50
            //Not yet implemented.
            //Access rights (decoded syntax) 
            //dbowners=<username>; license=<name>; 
            //{  view={all   | admin | patron | partner | guest | { <groupname> , <groupname>, ... , <groupname> } | { <username> , <username>, ... , <username> };  
            //   edit={all   | admin | patron | partner | guest | { <groupname> , <groupname>, ... , <groupname> } | { <username> , <username>, ... , <username> };
            //   copy={all   | admin | patron | partner | guest | { <groupname> , <groupname>, ... , <groupname> } | { <username> , <username>, ... , <username> };
            //   delete={all | admin | patron | partner | guest | { <groupname> , <groupname>, ... , <groupname> } | { <username> , <username>, ... , <username> }; }
            string m = "dbowners=jbunzel; license=GNU; view=all; edit=admin; copy=admin; delete=admin;";
            string n = "dbowners=jbunzel; license=GNU; view=all; copy=admin;";
            
            return code == "EXTITEM_AR_CODE" ? n : m;
        }

        public static string EncodeAccessRights(string accessRights)
        {
            //Not yet implemented.
            return accessRights == "EXTERNAL" ? "EXTITEM_AR_CODE" : "DBITEM_AR_CODE";
        }

        public static bool HasAdminAccess()
        {
            return HttpContext.Current.User.IsInRole("admin");
        }

        public static bool HasKosEditAccess()
        {
            return HasAdminAccess();
        }

        public static bool HasEditAccess()
        {
            return HasAdminAccess();
        }

        public static bool HasAddAccess()
        {
            return HasAdminAccess();
        }

        public static bool HasViewAccess(string accessRights)
        {
            return true;
        }
        
        public static string ResolveListAccessRights()
        {
            return ""; 
        }

        public static string ResolveItemAccessRights(string accessRights)
        {
            //string[] roles = GetUserRoles();
            //string user = GetUser()
            string user = HttpContext.Current.User.Identity.GetUserName();
            string rights;

            //roles = GetUserRoles();
            accessRights += " actual=";
            rights = accessRights.Substring(accessRights.IndexOf("view=") + "view=".Length);
            rights = rights.Substring(0, rights.IndexOf(";"));
            if ((rights == "all") || (rights == user) || HttpContext.Current.User.IsInRole(rights))
                accessRights += "view";
            rights = accessRights.Substring(accessRights.IndexOf("copy=") + "copy=".Length);
            rights = rights.Substring(0, rights.IndexOf(";"));
            if ((rights == "all") || (rights == user) || HttpContext.Current.User.IsInRole(rights))
                accessRights += "-copy";
            rights = accessRights.Substring(accessRights.IndexOf("edit=") + "edit=".Length);
            rights = rights.Substring(0, rights.IndexOf(";"));
            if ((rights == "all") || (rights == user) || HttpContext.Current.User.IsInRole(rights))
                accessRights += "-edit";
            rights = accessRights.Substring(accessRights.IndexOf("delete=") + "delete=".Length);
            rights = rights.Substring(0, rights.IndexOf(";"));
            if ((rights == "all") || (rights == user) || HttpContext.Current.User.IsInRole(rights))
                accessRights += "-delete";
            return accessRights;
        }
    }
}