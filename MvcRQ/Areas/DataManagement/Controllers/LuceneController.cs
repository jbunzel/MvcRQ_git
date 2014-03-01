using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RQLib.RQDAL;

namespace MvcRQ.Areas.DataManagement.Controllers
{
    public class LuceneController : MvcRQ.Controllers.BaseController
    {
        //
        // GET: /DataManagement/Lucene/

        public ActionResult Index()
        {
            ViewBag.HasAdminPermit = MvcRQ.Helpers.AccessRightsResolver.HasAdminAccess() ? "true" : "false";
            return View();
        }

        [HttpPost]
        public ActionResult New()
        {
            if (MvcRQ.Helpers.AccessRightsResolver.HasAdminAccess())
            {
                ViewBag.HasAdminPermit = "true";
                try
                {
                    new RQLuceneDBI().Reindex();
                }
                catch (Exception ex)
                {
                    throw new AccessViolationException(ex.Message);
                }
                return Json("The Lucene index has been successfully recreated and optimized!");
            }
            else
                throw new AccessViolationException("Not authorized for this function!");
        }

        [HttpPost]
        public ActionResult Optimize()
        {
            if (MvcRQ.Helpers.AccessRightsResolver.HasAdminAccess())
            {
                ViewBag.HasAdminPermit = "true";
                try
                {
                    new RQLuceneDBI().Optimize();
                }
                catch (Exception ex)
                {
                    throw new AccessViolationException(ex.Message);
                }
                return Json("The Lucene index has been successfully optimized!"); 
            }
            else
                throw new AccessViolationException("Not authorized for this function!");
        }

    }
}
