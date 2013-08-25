using System.Web.Mvc;

namespace MvcRQ.Areas.ItemViewer
{
    public class ItemViewerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ItemViewer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ItemViewer_default",
                "ItemViewer/{rqitemId}",
                new { area="ItemViewer", controller="ItemViewer",  action = "Index", rqitemId = UrlParameter.Optional }
            );
        }
    }
}
