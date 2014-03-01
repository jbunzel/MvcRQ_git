using System.Web.Mvc;

namespace MvcRQ.Areas.DataManagement
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
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
