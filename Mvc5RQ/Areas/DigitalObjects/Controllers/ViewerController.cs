using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using Mvc5RQ.Areas.DigitalObjects.Models;

namespace Mvc5RQ.Areas.DigitalObjects.Controllers
{
    /// <summary>
    /// Riquest Digital Object Service
    /// TODO: ItemViewer-Funktionalitäten in diesen Controller übertragen.
    /// </summary>
    [RoutePrefix("rqdos")]
    public class RQDOSController : ApiController
    {
        [Route("{type}/{name}")]
        [HttpGet]
        public TocViewModel Get(string type, string name) 
        {
            return new Mvc5RQ.Areas.DigitalObjects.Models.TocViewModel(name, type);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Index()
        //{
        //    return View();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        //public ActionResult TableOfContent (string id, string verb)
        //{
        //    return View(new Mvc5RQ.Areas.DigitalObjects.Models.TocViewModel(id, verb));
        //}
    }
}
