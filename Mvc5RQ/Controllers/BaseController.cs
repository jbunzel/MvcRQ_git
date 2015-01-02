using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc5RQ.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseController : Controller
    {
        #region protected methods
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.IsCustomErrorEnabled || true)  //IsCustomErrorEnabled always false if client is localhost or client and server IPs identical. True set to override.
            {
                filterContext.ExceptionHandled = true;
                // If this is an ajax request, return the exception in the response            
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 500;
                    var json = new JsonResult();
                    json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;   //SICHERHEITSPROBLEM: s. http://haacked.com/archive/2009/06/25/json-hijacking.aspx
                    json.Data = HttpUtility.UrlEncode(filterContext.Exception.Message); // +filterContext.Exception.StackTrace;
                    json.ExecuteResult(this.ControllerContext);
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
