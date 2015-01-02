using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Mvc5RQ.Controllers;
using Mvc5RQ.Areas.StaticTextPages.Models;

namespace Mvc5RQ.Areas.StaticTextPages.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AboutController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "RiQuest | " + RQResources.Views.Shared.SharedStrings.menu2; ;
            ViewBag.HTML = "<p>Über RiQuest</p>";
            return View("StaticText", new StaticTextModel("faq", "~/xslt/statictexttransforms/faq.xsl"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Terms()
        {
            ViewBag.Title = "Riquest | " + RQResources.Views.Shared.SharedStrings.link_terms;
            ViewBag.HTML = "<p>Nutzungsbedingungen</p>";
            return View("StaticText", new StaticTextModel("terms", "~/xslt/statictexttransforms/jbarticle.xsl"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Privacy()
        {
            ViewBag.Title = "Riquest | " + RQResources.Views.Shared.SharedStrings.link_privacy;
            ViewBag.HTML = "<p>Datenschutz</p>";
            return View("StaticText", new StaticTextModel("privacy", "~/xslt/statictexttransforms/jbarticle.xsl"));
        }
    }
}
