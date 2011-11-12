using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcRQ.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            ViewData["qryStr"] = "Suchbegriff eingeben";

            return this.RedirectToAction("Index","RQItems");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
