using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;
using Mvc5RQ.Helpers;

namespace Mvc5RQ.Areas.DigitalObjects.Helpers
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
                case "admin": 
                    return "a";
                case "patron": 
                    return "a";
                case "partner": 
                    return "a";
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