using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MvcRQ.Helpers
{
    public class AccessRightsResolver
    {
        private static string GetUser()
        {
            try
            {
                return Membership.GetUser().UserName;
            }
            catch
            {
                return "";
            }
        }
        
        private static string[] GetUserRoles()
        {
            return Roles.GetRolesForUser();
        }

        public static string GetUpmostPrivilege()
        {
            string[] roles = GetUserRoles();
            string res = "";

            foreach (string val in roles) {
                if (val == "Adminitrators") res = "Admin";
                if (val == "Members") 
                    if (res != "Admin") res = "Member";
                if (val == "Guest") 
                    if ((res != "Admin") && (res != "Member")) res = "Guest";
            }
            return res;
        }

        public static string DecodeAccessRights(string code)
        {
            //Access rights (coded syntax) NOTE: FieldLength = 50
            //Not yet implemented.
            //Access rights (decoded syntax) 
            //dbowners=<username>; license=<name>; 
            //{  view={All   | Administrators | Members | Guests | { <groupname> , <groupname>, ... , <groupname> } | { <username> , <username>, ... , <username> };  
            //   edit={All   | Administrators | Members | Guests | { <groupname> , <groupname>, ... , <groupname> } | { <username> , <username>, ... , <username> };
            //   copy={All   | Administrators | Members | Guests | { <groupname> , <groupname>, ... , <groupname> } | { <username> , <username>, ... , <username> };
            //    delete={All | Administrators | Members | Guests | { <groupname> , <groupname>, ... , <groupname> } | { <username> , <username>, ... , <username> }; }
            string m = "dbowners=jbunzel; license=GNU; view=All; edit=Administrators; copy=Administrators; delete=Administrators;";
            string n = "dbowners=jbunzel; license=GNU; view=All; copy=Administrators;";
            
            return code == "EXTITEM_AR_CODE" ? n : m;
        }

        public static string EncodeAccessRights(string accessRights)
        {
            //Not yet implemented.
            return accessRights == "EXTERNAL" ? "EXTITEM_AR_CODE" : "DBITEM_AR_CODE";
        }

        public static bool HasAdminAccess()
        {
            string[] roles = GetUserRoles();

            if (roles.Contains<string>("Administrators"))
                return true;
            else
                return false;
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
            string[] roles = GetUserRoles();
            string user = GetUser();
            string rights;

            roles = GetUserRoles();
            accessRights += " actual=";
            rights = accessRights.Substring(accessRights.IndexOf("view=") + "view=".Length);
            rights = rights.Substring(0, rights.IndexOf(";"));
            if ((rights == "All") || (rights == user) || roles.Contains<string>(rights))
                accessRights += "view";
            rights = accessRights.Substring(accessRights.IndexOf("copy=") + "copy=".Length);
            rights = rights.Substring(0, rights.IndexOf(";"));
            if ((rights == "All") || (rights == user) || roles.Contains<string>(rights))
                accessRights += "-copy";
            rights = accessRights.Substring(accessRights.IndexOf("edit=") + "edit=".Length);
            rights = rights.Substring(0, rights.IndexOf(";"));
            if ((rights == "All") || (rights == user) || roles.Contains<string>(rights))
                accessRights += "-edit";
            rights = accessRights.Substring(accessRights.IndexOf("delete=") + "delete=".Length);
            rights = rights.Substring(0, rights.IndexOf(";"));
            if ((rights == "All") || (rights == user) || roles.Contains<string>(rights))
                accessRights += "-delete";
            return accessRights;
        }
    }
}