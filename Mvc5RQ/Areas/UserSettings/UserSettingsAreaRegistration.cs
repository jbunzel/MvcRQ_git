using System.Web.Mvc;

namespace Mvc5RQ.Areas.UserSettings
{
    public class UserSettingsAreaRegistration : AreaRegistration
    {
        /// <summary>
        /// 
        /// </summary>
        public override string AreaName
        {
            get
            {
                return "UserSettings";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
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
