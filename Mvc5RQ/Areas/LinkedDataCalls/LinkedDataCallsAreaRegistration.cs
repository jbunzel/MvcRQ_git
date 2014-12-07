using System.Web.Mvc;

namespace Mvc5RQ.Areas.LinkedDataCalls
{
    public class LinkedDataCallsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "LinkedDataCalls";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "LinkedDataCalls_default",
                "LinkedDataCalls/{action}/{id}",
                new { area = "LinkedDataCalls", controller = "LinkedDataCalls", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
