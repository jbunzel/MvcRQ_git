using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc5RQ.Areas.Desktop.Controllers
{
    public class DesktopController : Mvc5RQ.Controllers.BaseController
    {
        // GET: Desktop/Desktop
        public ActionResult Index()
        {
            //ViewBag.Title = "Riquest | " + RQResources.Views.Shared.SharedStrings.menu4;
            //ViewBag.HTML = "<p>Arbeitsplatz</p>";
            //RQDesktop.RQZkn3 zkn = new RQDesktop.RQZkn3();

            //zkn.Load(@"D:\Users\Jorge\Dokumente\Doc Projects\Contents\Mein Zettelkasten\zettelkasten.zkn3");

            return View();

        }
    }
}