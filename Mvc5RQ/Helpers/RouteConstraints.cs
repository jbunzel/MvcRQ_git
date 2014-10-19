using System;
using System.Web;
using System.Web.Routing;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Mvc5RQ.Models;

namespace Mvc5RQ.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class IsDBName : IRouteConstraint
    {
        string _match = "rqitems";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="route"></param>
        /// <param name="parameterName"></param>
        /// <param name="values"></param>
        /// <param name="routeDirection"></param>
        /// <returns></returns>
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            //ApplicationUserManager um = httpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //Task<ApplicationUser> au = um.FindByNameAsync(values[parameterName].ToString());

            //if (au.Result == null)
            //    return String.Compare(values[parameterName].ToString().ToLower(), _match, true) == 0;
            //else
            //    return true;
            if (values[parameterName].ToString().ToLower() == "rqitems")
                return true;
            else
                return false;
        }
    }
}