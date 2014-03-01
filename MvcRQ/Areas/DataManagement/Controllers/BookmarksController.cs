using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RQLib.RQDAL;

namespace MvcRQ.Areas.DataManagement.Controllers
{
    public class BookmarksController : MvcRQ.Controllers.BaseController
    {
        //
        // GET: /DataManagement/Bookmarks/

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
                    string path = "D:\\Users\\Public\\MyVLib\\My Virtual Subject Library\\";
                    string name = "xml/dir.xml";
                    RQBookmarkDAL VLD = new RQBookmarkDAL();
                    System.IO.DirectoryInfo VLDir = new System.IO.DirectoryInfo(path);

                    if (VLDir.Exists)
                    {
                        VLD.LoadBookmarks(ref path, ref name);
                    }
                    else
                        throw new AccessViolationException("Bookmark directory does not exist!");
                 }
                catch (Exception ex)
                {
                    throw new AccessViolationException(ex.Message);
                }
                return Json("Bookmark directory has been successfully indexed!");
            }
            else
                throw new AccessViolationException("Not authorized for this function!");
        }

    }
}
