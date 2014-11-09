using System;
using System.Web;
using System.Web.Routing;
using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using Microsoft.AspNet.Identity.Owin;
//using Mvc5RQ.Models;

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
            if (values[parameterName].ToString().ToLower() == "rqitems")
                return true;
            else
                return false;
        }
    }

    public class IsRQItemId : IRouteConstraint
    {
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
            var regex = new Regex("[0-9]{5}$");

            if (regex.IsMatch(values[parameterName].ToString().ToLower()))
                return true;
            else
                return false;
        }
    }
}