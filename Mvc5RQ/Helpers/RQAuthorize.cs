using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Mvc5RQ.Models;

namespace Mvc5RQ.Helpers
{
    //public class RQAuthorizeAttribute : AuthorizeAttribute
    //{
    //    public override void OnAuthorization(AuthorizationContext filterContext)
    //    {
    //        base.OnAuthorization(filterContext);
    //        if (filterContext.Result is HttpUnauthorizedResult)
    //        {
    //            if (!filterContext.HttpContext.User.IsInRole(this.Roles))
    //                throw new NotImplementedException("No permission to access this action");
    //            else
    //                HandleUnauthorizedRequest(filterContext);
    //        }
    //    }
    //}

    public static class IdentityHelpers
    {
        public static void UpdateLastAccessDate(string userId)
        {
            ApplicationUserManager um = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser au = um.FindByIdAsync(userId).Result;

            au.LastActivityDate = DateTime.Now;
            um.Update(au);
        }
    }
}