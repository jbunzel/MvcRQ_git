﻿using System;
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
                "ServiceRQItemList",
                "{formatId}/RQItems",
                new { controller = "RQItems", action = "RQItemList", formatId = UrlParameter.Optional });

            routes.MapRoute(
                "RQItemList",
                "RQItems",
                new { controller = "RQItems", action = "Index" });

            routes.MapRoute(
                "ServiceSingleRQItem",
                "{formatId}/RQItems/{rqitemId}",
                new { controller = "RQItems", action = "RQItemRecord", formatId = UrlParameter.Optional});

            routes.MapRoute(
                "SingleRQItem",
                "RQItems/{rqitemId}",
                new { controller = "RQItems", action = "RQItemRecord" });

            routes.MapRoute(
                "RQItemItemDescElements",
                "RQItems/{rqitemId}/ItemDescElements/{itemdescelementId}",
                new { controller = "RQItems", action = "SingleRQItemItemDescElements", itemdescelementId = UrlParameter.Optional });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            // Source: http://haacked.com/archive/2010/04/15/sending-json-to-an-asp-net-mvc-action-method-argument.aspx
            // This must be added to accept JSON as request
            ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
            // This must be added to accept XML as request
            // Source: http://www.nogginbox.co.uk/blog/xml-to-asp.net-mvc-action-method
            ValueProviderFactories.Factories.Add(new XmlValueProviderFactory());
        }
    }
}