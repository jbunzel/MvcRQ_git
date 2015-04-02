using System.Web.Mvc;

namespace Mvc5RQ.Areas.DataManagement
{
    public class DataManagementAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DataManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DataManagement_default",
                "DataManagement/{controller}/{action}/{id}",
                new { area = "DataManagement", controller = "DataManagement", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}