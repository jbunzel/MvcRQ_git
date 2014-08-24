using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;
using MvcRQ.Helpers;

namespace MvcRQ.Areas.ItemViewer.Helpers
{
    public static class AccessControl
    {
        public static bool HasAccess(string digialObjectAdress)
        {
            return true;
        }

        public static string GetAccessRightsCode()
        {
            string role = AccessRightsResolver.GetUpmostPrivilege();

            switch (role)
            {
                case "Admin": 
                    return "a";
                case "Member": 
                    return "a";
                case "Guest": 
                    return "";
                default:
                    return "";
            }
        }

        public static string AppendAccessRightsCode(string itemAdress)
        {
            string accessRightsCode = Helpers.AccessControl.GetAccessRightsCode();
            string pattern = @"(?<id>(MyDocs|MyMusic|MyVideos)/[0-9]{5})";

            if (Regex.IsMatch(itemAdress, pattern))
            {
                string replacement = Regex.Match(itemAdress, pattern).Groups["id"].Value + accessRightsCode;

                return Regex.Replace(itemAdress, pattern, replacement);
            }
            else
                return "";
        }
    }
}