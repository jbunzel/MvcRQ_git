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
                "{serviceId}/{dbname}",
                new { controller = "RQItems", action = "RQItemList" },
                new { dbname = new MvcRQ.Helpers.IsDBName() }
                );
            routes.MapRoute(
                "RQItemList",
                "{dbname}",
                new { controller = "RQItems", action = "RQItemList" }, 
                new { dbname = new MvcRQ.Helpers.IsDBName() }
                );
            routes.MapRoute(
                "ServiceRQItemRecord",
                "{serviceId}/{dbname}/{rqitemId}",
                new { controller = "RQItems", action = "RQItemRecord" },
                new { dbname = new MvcRQ.Helpers.IsDBName() }
                );
            routes.MapRoute(
                "RQItemRecord",
                "{dbname}/{rqitemId}",
                new { controller = "RQItems", action = "RQItemRecord" },
                new { dbname = new MvcRQ.Helpers.IsDBName() }
                );
            routes.MapRoute(
                "RQItemSubField",
                "{dbname}/{rqitemId}/{fieldName}/{subFieldIndex}",
                new { controller = "RQItems", action = "RQItemDescElement" },
                new { dbname = new MvcRQ.Helpers.IsDBName() }
                );
            routes.MapRoute(
                "RQItemField",
                "{dbname}/{rqitemId}/{fieldName}",
                new { controller = "RQItems", action = "RQItemDescElement" },
                new { dbname = new MvcRQ.Helpers.IsDBName() }
                );
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            //System.Data.Entity.Database.SetInitializer(new MvcRQ.Areas.UserSettings.Models.SettingsDBInitializer());

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
            if (MvcRQ.Areas.UserManagement.Controllers.UserManagementController.IsTargeted())
            {
                MvcRQ.Areas.UserManagement.Controllers.UserManagementController.IsRequestAuthorized = System.Web.Security.Roles.GetRolesForUser().Contains("Administrators");
            }
        }
    }
}