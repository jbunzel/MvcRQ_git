using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcRQ.Helpers
{
    public class RQAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                if (!filterContext.HttpContext.User.IsInRole(this.Roles))
                    throw new NotImplementedException("No permission to access this action");
                else
                    HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}