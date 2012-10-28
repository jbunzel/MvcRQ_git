using System.Web.Mvc;

namespace MvcRQ.Areas.UserSettings
{
    public class UserSettingsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "UserSettings";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "UserSettings_default",
                "UserSettings/{action}/{id}",
                new { area= "UserSettings", controller="UserSettings", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
