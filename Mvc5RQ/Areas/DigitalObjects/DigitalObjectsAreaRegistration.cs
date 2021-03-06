﻿using System.Web.Mvc;

namespace Mvc5RQ.Areas.DigitalObjects
{
    public class DigitalObjectsAreaRegistration : AreaRegistration
    {
        /// <summary>
        /// 
        /// </summary>
        public override string AreaName
        {
            get
            {
                return "DigitalObjects";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DigitalObjects_default",
                "DigitalObjects/{rqitemId}",
                new { area = "DigitalObjects", controller = "Viewer", action = "Index", rqitemId = UrlParameter.Optional }
            );
            
            //context.MapRoute(
            //    "DigitalObjects_default",
            //    "DigitalObjects/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
