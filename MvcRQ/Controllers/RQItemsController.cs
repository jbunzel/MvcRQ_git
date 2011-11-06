using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcRQ.Models;
using MvpRestApiLib;
using System.Web.UI;

using RQLib.RQQueryForm;

namespace MvcRQ.Controllers
{
    public class RQItemsController : Controller
    {
        // Controller supports the following URL
        // /RQItems/{RQItemId}/ItemDescElements/{itemdescelementId}

        private RQItemModel GetModel( string queryString)
        {
            RQquery query = RQLib.RQStateManager.RQSessionState.RQQueryState.GetQueryState(queryString);
            var rqitemModel = base.HttpContext.Cache[query.QueryString.ToLower()] as RQItemModel;

            if (rqitemModel == null)
            {
                if (query.QueryBookmarks == false)
                    query.QueryBookmarks = true;
                rqitemModel = new RQItemModel(query);
                base.HttpContext.Cache[query.QueryString.ToLower()] = rqitemModel;
            }
            return rqitemModel;
        }

        // GET /RQItems
        // Return all RQItems.
        [EnableJson, EnableXml]
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Index(string verb)
        {
            if (verb == "New")
                return View("NewRQItem", new RQItem());
            else
                return this.Content(GetModel("").toHTML(verb), "text/html", System.Text.Encoding.UTF8);
        }

        // POST /RQItems
        // Query RQItems.
        [EnableJson, EnableXml]
        [HttpPost, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        [ActionName("Index")]
        public ActionResult SearchRQItem(string queryString)
        {
            return View(GetModel(queryString).RQItems);
        }
        
        //// POST /RQItems
        //// Add a new RQItem.
        //[EnableJson, EnableXml]
        //[HttpPost, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        //[ActionName("Index")]
        //public ActionResult AddNewRQItem(RQItem newRQItem)
        //{
        //    List<RQItem> rqitems = new List<RQItem>(GetModel().RQItems);
        //    newRQItem.ID = "CUS" + (rqitems.Count + 1).ToString("0000");
        //    rqitems.Add(newRQItem);
        //    GetModel().RQItems = rqitems;

        //    return RedirectToAction("SingleRQItem", new { rqitemId = newRQItem.ID });
        //}

        //// GET /RQItems/00001
        //// Return a single rqitem data.
        //[EnableJson, EnableXml(XSLTransform = "/xslt/RQ2DC.xslt")]
        //[HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        //public ActionResult SingleRQItem(string rqitemId)
        //{
        //    RQItem rqitem = GetModel().RQItems.FirstOrDefault(p => p.DocNo == rqitemId); 

        //    return View("SingleRQItem", rqitem);
        //}

        // GET /RQItems/00001
        // Return a single rqitem data.
        [EnableJson, EnableXml]
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult SingleRQItem(string rqitemId, string formatId)
        {
            RQItem rqitem = GetModel("").RQItems.FirstOrDefault(p => p.DocNo == rqitemId);
            if (formatId != null)
            {
                System.Xml.XmlTextReader r = rqitem.ConvertTo(formatId);

                r.MoveToContent();
                return this.Content(r.ReadOuterXml(), "text/xml", System.Text.Encoding.UTF8);
            }
            else
                return View("SingleRQItem", rqitem);
        }

        //// DELETE /RQItems/CUS0001
        //// Delete a single rqitem.
        //[EnableJson, EnableXml]
        //[HttpDelete]
        //[ActionName("SingleRQItem")]
        //public ActionResult SingleRQItemDelete(string rqitemId)
        //{
        //    List<RQItem> rqitems = new List<RQItem>(GetModel().RQItems);
        //    rqitems.Remove(rqitems.Find(c => c.ID == rqitemId));
        //    GetModel().RQItems = rqitems;
        //    return RedirectToAction("Index", "RQItems");
        //}

        //// POST /RQItems/CUS0001(?verb=Delete)
        //// Update/Delete a single rqitem
        //[HttpPost]
        //[EnableJson, EnableXml]
        //public ActionResult SingleRQItem(RQItem changeRQItem, string rqitemId, string verb)
        //{
        //    if (verb == "Delete")
        //    {
        //        return SingleRQItemDelete(rqitemId);
        //    }
        //    else
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var existingRQItem = GetModel().RQItems.First(c => c.ID == rqitemId);
        //            existingRQItem.Title = changeRQItem.Title;
        //            existingRQItem.Location = changeRQItem.Locality;

        //            ViewData["Message"] = "Saved";
        //            return SingleRQItem(rqitemId);
        //        }
        //        else
        //        {
        //            throw new ApplicationException("Invalid model state");
        //        }
        //    }            
        //}

        //// GET /RQItems/CUS0001/ItemDescElements(/ORD0001)
        //// Return rqitem itemdescelements. If itemdescelementId specified, then return a single itemdescelement.
        //[EnableJson, EnableXml]        
        //[HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]        
        //public ActionResult SingleRQItemItemDescElements(string rqitemId, string itemdescelementId)
        //{
        //    if (!string.IsNullOrEmpty(itemdescelementId))
        //        return View("SingleRQItemSingleItemDescElement", GetModel()
        //            .RQItems.First(c => c.ID == rqitemId)
        //            .ItemDescElements.First(o => o.ID == itemdescelementId));
        //    else
        //        return View("SingleRQItemItemDescElements", GetModel()
        //            .RQItems.First(c => c.ID == rqitemId)
        //            .ItemDescElements);

        //}

        //// POST /RQItems/CUS0001/ItemDescElements/ORD0001(?verb=Delete)
        //// Update/Delete a single itemdescelement.
        //[HttpPost]
        //[EnableJson, EnableXml]
        //public ActionResult SingleRQItemItemDescElements(ItemDescElement changedItemDescElement, string rqitemId, string itemdescelementId, string verb)
        //{
        //    if (verb == "Delete")
        //    {
        //        var rqitem = GetModel().RQItems.First(c => c.ID == rqitemId);
        //        List<ItemDescElement> itemdescelements = new List<ItemDescElement>(rqitem.ItemDescElements);
        //        itemdescelements.Remove(itemdescelements.Find(o => o.ID == itemdescelementId));
        //        rqitem.ItemDescElements = itemdescelements;
        //        return RedirectToAction("SingleRQItem", "RQItems", new { rqitemId = rqitemId });
        //    }            
        //    else 
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var existingItemDescElement = GetModel().RQItems.First(c => c.ID == rqitemId)
        //                .ItemDescElements.First(o => o.ID == itemdescelementId);
        //            existingItemDescElement.Name = changedItemDescElement.Name;
        //            existingItemDescElement.Value1 = changedItemDescElement.Value1;
        //            existingItemDescElement.Value1 = changedItemDescElement.Value1;

        //            ViewData["Message"] = "Saved";
        //            return SingleRQItemItemDescElements(rqitemId, itemdescelementId);
        //        }
        //        else
        //        {
        //            throw new ApplicationException("Invalid model state");
        //        }
        //    }            
        //}
        
    }

}
