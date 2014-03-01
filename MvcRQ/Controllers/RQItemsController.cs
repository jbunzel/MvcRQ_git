using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using MvcRQ.Models;
using MvpRestApiLib;

using RQLib.RQQueryForm;
using MvcRQ.Areas.UserSettings;
using MvcRQ.Helpers;

namespace MvcRQ.Controllers
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
        bool bUseHttpCache = true;
        string strSortType = "Fach";

        #endregion

        #region private methods

        private RQquery GetQuery(string queryString, UserState.States stateType)
        {
            RQquery q = StateStorage.GetQueryFromState(queryString, stateType);

            ViewData["qryStr"] = q.QueryString;
            return q;
        }
        
        private RQquery GetQuery(string queryString)
        {
            UserState.States stateType = (!string.IsNullOrEmpty(queryString) && (queryString.StartsWith("$class$") == true)) ? UserState.States.BrowseViewState : UserState.States.ListViewState;
            return GetQuery(queryString, stateType);
        }

        private RQItemModel GetModel(string queryString, UserState.States stateType, bool forEdit)
        {
            RQItemModel rqitemModel = null;
            RQquery query = this.GetQuery(queryString, stateType);

            if (!forEdit && bUseHttpCache) rqitemModel = CacheManager.Get<RQItemModel>(query.Id.ToString());
            if (forEdit) query.QueryExternal = "";
            if ((rqitemModel == null) || (rqitemModel.IsEditable() != forEdit))
            {
                if (query.QueryBookmarks == false)
                    query.QueryBookmarks = true;
                rqitemModel = new RQItemModel(query, forEdit);
                if (! forEdit && bUseHttpCache) CacheManager.Add(query.Id.ToString(), rqitemModel);
            }
            return rqitemModel;
        }

        private RQItemModel GetModel(string queryString, UserState.States stateType)
        {
            return this.GetModel(queryString, stateType, false);
        }

        private RQItemModel GetModel(string queryString)
        {
            UserState.States stateType = (!string.IsNullOrEmpty(queryString) && (queryString.StartsWith("$class$") == true)) ? UserState.States.BrowseViewState : UserState.States.ListViewState;
            return this.GetModel(queryString, stateType, false);
        }

        private RQItem GetRQItem(string rqitemId, UserState.States stateType, bool forEdit)
        {
            try // try to get item to copy from cache
            {
                RQItem res = this.GetModel("", stateType, forEdit).RQItems.FirstOrDefault(p => p.DocNo == rqitemId);

                if (res == null) throw new Exception();
                return res;
            }
            catch
            {
                try // try to get item to copy from database
                {
                    return this.GetModel("$access$" + rqitemId, stateType, forEdit).RQItems.FirstOrDefault(p => p.DocNo == rqitemId);
                }
                catch
                {
                    throw new NotImplementedException("Item with DocNo " + rqitemId + "could not be found.");
                }
            }
        }

        private string TransformItem (RQItem rqItem, RQItem.DisplFormat format)
        {
            return rqItem.ConvertToHTML(format);
        }

        private string TransformModel(RQItemModel model, string format, int fromItem, int toItem)
        {
            System.Xml.XmlTextReader r = model.RQItems.ConvertTo(format, fromItem, toItem);

            try
            {
                var xTrf = new System.Xml.Xsl.XslCompiledTransform(true);
                var xTrfArg = new System.Xml.Xsl.XsltArgumentList();
                var xSet = new System.Xml.Xsl.XsltSettings(true, true);
                var mstr = new System.Xml.XmlTextWriter(new System.IO.MemoryStream(), System.Text.Encoding.UTF8);
                var doc = new System.Xml.XmlDocument();

                //TESTDATEI(EZEUGEN)
                //System.Xml.XmlDocument Doc = new System.Xml.XmlDocument();
                //Doc.Load(r);
                //Doc.Save("D:\\Users\\Jorge\\Desktop\\MVCTest.xml");
                //ENDE TESTDATEI 
                r.MoveToContent();
                xTrf.Load(Server.MapPath("~/xslt/ViewTransforms/RQResultList2RQSorted_Paging.xslt"),xSet, new System.Xml.XmlUrlResolver());
                xTrfArg.AddParam("ApplPath", "", "http://" + Request.ServerVariables.Get("HTTP_HOST") + (Request.ApplicationPath.Equals("/") ? "" : Request.ApplicationPath));
                xTrfArg.AddParam("MyDocsPath", "", "http://" + Request.ServerVariables.Get("HTTP_HOST") + (Request.ApplicationPath.Equals("/") ? "" : Request.ApplicationPath));
                xTrfArg.AddParam("SortType", "", strSortType);
                xTrf.Transform(new System.Xml.XPath.XPathDocument(r), xTrfArg, mstr);
                mstr.BaseStream.Flush();
                mstr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                doc.Load(mstr.BaseStream);
                //TESTDATEI EZEUGEN
                //doc.Save("D:\\Users\\Jorge\\Desktop\\MVCTest.xml");
                //mstr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                //ENDE TESTDATEI
                //var rd = new System.Xml.XmlTextReader(mstr.BaseStream);
                return doc.OuterXml;
            }
            catch
            {
                // RQItemSet ist leer
                throw new NotImplementedException("Could not find a RiQuest item with requested document number.");

                //return "";
            }
        }
        
        #endregion

        #region public actions

        /// <summary>
        /// Controller action answering GET http-requests for RQItems
        /// </summary>
        /// <remarks>
        /// The action reacts to URLs of type "~/RQItems" | "~/{serviceId}/RQItems".
        /// The action response is filtered by the ActionFilterAttributes "EnableJson" and "EnableXml":
        /// If the GET request contains dataType=application/json | text/json or dataType=application/xml | text/xml 
        /// the action response is generated by class "JsonResult2" or "XmlResult" of "MvpRestApiLib".
        /// In both cases the client is responsible to render the xml (f. e. by XSLT).
        /// Otherwise the action response is transformed to text/html on the server according to the value of parameters "verb" or "serviceId" respectively. 
        /// </remarks>
        /// <param name="verb">
        /// null | New | QueryList | BrowseList.
        /// </param>
        /// <param name="queryString">
        /// </param>
        /// <param name="serviceId">
        /// Id of requested sercive API:
        /// "rqi" = internal RiQuest API (default);
        /// "rq" = RiQuest proprietary exchange format;
        /// "mods" = Metadata Object Description Format (MODS) API of the Library of Congress (not yet supported);
        /// "oai_dc" = Dublin Core(DC) API serving metadata according to the specification of Open Archives Initiative (OAI);
        /// "srw_dc" = Dublin Core(DC) API serving metadata according to the specification of the Search / Retrieve Web Service (SRW) (not yet supported);
        /// "info_ofi" = OpenURL API (not yet supported);
        /// "pubmed" = API serving metadata according to the PubMed metadata scheme (not yet supported).
        /// </param>
        /// <param name="dbname">
        /// "rqitem" | {username} = name of the requested database as indicated by the requested url.
        ///     Database "rqitem" contains all rqitems of all users, which may be accessed by current user.
        ///     Database {username} contains all rqitems of the user named {username}, which may be accessed by current user. 
        ///     
        ///     NOTE: The feature is currently not implemented. If implemented, the parameter has to be included in other actions, too.  
        /// </param>
        /// <returns>
        /// verb == null: 
        ///     The action returns the (empty) RQItems/Index view.
        /// verb == New: 
        ///     The action returns an (empty) data entry mask to add a new RQItem.
        /// verb == QueryList | BrowseList: 
        ///     The action returns a collection of RQItems   
        ///     as */json, */xml or text/html - the latter generated by the XSLT "RQResultList2RQSorted.xslt" - 
        ///     depending on data types specified in the GET request.
        ///     If appropriate the query stored in ListView or BrowseView state storage is used. A new query is generated if the query strings 
        ///     or query options differ.
        /// serviceId = rq | rqi | mods | oai_dc | srw_dc | info_ofi | pubmed: 
        ///     The action returns a collection of RQItems formatted according to the requested service API 
        ///     as */json, */xml or text/html - the latter generated by the XSLT "RQResultList2RQSorted.xslt" - 
        ///     depending on data types specified in the GET request. 
        /// The returned collection is the result set pertaining to the query set in parameter queryString, the last query stored in the state storage or  
        ///  - if both are void - to the "recent additions" query. 
        /// </returns>
        [EnableJson, EnableXml]
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RQItemList(string verb, string queryString, string serviceId, string dbname)
        {
            if (!string.IsNullOrEmpty(queryString) && (queryString.StartsWith("Signature: R*"))) strSortType = "Regal";

            if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "new") || (verb == RQResources.Views.Shared.SharedStrings.add)))
            {
                if (MvcRQ.Helpers.AccessRightsResolver.HasAddAccess())
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
            else if ((!string.IsNullOrEmpty(verb)) && (verb.ToLower() == "querylist"))
                return this.Content(TransformModel(GetModel(queryString, UserState.States.ListViewState), verb, 1, 0), "text/html", System.Text.Encoding.UTF8);
            else if ((!string.IsNullOrEmpty(verb)) && (verb.ToLower() == "browselist"))
                return this.Content(TransformModel(GetModel(queryString, UserState.States.BrowseViewState), verb, 1, 0), "text/html", System.Text.Encoding.UTF8);
            else if (!string.IsNullOrEmpty(serviceId))
            {
                ViewBag.ServiceType = serviceId;
                ViewBag.TextSeg0 = "a list of RQItems";
                switch (serviceId)
                {
                    case "mods":
                        throw new NotImplementedException("RiQuest data service not yet implemented for mods format.");
                    case "oai_dc":
                        EnableXmlAttribute.XSLTransform = "~/xslt/rqi2dc.xslt";
                        break;
                    case "srw_dc":
                        throw new NotImplementedException("RiQuest data service not yet implemented for srw_dc format.");
                    case "info_ofi":
                        throw new NotImplementedException("RiQuest data service not yet implemented for info_ofi format.");
                    case "rqi":
                        EnableXmlAttribute.XSLTransform = "";
                        break;
                    case "rq":
                        EnableXmlAttribute.XSLTransform = "~/xslt/rqi2rq.xslt";
                        break;
                    default:
                        throw new NotImplementedException("RiQuest data service not implemented for unknown format.");
                }
                return View("ServRQItem", GetModel(queryString));
            }
            else
            {
                this.GetQuery(queryString);
                ViewBag.docNo = HttpContext.Request.QueryString.Get("d") != null ? HttpContext.Request.QueryString.Get("d") : "";
                ViewBag.HasAddPermit = MvcRQ.Helpers.AccessRightsResolver.HasAddAccess(); // Enable the add new button if user is allowed to add RQItems ti the database.
                ViewBag.GetRQItemVerb = "QueryItem"; // Tell GetRQItem() in ResultViewer the appropiate verb for saving the user state.
                return View("Index");
            }
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
        [EnableJson, EnableXml]
        [HttpPost, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        [ValidateInput(false)]
        public ActionResult RQItemList(string verb, string queryString, RQItem newRQItem)
        {
            if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "new") || (verb == RQResources.Views.Shared.SharedStrings.add)))
            {
                if (MvcRQ.Helpers.AccessRightsResolver.HasAddAccess())
                {
                    RQItem rqitem = null;

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            RQItemModel model = new RQItemModel(true);

                            rqitem = model.Add(newRQItem);
                            model.Update();
                        }
                        catch (Exception ex)
                        {
                            string message = "Add operation failed. ";
                            Exception iex = ex;

                            while (iex != null)
                            {
                                if (!string.IsNullOrEmpty(iex.Message))
                                    message += "\n - " + iex.Message;
                                iex = iex.InnerException;
                            }
                            throw new Exception(message);
                        }
                        CacheManager.Clear();
                        if (bClientEditing)
                        {
                        }
                        else
                        {
                            ViewBag.EditButton2 = RQResources.Views.Shared.SharedStrings.finish;
                            ViewBag.EditButton3 = RQResources.Views.Shared.SharedStrings.copy;
                            ViewBag.EditButton3Link = "/RQItems/" + rqitem.DocNo + "?verb=copy";
                            ViewBag.EditButton4 = RQResources.Views.Shared.SharedStrings.add;
                            ViewBag.EditButton4Link = "/RQItems?verb=new";
                        }
                        return RQItemRecord((verb == "") ? verb : "edititem", rqitem.DocNo, null);
                    }
                    else
                    {
                        if (bClientEditing)
                        {
                            ViewBag.RQItemId = "";
                            return View("ClientEditor");
                        }
                        else
                        {
                            ViewBag.EditButton1 = RQResources.Views.Shared.SharedStrings.add;
                            ViewBag.EditButton2 = RQResources.Views.Shared.SharedStrings.cancel;
                            return View("EditRQItem", newRQItem);
                        }
                    }
                }
                else
                    throw new AccessViolationException("Not authorized for this function!");
            }
            else if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "cancel") || (verb == RQResources.Views.Shared.SharedStrings.cancel)))
                return this.RedirectToRoute("RQItemList", new { dbname = "rqitems" });
            else
            {
                this.GetQuery(queryString);
                ViewBag.HasAddPermit = MvcRQ.Helpers.AccessRightsResolver.HasAddAccess(); // Enable the add new button if user is allowed to add RQItems ti the database.
                ViewBag.GetRQItemVerb = "QueryItem"; // Tell GetRQItem() in ResultViewer the appropiate verb for saving the user state.
                return View("Index");
            }
        }

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
        [EnableJson, EnableXml]
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RQItemRecord(string verb, string rqitemId, string serviceId )
        {
            RQItem rqitem = null;
            string view = "";

            if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "edit")))
            {
                if (MvcRQ.Helpers.AccessRightsResolver.HasEditAccess())
                {
                    ViewBag.Title = "RiQuest | Edit";
                    ViewBag.Header = "Dokument editieren";
                    if (bClientEditing)
                    {
                        ViewBag.RQItemId = rqitemId;
                        return View("ClientEditor");
                    }
                    else
                    {
                        ViewBag.EditButton1 = RQResources.Views.Shared.SharedStrings.update;
                        ViewBag.EditButton2 = RQResources.Views.Shared.SharedStrings.cancel;
                        view = "EditRQItem";
                        rqitem = this.GetRQItem(rqitemId, UserState.States.EditState, true);
                    }
                }
                else
                    throw new AccessViolationException("Not authorized for this function!");
            }
            else if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "copy")))
            {
                if (MvcRQ.Helpers.AccessRightsResolver.HasAddAccess())
                {
                    ViewBag.Title = "RiQuest | Copy";
                    ViewBag.Header = "Dokument kopieren";
                    if (bClientEditing)
                    {
                        ViewBag.RQItemId = rqitemId;
                        return View("ClientEditor");
                    }
                    else
                    {
                        ViewBag.EditButton1 = RQResources.Views.Shared.SharedStrings.add;
                        ViewBag.EditButton2 = RQResources.Views.Shared.SharedStrings.cancel;
                        rqitem = new RQItem(this.GetRQItem(rqitemId, (RQItem.IsExternal(rqitemId) ? UserState.States.ListViewState : UserState.States.EditState), true)._resultItem);
                        rqitem.DocNo = "";
                        rqitem.ID = "";
                        view = "EditRQItem";
                    }
                }
                else
                    throw new AccessViolationException("Not authorized for this function!");
            }
            else if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "queryitem")))
            {
                rqitem = this.GetRQItem(rqitemId, UserState.States.ListViewState, false);
                if (HttpContext.Request.AcceptTypes.Contains("text/html"))
                    return this.Content(TransformItem(rqitem, RQItem.DisplFormat.single_item), "text/html", System.Text.Encoding.UTF8);
                else
                    view = "DisplRQItem";
            }
            else if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "browseitem")))
            {
                rqitem = this.GetRQItem(rqitemId, UserState.States.BrowseViewState, false);
                if (HttpContext.Request.AcceptTypes.Contains("text/html"))
                    return this.Content(TransformItem(rqitem, RQItem.DisplFormat.single_item), "text/html", System.Text.Encoding.UTF8);
                else
                    view = "DisplRQItem";
            }
            else if ((!string.IsNullOrEmpty(verb)) && ((verb.ToLower() == "edititem")))
            {
                view = "EditRQItem";
                rqitem = this.GetRQItem(rqitemId, (RQItem.IsExternal(rqitemId) ? UserState.States.ListViewState : UserState.States.EditState), RQItem.IsExternal(rqitemId) ? false : true);
            }
            else
            {
                rqitem = this.GetRQItem(rqitemId, UserState.States.ItemViewState, false);
                if (string.IsNullOrEmpty(serviceId))
                {
                    this.GetQuery(rqitemId);
                    ViewBag.docNo = HttpContext.Request.QueryString.Get("d") != null ? HttpContext.Request.QueryString.Get("d") : "";
                    ViewBag.HasAddPermit = MvcRQ.Helpers.AccessRightsResolver.HasAddAccess(); // Enable the add new button if user is allowed to add RQItems ti the database.
                    ViewBag.GetRQItemVerb = "QueryItem"; // Tell GetRQItem() in ResultViewer the appropiate verb for saving the user state.
                    return View("Index");
                }
                else
                {
                    view = "ServRQItem";
                    ViewBag.ServiceType = serviceId;
                    ViewBag.TextSeg0 = "an individual RQItem";
                }
                switch (serviceId)
                {
                    case "mods":
                        throw new NotImplementedException("RiQuest data service not yet implemented for mods format.");
                    case "oai_dc":
                        EnableXmlAttribute.XSLTransform = "~/xslt/rqi2dc.xslt";
                        break;
                    case "srw_dc":
                        throw new NotImplementedException("RiQuest data service not yet implemented for srw_dc format.");
                    case "info_ofi":
                        throw new NotImplementedException("RiQuest data service not yet implemented for info_ofi format.");
                    case "rqi":
                        EnableXmlAttribute.XSLTransform = "";
                        break;
                    case "rq":
                        EnableXmlAttribute.XSLTransform = "~/xslt/rqi2rq.xslt";
                        break;
                    default:
                        throw new NotImplementedException("RiQuest data service not yet implemented for unknown format.");
                }
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
 
        // POST /RQItems
        // Add a new RQItem.
        [EnableJson, EnableXml]
        [HttpPost, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        [ValidateInput(false)]
        [ActionName("RQItemRecord")]
        public ActionResult UpdateItem(string verb, string rqitemId, string serviceId, RQItem changeRQItem)
        {
            if (!ModelState.IsValid)
                return View();
            if ((rqitemId != null) && (rqitemId != ""))
            {
                if ((verb.ToLower() == "cancel") || (verb == RQResources.Views.Shared.SharedStrings.cancel))
                    return this.RedirectToRoute("RQItemList", new { dbname = "rqitems" });
                else if ((verb.ToLower() == "finalize") || (verb == RQResources.Views.Shared.SharedStrings.finish))
                {
                    return this.RedirectToRoute("RQItemList", new { dbname = "rqitems" });
                }
                else
                {
                    if (MvcRQ.Helpers.AccessRightsResolver.HasAddAccess())
                    {
                        RQItemModel model = null;
                        RQItem rqitem = null;
                        try
                        {
                            model = GetModel("$access$" + rqitemId, UserState.States.EditState, true);
                            rqitem = model.RQItems.FirstOrDefault(p => p.DocNo == rqitemId);
                            if ((verb.ToLower() == "update") || (verb == RQResources.Views.Shared.SharedStrings.update))
                                rqitem.Change(changeRQItem);
                            else if ((verb.ToLower() == "new") || (verb == RQResources.Views.Shared.SharedStrings.add))
                                rqitem = model.Add(changeRQItem);
                            else if ((verb.ToLower() == "delete") || (verb == RQResources.Views.Shared.SharedStrings.delete))
                            { } // not yet implemented
                            model.Update();
                        }
                        catch (Exception ex)
                        {
                            string message = "Update operation failed. ";
                            Exception iex = ex;

                            while (iex != null)
                            {
                                if (!string.IsNullOrEmpty(iex.Message))
                                    message += "\n - " + iex.Message;
                                iex = iex.InnerException;
                            }
                            throw new Exception(message);
                        };
                        CacheManager.Clear();
                        ViewBag.EditButton2 = RQResources.Views.Shared.SharedStrings.finish;
                        if (AccessRightsResolver.HasAddAccess())
                        {
                            ViewBag.EditButton3 = RQResources.Views.Shared.SharedStrings.copy;
                            ViewBag.EditButton3Link = "/RQItems/" + rqitem.DocNo + "?verb=copy";
                            ViewBag.EditButton4 = RQResources.Views.Shared.SharedStrings.add;
                            ViewBag.EditButton4Link = "/RQItems?verb=new";
                        }
                        return RQItemRecord((verb == "") ? verb : "edititem", rqitem.DocNo, serviceId);
                    }
                    else
                        throw new System.AccessViolationException("Not authorized for this function!");
                }
            }
            throw new NotImplementedException("No item for update specified.");
        }

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
        [EnableJson, EnableXml]        
        [HttpGet, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]        
        public ActionResult RQItemDescElement(string rqitemId, string fieldName, string subFieldIndex)
        {
            if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(rqitemId) )
            {
                try
                {
                    RQItem rqitem = GetModel("$access$" + rqitemId, UserState.States.ItemViewState).RQItems.FirstOrDefault(p => p.DocNo == rqitemId);

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
