using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MvcRQUser.UserManagement
{
    class UserManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "MvcRQUser.UserManagement"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("UserManagementRoute",
                             "UserManagement/{action}",
                             new { controller = "UserManagement", action = "GetAllUsers" });
        }
    }
}
