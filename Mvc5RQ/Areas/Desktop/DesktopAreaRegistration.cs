using System.Web.Mvc;

namespace Mvc5RQ.Areas.Desktop
{
    public class DesktopAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Desktop";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Desktop_default",
                "Desktop/{action}/{id}",
                new { area = "Desktop", controller = "Desktop", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}