using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace Mvc5RQ.Areas.UserManagement
{
  class UserManagementAuthorization : AuthorizeAttribute
  {

    protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
    {
        return Controllers.UserManagementController.IsRequestAuthorized;
    }

  }
}
