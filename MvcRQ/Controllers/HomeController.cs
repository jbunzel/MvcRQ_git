using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcRQ.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            ViewData["qryStr"] = "Suchbegriff eingeben";
            return this.RedirectToRoute("RQItemList", new { dbname = "rqitems" });
        }

        public ActionResult About()
        {
            ViewBag.Title = "RiQuest | " + RQResources.Views.Shared.SharedStrings.menu2; ;
            ViewBag.HTML = "<p>Über RiQuest</p>";
            return View("StaticText");
        }

        public ActionResult News()
        {
            ViewBag.Title = "RiQuest | " + RQResources.Views.Shared.SharedStrings.menu5;
            ViewBag.HTML = "<p>Nachrichten</p>";
            return View("StaticText");
        }

        public ActionResult Work()
        {
            ViewBag.Title = "Riquest | " + RQResources.Views.Shared.SharedStrings.menu4;
            ViewBag.HTML = "<p>Arbeitsplatz</p>";
            return View("StaticText");
        }

        public ActionResult Terms()
        {
            ViewBag.Title = "Riquest | " + RQResources.Views.Shared.SharedStrings.link_terms;
            ViewBag.HTML = "<p>Nutzungsbedingungen</p>";
            return View("StaticText");
        }

        public ActionResult Privacy()
        {
            ViewBag.Title = "Riquest | " + RQResources.Views.Shared.SharedStrings.link_privacy;
            ViewBag.HTML = "<p>Datenschutz</p>";
            return View("StaticText");
        }
    }
}
