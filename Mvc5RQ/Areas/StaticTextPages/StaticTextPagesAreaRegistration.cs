using System.Web.Mvc;

namespace Mvc5RQ.Areas.StaticTextPages
{
    public class StaticTextPagesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "StaticTextPages";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "StaticTextPages_default",
                "About/{controller}/{action}/{id}",
                new { area = "StaticTextPages", controller="About", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}