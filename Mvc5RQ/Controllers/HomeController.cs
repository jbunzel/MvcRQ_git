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
    public class HomeController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            ViewData["qryStr"] = "Suchbegriff eingeben";
            return this.RedirectToRoute("RQItemList", new { dbname = "rqitems" });
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult About()
        //{
        //    ViewBag.Title = "RiQuest | " + RQResources.Views.Shared.SharedStrings.menu2; ;
        //    ViewBag.HTML = "<p>Über RiQuest</p>";
        //    return View("StaticText", new Mvc5RQ.Models.StaticTextModel("faq", "~/xslt/statictexttransforms/faq.xsl"));

        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult News()
        {
            ViewBag.Title = "RiQuest | " + RQResources.Views.Shared.SharedStrings.menu5;
            ViewBag.HTML = "<p>Nachrichten</p>";
            return View();
        }
    }
}
