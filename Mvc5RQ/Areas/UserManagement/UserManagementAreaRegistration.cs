using System.Web.Mvc;

namespace Mvc5RQ.Areas.UserManagement
{
    public class UserManagementAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "UserManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "UserManagement_default",
                "UserManagement/{action}/{id}",
                new { area = "UserManagement", controller="UserManagement", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "RoleManagement_default",
                "RolesAdmin/{action}/{id}",
                new { area = "UserManagement", controller = "RolesAdmin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}