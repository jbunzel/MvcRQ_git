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
            return View();
        }
    }
}
