using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mvc5RQ.Areas.DigitalObjects.Models;

namespace Mvc5RQ.Areas.DigitalObjects.Models
{
    /// <summary>
    /// Controller Class for URLs designating ItemViewer objects
    /// </summary>
    public class ViewerController : Mvc5RQ.Controllers.BaseController
    {
        bool _bRedirectToRemote = false;

        /// <summary>
        /// Controller action answering GET http-requests for RQItems
        /// </summary>
        /// <returns>
        /// Standard view
        /// </returns>
        public ActionResult Index(string rqitemId, string digitalObjectAdress)
        {
            ViewBag.DocNo = rqitemId;
            if (digitalObjectAdress.StartsWith("MyDocs"))
            {
                ViewerModel theModel;

                if (    (!_bRedirectToRemote)
                     && (System.Web.HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST").ToLower() == "localhost") || (System.Web.HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST").ToLower() == "admin-pc"))
                    theModel = new ViewerModel(rqitemId, "http://" + System.Web.HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST") + "/" + digitalObjectAdress);
                else
                {
                    digitalObjectAdress = Helpers.AccessControl.AppendAccessRightsCode(digitalObjectAdress);
                    theModel = new ViewerModel(rqitemId, "http://mydocs.strands.de/" + digitalObjectAdress);
                }
                ViewBag.DocAdr = (theModel.Count() > 0) ? theModel.digitalObjectAdress : "UNDEFINED";
                return View(theModel);
            }
            else if (digitalObjectAdress.StartsWith("MyMusic"))
            {
                AudioModel theModel;

                if ((System.Web.HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST").ToLower() == "localhost") || (System.Web.HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST").ToLower() == "admin-pc"))
                    theModel = new AudioModel(rqitemId, "http://" + System.Web.HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST") + "/" + digitalObjectAdress);
                else
                {
                    //Access to remote document server & access control not yet implemented
                    //digitalObjectAdress = Helpers.AccessControl.AppendAccessRightsCode(digitalObjectAdress);
                    theModel = new AudioModel(rqitemId, "http://mydocs.strands.de/" + digitalObjectAdress);
                }
                ViewBag.DocAdr = theModel.Count() == 1 ? theModel.PlayList.Tracks[0].mp3 : (theModel.Count() > 0 ? "PLAYLIST" : "UNDEFINED");
                return View("AudioPlayer", theModel);
            }
            else
            {
                string fullAdress = "http://" + System.Web.HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST") + "/" + digitalObjectAdress + (digitalObjectAdress.EndsWith(".m4v") ? "" : "/" + digitalObjectAdress.Substring(digitalObjectAdress.LastIndexOf("/") + 1) + ".m4v");
                VideoModel theModel = new VideoModel(rqitemId, fullAdress);

                ViewBag.DocAdr = (theModel.Count() > 0) ? theModel.digitalObjectAdress : "UNDEFINED";
                return View("VideoPlayer", theModel);
            }
        }
    }
}
