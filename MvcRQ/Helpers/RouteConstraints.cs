using System;
using System.Web;
using System.Web.Routing;
using System.Web.Security;

namespace MvcRQ.Helpers
{
    public class IsDBName : IRouteConstraint
    {
        string _match = "rqitems";

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