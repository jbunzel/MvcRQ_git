using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;

namespace Mvc5RQ
{
    /// <summary>
    /// 
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// 
        /// </summary>
        protected void Application_Start()
        {
            //System.Data.Entity.Database.SetInitializer(new Mvc5RQ.Areas.UserSettings.Models.SettingsDBInitializer());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Application_PostAuthorizeRequest()
        {
            if (WebApiCalledByApplication())
                System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
            else if (User.Identity.IsAuthenticated)
                Mvc5RQ.Helpers.IdentityHelpers.UpdateLastAccessDate(User.Identity.GetUserId());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool WebApiCalledByApplication()
        {
            return (HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest");
        }
    }
}
