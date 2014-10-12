using System;
using System.Web;
using System.Web.Routing;
using System.Web.Security;

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
            MembershipUser user = Membership.GetUser(values[parameterName].ToString());

            if (user == null)
                return String.Compare(values[parameterName].ToString().ToLower(), _match, true) == 0;
            else
                return true;
        }
    }
}