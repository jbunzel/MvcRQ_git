using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mvc5RQ
{
    /// <summary>
    /// 
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "RQKosBranch",
                "rqkos/{id}",
                new { controller = "RQKos", action = "RQKosBranch", id = UrlParameter.Optional });
            routes.MapRoute(
                "RQItemList",
                "{dbname}",
                new { controller = "RQItems", action = "RQItemList" },
                new { dbname = new Mvc5RQ.Helpers.IsDBName() });
            routes.MapRoute(
                "RQItemRecord",
                "{dbname}/{rqitemId}",
                new { controller = "RQItems", action = "RQItemRecord" },
                new { dbname = new Mvc5RQ.Helpers.IsDBName(), rqItemId = new Mvc5RQ.Helpers.IsRQItemId() });
            routes.MapRoute(
                "RQItemSubField",
                "{dbname}/{rqitemId}/{fieldName}/{subFieldIndex}",
                new { controller = "RQItems", action = "RQItemDescElement" },
                new { dbname = new Mvc5RQ.Helpers.IsDBName(), rqItemId = new Mvc5RQ.Helpers.IsRQItemId() });
            routes.MapRoute(
                "RQItemField",
                "{dbname}/{rqitemId}/{fieldName}",
                new { controller = "RQItems", action = "RQItemDescElement" },
                new { dbname = new Mvc5RQ.Helpers.IsDBName(), rqItemId = new Mvc5RQ.Helpers.IsRQItemId() });
            routes.MapRoute(
                "Database", // Route name
                "{dbname}/{action}/{rqitemId}", // URL with parameters
                new { controller = "RQItems", action = "RQItemList", rqitemId = UrlParameter.Optional },
                new { dbname = new Mvc5RQ.Helpers.IsDBName() });
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }
    }
}
