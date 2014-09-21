using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;

using MvpRestApiLib;
using RQDigitalObjects;
using RQDigitalObjects.AudioObjects.MP3;

namespace MvcRQ.Areas.DigitalObjects.Controllers
{
    /// <summary>
    /// TODO: ItemViewer-Funktionalitäten in diesen Controller übertragen.
    /// </summary>
    public class ViewerController : MvcRQ.Controllers.BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [EnableJson, EnableXml]
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult TableOfContent (string id, string verb)
        {
            return View(new MvcRQ.Areas.DigitalObjects.Models.TocViewModel(id, verb));
        }
    }
}
