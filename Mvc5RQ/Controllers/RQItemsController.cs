﻿using System;
using System.Linq;
using System.Web.UI;
using System.Web.Mvc;
using Mvc5RQ.Models;
using Mvc5RQ.Areas.UserSettings;
using Mvc5RQ.Helpers;

namespace Mvc5RQ.Controllers
{
    /// <summary>
    /// Controller Class for URLs designating RQItem objects
    /// </summary>
    /// <remarks>
    /// The controller supports URLs of type 
    /// {serviceId}RQItems/{RQItemId}
    /// </remarks>
    public class RQItemsController : BaseController
    {
        #region private members

        bool bClientEditing = true;
        RQItemModelRepository modelRepository = new RQItemModelRepository();

        #endregion

        #region public actions

        /// <summary>
        /// Controller action answering GET http-requests for RQItems
        /// </summary>
        /// <remarks>
        /// The action reacts to URLs of type "~/RQItems".
        /// </remarks>
        /// <param name="queryString">
        /// </param>
        /// <param name="dbname">
        /// "rqitem" | {username} = name of the requested database as indicated by the requested url.
        ///     Database "rqitem" contains all rqitems of all users, which may be accessed by current user.
        ///     Database {username} contains all rqitems of the user named {username}, which may be accessed by current user. 
        ///     
        ///     NOTE: The feature is currently not implemented. If implemented, the parameter has to be included in other actions, too.  
        /// </param>
        /// <returns>
        /// The standard RQItem Vierwer (Index View)
        /// Actual document number (docNo), add item permission of actual user and actual user state are set for evaluation by result-viewer.js script.
        /// </returns>
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RQItemList(string queryString, string dbname)
        {
            if (!string.IsNullOrEmpty(queryString) && (queryString.StartsWith("Signature: R*"))) this.modelRepository.sortParameter = new SortParameter(SortParameter.SortOrderEnum.ByShelf);
            ViewBag.docNo = HttpContext.Request.QueryString.Get("d") != null ? HttpContext.Request.QueryString.Get("d") : modelRepository.GetQuery(queryString).DocId;
            ViewBag.HasAddPermit = User.IsInRole("admin"); // Enable the add new button if user is allowed to add RQItems to the database.
            ViewBag.GetRQItemVerb = "QueryItem"; // Tell GetRQItem() in ResultViewer the appropiate verb for saving the user state.
            return View("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbname"></param>
        /// <returns></returns>
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Add(string dbname)
        {
            if (Helpers.AccessRightsResolver.HasAddAccess())
            {
                ViewBag.Title = "RiQuest | Add";
                ViewBag.Header = "Neues Dokument";
                if (bClientEditing)
                {
                    return View("ClientEditor");
                }
                else
                {
                    ViewBag.EditButton1 = RQResources.Views.Shared.SharedStrings.add;
                    ViewBag.EditButton2 = RQResources.Views.Shared.SharedStrings.cancel;
                    return View("EditRQItem", new RQItem());
                }
            }
            else
                throw new AccessViolationException("Not authorized for this function!");
        }

        /// <summary>
        /// Controller action answering POST http-requests to RQItems.
        /// </summary>
        /// <param name="queryString">
        /// Accepted values are character strings conforming to the RiQuest query language. 
        /// </param>
        /// <param name="newRQItem">
        /// </param>
        /// <param name="verb">
        /// </param>
        /// <returns>
        /// </returns>
        //[HttpPost, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        //[ValidateInput(false)]
        //public ActionResult RQItemList(string verb, string queryString, RQItem newRQItem)
        //{
        //    if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "new") || (verb == RQResources.Views.Shared.SharedStrings.add)))
        //    {
        //        if (Helpers.AccessRightsResolver.HasAddAccess())
        //        {
        //            RQItem rqitem = null;
        //            if (ModelState.IsValid)
        //            {
        //                try
        //                {
        //                    RQItemModel model = new RQItemModel(true);
        //                    rqitem = model.Add(newRQItem);
        //                    model.Update();
        //                }
        //                catch (Exception ex)
        //                {
        //                    string message = "Add operation failed. ";
        //                    Exception iex = ex;
        //                    while (iex != null)
        //                    {
        //                        if (!string.IsNullOrEmpty(iex.Message))
        //                            message += "\n - " + iex.Message;
        //                        iex = iex.InnerException;
        //                    }
        //                    throw new Exception(message);
        //                }
        //                CacheManager.Clear();
        //                if (bClientEditing)
        //                {
        //                }
        //                else
        //                {
        //                    ViewBag.EditButton2 = RQResources.Views.Shared.SharedStrings.finish;
        //                    ViewBag.EditButton3 = RQResources.Views.Shared.SharedStrings.copy;
        //                    ViewBag.EditButton3Link = "/RQItems/" + rqitem.DocNo + "?verb=copy";
        //                    ViewBag.EditButton4 = RQResources.Views.Shared.SharedStrings.add;
        //                    ViewBag.EditButton4Link = "/RQItems?verb=new";
        //                }
        //                return RQItemRecord((verb == "") ? verb : "edititem", rqitem.DocNo);
        //            }
        //            else
        //            {
        //                if (bClientEditing)
        //                {
        //                    ViewBag.RQItemId = "";
        //                    return View("ClientEditor");
        //                }
        //                else
        //                {
        //                    ViewBag.EditButton1 = RQResources.Views.Shared.SharedStrings.add;
        //                    ViewBag.EditButton2 = RQResources.Views.Shared.SharedStrings.cancel;
        //                    return View("EditRQItem", newRQItem);
        //                }
        //            }
        //        }
        //        else
        //            throw new AccessViolationException("Not authorized for this function!");
        //    }
        //    else if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "cancel") || (verb == RQResources.Views.Shared.SharedStrings.cancel)))
        //        return this.RedirectToRoute("RQItemList", new { dbname = "rqitems" });
        //    else
        //    {
        //        ViewBag.HasAddPermit = Helpers.AccessRightsResolver.HasAddAccess(); // Enable the add new button if user is allowed to add RQItems ti the database.
        //        ViewBag.GetRQItemVerb = "QueryItem"; // Tell GetRQItem() in ResultViewer the appropiate verb for saving the user state.
        //        return View("Index");
        //    }
        //}

        /// <summary>
        /// Controller action answering GET http-requests to {serviceId}/RQItems/{rqitemID}.
        /// </summary>
        /// <remarks>
        /// The action reacts to URLs of type "~/{serviceId}/RQItems/{rqitemID}".
        /// The action request is filtered through the ActionFilterAttributes EnableJson and EnableXml:
        /// If the client GET request contains dataType=application/json | text/json or dataType=application/xml | text/xml the action response is 
        /// generated by class JsonResult2 or XmlResult of MvpRestApiLib. In both cases the client is responsible to render the xml (f. e. by XSLT).
        /// Otherwise the action response is tranformed on the server according to the serviceId specified. 
        /// </remarks>
        /// <param name="verb">
        /// verb == QueryItem: requests a single RQItem of a query result list.
        /// verb == BrowseItem: requests a single RQItem of a browse result list.
        /// verb == Edit: allows editing of a database record.
        /// verb == Copy: allows copying of a database record.
        /// </param>
        /// <param name="serviceId">
        /// Id of desired sercive API:
        /// "rqi" = internal RiQuest API (default);
        /// "rq" = RiQuest proprietary exchange format;
        /// "mods" = Metadata Object Description Format (MODS) API of the Library of Congress (not yet supported);
        /// "oai_dc" = Dublin Core(DC) API serving metadata according to the specification of Open Archives Initiative (OAI);
        /// "srw_dc" = Dublin Core(DC) API serving metadata according to the specification of the Search / Retrieve Web Service (SRW) (not yet supported);
        /// "info_ofi" = OpenURL API (not yet supported);
        /// "pubmed" = API serving metadata according to the PubMed metadata scheme (not yet supported).
        /// </param>
        /// <param name="rqitemId">
        /// DocNo of RQItem to be returned.
        /// </param>
        /// <returns>
        /// The action returns the single RQItem with DocNo=rqitemID as */json, */xml response or text/html response - the latter formatted according to 
        /// SingleRQItemView - depending on data types specified in the GET request.  
        /// </returns>
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RQItemRecord(string verb, string rqitemId)
        {
            RQItem rqitem = null;
            string view = "";

            //if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "edit")))
            //{
            //    if (Helpers.AccessRightsResolver.HasEditAccess())
            //    {
            //        ViewBag.Title = "RiQuest | Edit";
            //        ViewBag.Header = "Dokument editieren";
            //        if (bClientEditing)
            //        {
            //            ViewBag.RQItemId = rqitemId;
            //            return View("ClientEditor");
            //        }
            //        else
            //        {
            //            ViewBag.EditButton1 = RQResources.Views.Shared.SharedStrings.update;
            //            ViewBag.EditButton2 = RQResources.Views.Shared.SharedStrings.cancel;
            //            view = "EditRQItem";
            //            rqitem = modelRepository.GetRQItem(rqitemId, UserState.States.EditState, true);
            //        }
            //    }
            //    else
            //        throw new AccessViolationException("Not authorized for this function!");
            //}
            //else if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "copy")))
            //{
            //    if (Helpers.AccessRightsResolver.HasAddAccess())
            //    {
            //        ViewBag.Title = "RiQuest | Copy";
            //        ViewBag.Header = "Dokument kopieren";
            //        if (bClientEditing)
            //        {
            //            ViewBag.RQItemId = rqitemId;
            //            return View("ClientEditor");
            //        }
            //        else
            //        {
            //            ViewBag.EditButton1 = RQResources.Views.Shared.SharedStrings.add;
            //            ViewBag.EditButton2 = RQResources.Views.Shared.SharedStrings.cancel;
            //            rqitem = new RQItem(modelRepository.GetRQItem(rqitemId, (RQItem.IsExternal(rqitemId) ? UserState.States.ListViewState : UserState.States.EditState), true)._resultItem);
            //            rqitem.DocNo = "";
            //            rqitem.ID = "";
            //            view = "EditRQItem";
            //        }
            //    }
            //    else
            //        throw new AccessViolationException("Not authorized for this function!");
            //}
            //else if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "edititem")))
            if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "edititem")))
            {
                view = "EditRQItem";
                rqitem = modelRepository.GetRQItem(rqitemId, (RQItem.IsExternal(rqitemId) ? UserState.States.ListViewState : UserState.States.EditState), RQItem.IsExternal(rqitemId) ? false : true);
            }
            else
            {
                rqitem = modelRepository.GetRQItem(rqitemId, UserState.States.ItemViewState, false);
                modelRepository.GetQuery(rqitemId);
                ViewBag.docNo = HttpContext.Request.QueryString.Get("d") != null ? HttpContext.Request.QueryString.Get("d") : "";
                ViewBag.HasAddPermit = User.IsInRole("admin"); //Helpers.AccessRightsResolver.HasAddAccess(); // Enable the add new button if user is allowed to add RQItems to the database.
                ViewBag.GetRQItemVerb = "QueryItem"; // Tell GetRQItem() in ResultViewer the appropiate verb for saving the user state.
                return View("Index");
            }
            if (rqitem == null)
                throw new NotImplementedException("Could not find a RiQuest item with requested document number.");
            else
            {
                bool bClassificationLinkedData = false;  //all semantic web servers for classification data not working properly

                if (bClassificationLinkedData == true)
                    rqitem.LoadLinkedData("Classification");
                return View(view, rqitem);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbname"></param>
        /// <returns></returns>
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Copy(string dbname, string rqitemId)
        {
            if (Helpers.AccessRightsResolver.HasAddAccess())
            {
                ViewBag.Title = "RiQuest | Copy";
                ViewBag.Header = "Dokument kopieren";
                ViewBag.RQItemId = rqitemId;
                ViewBag.EditType = "copy";
                return View("ClientEditor");
            }
            else
                throw new AccessViolationException("Not authorized for this function!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbname"></param>
        /// <returns></returns>
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Edit(string dbname, string rqitemId)
        {
            if (Helpers.AccessRightsResolver.HasEditAccess())
            {
                ViewBag.Title = "RiQuest | Edit";
                ViewBag.Header = "Dokument editieren";
                ViewBag.RQItemId = rqitemId;
                ViewBag.EditType = "edit";
                return View("ClientEditor");
            }
            else
                throw new AccessViolationException("Not authorized for this function!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbname"></param>
        /// <returns></returns>
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        [Authorize(Roles="admin, patron")]
        public ActionResult Delete(string dbname, string rqitemId)
        {
            throw new NotImplementedException("Die Funktion zum Löschen eines Elements aus der Datenbank ist noch nicht verfügbar.");
        }

        /// <summary>
        /// POST /RQItems. Add a new RQItem.
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="rqitemId"></param>
        /// <param name="changeRQItem"></param>
        /// <returns></returns>
        //[HttpPost, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        //[ValidateInput(false)]
        //[ActionName("RQItemRecord")]
        //public ActionResult UpdateItem(string verb, string rqitemId, RQItem changeRQItem)
        //{
        //    if (!ModelState.IsValid)
        //        return View();
        //    if ((rqitemId != null) && (rqitemId != ""))
        //    {
        //        if ((verb.ToLower() == "cancel") || (verb == RQResources.Views.Shared.SharedStrings.cancel))
        //            return this.RedirectToRoute("RQItemList", new { dbname = "rqitems" });
        //        else if ((verb.ToLower() == "finalize") || (verb == RQResources.Views.Shared.SharedStrings.finish))
        //        {
        //            return this.RedirectToRoute("RQItemList", new { dbname = "rqitems" });
        //        }
        //        else
        //        {
        //            if (Helpers.AccessRightsResolver.HasAddAccess())
        //            {
        //                RQItemModel model = null;
        //                RQItem rqitem = null;
        //                try
        //                {
        //                    model = modelRepository.GetModel("$access$" + rqitemId, UserState.States.EditState, true);
        //                    rqitem = model.RQItems.FirstOrDefault(p => p.DocNo == rqitemId);
        //                    if ((verb.ToLower() == "update") || (verb == RQResources.Views.Shared.SharedStrings.update))
        //                        rqitem.Change(changeRQItem);
        //                    else if ((verb.ToLower() == "new") || (verb == RQResources.Views.Shared.SharedStrings.add))
        //                        rqitem = model.Add(changeRQItem);
        //                    else if ((verb.ToLower() == "delete") || (verb == RQResources.Views.Shared.SharedStrings.delete))
        //                    { } // not yet implemented
        //                    model.Update();
        //                }
        //                catch (Exception ex)
        //                {
        //                    string message = "Update operation failed. ";
        //                    Exception iex = ex;

        //                    while (iex != null)
        //                    {
        //                        if (!string.IsNullOrEmpty(iex.Message))
        //                            message += "\n - " + iex.Message;
        //                        iex = iex.InnerException;
        //                    }
        //                    throw new Exception(message);
        //                };
        //                CacheManager.Clear();
        //                ViewBag.EditButton2 = RQResources.Views.Shared.SharedStrings.finish;
        //                if (AccessRightsResolver.HasAddAccess())
        //                {
        //                    ViewBag.EditButton3 = RQResources.Views.Shared.SharedStrings.copy;
        //                    ViewBag.EditButton3Link = "/RQItems/" + rqitem.DocNo + "?verb=copy";
        //                    ViewBag.EditButton4 = RQResources.Views.Shared.SharedStrings.add;
        //                    ViewBag.EditButton4Link = "/RQItems?verb=new";
        //                }
        //                return RQItemRecord((verb == "") ? verb : "edititem", rqitem.DocNo);
        //            }
        //            else
        //                throw new System.AccessViolationException(RQResources.Views.Shared.SharedStrings.err_not_authorized);
        //        }
        //    }
        //    throw new NotImplementedException("No item for update specified.");
        //}

        /// <summary>
        /// Controller action answering GET http-requests to /RQItems/{rqitemID}/{fieldName}.
        /// </summary>
        /// <remarks>
        /// The action reacts to URLs of type "~/RQItems/{rqitemID}/{fieldName}".
        /// The action request is filtered through the ActionFilterAttributes EnableJson and EnableXml:
        /// If the client GET request contains dataType=application/json | text/json or dataType=application/xml | text/xml the action response is generated by class JsonResult2 or XmlResult of MvpRestApiLib.
        /// In both cases the client is responsible to render the xml (f. e. by XSLT).
        /// Otherwise the action response is tranformed on the server according to the XXX-View. 
        /// </remarks>
        /// <param name="rqitemId">
        /// DocNo of RQItem to be used.
        /// </param>
        /// <param name="fieldName">
        /// Name of ItemAplphanumeric string with name of ItemDescElement: f. e. Classification
        /// </param>
        /// <param name="subFieldIndex">
        /// Index of the required subfield of the ItemDescElement.
        /// </param>
        /// <returns>
        /// The action returns the ItemDescElement with FieldName = descElementId of RQItem with DocNo=rqitemId as */json, */xml response or text/html response - the latter formatted according to DescElementView - depending on data types specified in the GET request.  
        /// </returns>
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]        
        public ActionResult RQItemDescElement(string rqitemId, string fieldName, string subFieldIndex)
        {
            if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(rqitemId) )
            {
                try
                {
                    RQItem rqitem = modelRepository.GetModel("$access$" + rqitemId, UserState.States.ItemViewState).RQItems.FirstOrDefault(p => p.DocNo == rqitemId);

                    return View("ServIndRQItem", rqitem.GetLinkedData(fieldName, System.Convert.ToInt16(subFieldIndex)));
                }
                catch (Exception)
                {
                    throw;
                };
            }
            return null;
        }

        #endregion
    }
}