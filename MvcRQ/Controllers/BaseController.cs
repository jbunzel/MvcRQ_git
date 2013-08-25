using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcRQ.Controllers
{
    public class BaseController : Controller
    {
        #region protected methods

        protected override void ExecuteCore()
        {
            string cultureName = null;
            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                try
                {
                    cultureName = Request.UserLanguages[0]; // obtain it from HTTP header AcceptLanguages
                }
                catch
                {
                }
            if (!string.IsNullOrEmpty(cultureName))
            {
                // Validate culture name
                cultureName = MvcRQ.Helpers.CultureHelper.GetImplementedCulture(cultureName); // This is safe
                // Modify current thread's cultures            
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            }

            if (Request.IsAuthenticated)
            {
                var name = User.Identity.Name;

                Guid gu = (Guid)System.Web.Security.Membership.GetUser().ProviderUserKey;
            }
            MvpRestApiLib.EnableXmlAttribute.XSLTransform = ""; // Essential, because old string values may have survived in memory. 
            base.ExecuteCore();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.IsCustomErrorEnabled || true)  //IsCustomErrorEnabled always false if client is localhost or client and server IPs identical. True set to override.
            {
                filterContext.ExceptionHandled = true;
                // If this is an ajax request, return the exception in the response            
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    //if (HttpContext.Response.ContentType == "text/html")
                    //{
                    //    filterContext.HttpContext.Response.StatusCode = 500;
                    //    var json = new JsonResult();
                    //    json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;   //SICHERHEITSPROBLEM: s. http://haacked.com/archive/2009/06/25/json-hijacking.aspx
                    //    json.Data = HttpUtility.UrlEncode(filterContext.Exception.Message); // +filterContext.Exception.StackTrace;
                    //    json.ExecuteResult(this.ControllerContext);
                    //}
                    //else
                    //{
                        filterContext.HttpContext.Response.StatusCode = 500;
                        var json = new JsonResult();
                        json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;   //SICHERHEITSPROBLEM: s. http://haacked.com/archive/2009/06/25/json-hijacking.aspx
                        json.Data = HttpUtility.UrlEncode(filterContext.Exception.Message); // +filterContext.Exception.StackTrace;
                        json.ExecuteResult(this.ControllerContext);
                    //}
                }
                else
                {
                    ViewData.Model = new System.Web.Mvc.HandleErrorInfo(filterContext.Exception, "ControllerName", "ActionName");
                    // Pass a flag to the view to tell it whether or not to show a the stack trace                
                    ViewBag.IsCustomErrorEnabled = true; //filterContext.HttpContext.IsCustomErrorEnabled;
                    this.View("Error").ExecuteResult(this.ControllerContext);
                }
            }
        }

        #endregion
    }
}
