using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvpRestApiLib.NogginBox.MvcExtras.Providers;

namespace MvcRQ
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "ServiceRQKosItem",
                "{serviceId}/rqkos/{id}",
                new { controller = "RQKos", action = "RQKosItemRecord", serviceId = UrlParameter.Optional, id = UrlParameter.Optional });
            routes.MapRoute(
                "RQKosItem",
                "rqkos/{id}",
                new { controller = "RQKos", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute(
                "ServiceRQItemList",
                "{serviceId}/rqitems",
                new { controller = "RQItems", action = "RQItemList"  });
            routes.MapRoute(
                "RQItemList",
                "rqitems",
                new { controller = "RQItems", action = "RQItemList" });
            routes.MapRoute(
                "ServiceRQItemRecord",
                "{serviceId}/rqitems/{rqitemId}",
                new { controller = "RQItems", action = "RQItemRecord"});
            routes.MapRoute(
                "RQItemRecord",
                "rqitems/{rqitemId}",
                new { controller = "RQItems", action = "RQItemRecord" });
            routes.MapRoute(
                "RQItemSubField",
                "rqitems/{rqitemId}/{fieldName}/{subFieldIndex}",
                new { controller = "RQItems", action = "RQItemDescElement" });
            routes.MapRoute(
                "RQItemField",
                "rqitems/{rqitemId}/{fieldName}",
                new { controller = "RQItems", action = "RQItemDescElement" });
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            //System.Data.Entity.Database.SetInitializer(new MvcRQ.Models.SettingsDBInitializer());

            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            // Source: http://haacked.com/archive/2010/04/15/sending-json-to-an-asp-net-mvc-action-method-argument.aspx
            // This must be added to accept JSON as request
            ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
            // This must be added to accept XML as request
            // Source: http://www.nogginbox.co.uk/blog/xml-to-asp.net-mvc-action-method
            ValueProviderFactories.Factories.Add(new XmlValueProviderFactory());

            ModelBinders.Binders.DefaultBinder = new MvcRQ.Helpers.RQCustomModelBinders();
            //ModelBinders.Binders.Add(typeof(MvcRQ.Models.RQItem), new MvcRQ.Helpers.RQCustomModelBinders());
        }

        protected void Application_AuthenticateRequest()
        {
            if (MvcRQUser.UserManagementController.IsTargeted())
            {
                MvcRQUser.UserManagementController.IsRequestAuthorized = System.Web.Security.Roles.GetRolesForUser().Contains("admin");
            }
        }
    }
}