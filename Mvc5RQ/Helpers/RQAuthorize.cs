using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net;
using System.Net.Http;
//using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Mvc5RQ.Models;

namespace Mvc5RQ.Helpers
{
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    //public class RQAuthorizeAttribute : AuthorizationFilterAttribute
    //{
    //    public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
    //    {
    //        base.OnAuthorization(actionContext);
    //    }

    //    protected override bool AuthorizeCore(HttpContextBase httpContext)
    //    {
    //        if (!httpContext.Request.IsAuthenticated)
    //            return false;
    //        if (!AccessRightsResolver.HasEditAccess()) // implement this method based on your tables and logic
    //        {
    //            return false;
    //            //base.HandleUnauthorizedRequest(filterContext);
    //        }
    //        return true;
    //        // base.OnAuthorization(filterContext);
    //    }

    //    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    //    {
    //        if (filterContext.HttpContext.Request.IsAjaxRequest())
    //        {
    //            var viewResult = new JsonResult();

    //            viewResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
    //            viewResult.Data = (new { IsSuccess = "Unauthorized", description = "Sorry, you do not have the required permission to perform this action." });
    //            filterContext.Result = viewResult;
    //        }
    //        else
    //        {
    //            var viewResult = new ViewResult();

    //            viewResult.ViewName = "~/Views/Errors/_Unauthorized.cshtml";
    //            filterContext.Result = viewResult;
    //        }
    //        //base.HandleUnauthorizedRequest(filterContext);
    //    }
    //}

    public class RQAuthorizeAttribute : AuthorizeAttribute
    {
        //public override void OnAuthorization(HttpActionContext filterContext)
        //{
        //    //base.OnAuthorization(filterContext);
        //    if (this.IsAuthorized(filterContext))
        //        this.HandleUnauthorizedRequest(filterContext);

        //}

        //protected override bool IsAuthorized(HttpActionContext actionContext)
        //{
        //    return HttpContext.Current.User.Identity.IsAuthenticated;
        //}


        //protected override bool AuthorizeCore(HttpActionContext httpContext)
        //{
        //    //if (!httpContext.Request.IsAuthenticated)
        //    //    return false;
        //    if (!AccessRightsResolver.HasEditAccess()) // implement this method based on your tables and logic
        //    {
        //        return false;
        //        //base.HandleUnauthorizedRequest(filterContext);
        //    }
        //    return true;
        //    // base.OnAuthorization(filterContext);
        //}

        protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
        {
            string message = "Sie sind nicht autorisiert diese Funktion auszuführen.";

            Mvc5RQ.Areas.MyJsonResult UnauthorizedResult = Mvc5RQ.Areas.MyJsonResult.CreateError(message);
            filterContext.Response = filterContext.Request.CreateResponse<Mvc5RQ.Areas.MyJsonResult>(HttpStatusCode.BadRequest, UnauthorizedResult);
        }
    }

    public static class IdentityHelpers
    {
        public static void UpdateLastAccessDate(string userId)
        {
            ApplicationUserManager um = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser au = um.FindByIdAsync(userId).Result;

            au.LastActivityDate = DateTime.Now;
            um.Update(au);
        }
    }
}