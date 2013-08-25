using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcRQ.Areas.ItemViewer.Models;

namespace MvcRQ.Areas.ItemViewer.Controllers
{
    /// <summary>
    /// Controller Class for URLs designating ItemViewer objects
    /// </summary>
    public class ItemViewerController : Controller
    {

        /// <summary>
        /// Controller action answering GET http-requests for RQItems
        /// </summary>
        /// <returns>
        /// Standard view
        /// </returns>
        public ActionResult Index(string rqitemId, string itemAdress)
        {
            ViewBag.DocNo = rqitemId;
            if (itemAdress.StartsWith("MyDocs") )
            {
                //ItemViewerModel theModel = new ItemViewerModel(rqitemId,"http://mydocs.strands.de/" + itemAdress);
                ItemViewerModel theModel = new ItemViewerModel(rqitemId, "http://" + System.Web.HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST") + "/" + itemAdress);
                
                ViewBag.DocAdr = (theModel.Count() > 0 ) ? theModel.itemAdress : "UNDEFINED";
                return View(theModel);        
            }
            else if (itemAdress.StartsWith("MyMusic"))
            {
                AudioModel theModel = new AudioModel(rqitemId, "http://" + System.Web.HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST") + "/" + itemAdress);

                ViewBag.DocAdr = theModel.Count() == 1 ? theModel.PlayList.Tracks[0].mp3 : (theModel.Count() > 0 ? "PLAYLIST" : "UNDEFINED");
                return View("AudioPlayer", theModel);
            }
            else
            {
                string fullAdress = "http://" + System.Web.HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST") + "/" + itemAdress + (itemAdress.EndsWith(".m4v") ? "" : "/" + itemAdress.Substring(itemAdress.LastIndexOf("/") + 1) + ".m4v"); 
                VideoModel theModel = new VideoModel(rqitemId, fullAdress);

                ViewBag.DocAdr = (theModel.Count() > 0) ? theModel.itemAdress : "UNDEFINED";
                return View("VideoPlayer", theModel);
            }
        }

    }
}
