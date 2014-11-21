using System;
//using System.Web;
//using System.Collections.Generic;
using System.Linq;
//using System.Net;
using System.Net.Http;
using System.Web.Http;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin.Security;
using Mvc5RQ.Models;
using Mvc5RQ.Helpers;
using Mvc5RQ.Areas.UserSettings;
//using System.Threading;
//using System.Threading.Tasks;
//using System.IO;
//using System.Net.Http.Headers;

namespace Mvc5RQ.Controllers
{
    /// <summary>
    /// Riquest Date Service (RQDS)
    /// </summary>
    [RoutePrefix("rqds")]
    public class RQDSController : ApiController
    {
        #region public contructors

        public RQDSController()
        {
        }

        public RQDSController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion

        #region public actions

        /// <summary>
        /// Controller action answering GET web-api-requests for rqitems.
        /// </summary>
        /// <remarks>
        /// The action reacts to URLs of type "~/rqds/{dbname}/{format}".
        /// </remarks>
        /// <param name="dbname">
        /// "rqitems" | {user-dbname} 
        ///     Name of the requested database as indicated by the requested url.
        ///     Database "rqitems" contains all rqitems of all users, which may be accessed by current user.
        ///     Database {db-username} contains all rqitems of the user named {username}, which may be accessed by current user. 
        ///     
        ///     NOTE: The feature is currently not implemented. If implemented, the parameter has to be included in other actions, too.
        /// </param>
        /// <param name="format">
        /// null | rqi | rq | oai_dc | ...
        /// </param>
        /// <param name="verb">
        /// null | querylist | browselist.
        /// </param>
        /// <param name="queryString">
        /// query string to select the requested rqitems.
        /// </param>
        /// <returns>
        /// The list of requested rqitems according to requested data format ({format}) and accepted exchange format ({application/json} | {application/xml}).
        /// </returns>
        [Route("{dbname}/{format}")]
        [HttpGet]
        public RQItemModel Get(string dbname, string format, string verb = "", string queryString = "")
        {
            try 
            {
                RQItemModelRepository repo = new RQItemModelRepository(new FormatParameter((FormatParameter.FormatEnum)Enum.Parse(typeof(FormatParameter.FormatEnum), format)));
                
                return repo.GetModel(queryString, Areas.UserSettings.UserState.StateType(verb));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Controller action answering GET web-api-requests for single rqitem.
        /// </summary>
        /// <remarks>
        /// The action reacts to URLs of type "~/rqds/{dbname}/{id}/{format}".
        /// </remarks>
        /// <param name="dbname">
        /// "rqitems" | {user-dbname} 
        ///     Name of the requested database as indicated by the requested url.
        ///     Database "rqitems" contains all rqitems of all users, which may be accessed by current user.
        ///     Database {db-username} contains all rqitems of the user named {username}, which may be accessed by current user. 
        ///     
        ///     NOTE: The feature is currently not implemented. If implemented, the parameter has to be included in other actions, too.
        /// </param>
        /// <param name="id">
        /// Identification number of the requested rqitem.
        /// </param>
        /// <param name="format">
        /// null | rqi | rq | oai_dc | ...
        /// </param>
        /// <param name="verb">
        /// null | querylist | browselist | edititem
        /// </param>
        /// <param name="queryString"></param>
        /// <returns>
        /// The requested rqitem according to requested data format ({format}) and accepted exchange format ({application/json} | {application/xml}).
        /// </returns>
        [Route("{dbname}/{id}/{format}")]
        [HttpGet]
        public RQItem Get(string dbname, string id, string format, string verb = "", string queryString = "")
        {
            UserState.States state;
            bool forEdit;

            RQItemModelRepository repo = new RQItemModelRepository(new FormatParameter((FormatParameter.FormatEnum)Enum.Parse(typeof(FormatParameter.FormatEnum), format + ((format == "rqi") ? "_single_item" : ""))));
            if (!string.IsNullOrEmpty(verb))
                switch (verb.ToLower())
                {
                    case "queryitem":
                        state = UserState.States.ListViewState;
                        forEdit = false;
                        break;
                    case "browseitem":
                        state = UserState.States.BrowseViewState;
                        forEdit = false;
                        break;
                    case "edititem":
                        state = RQItem.IsExternal(id) ? UserState.States.ListViewState : UserState.States.EditState;
                        forEdit = RQItem.IsExternal(id) ? false : true;
                        break;
                    default:
                        state = UserState.States.ListViewState;
                        forEdit = false;
                        break;
                }
            else
            {
                state = UserState.States.ListViewState;
                forEdit = false;
            }
            return repo.GetRQItem(id, state, forEdit);
        }

        /// <summary>
        /// Controller action answering GET web-api-requests for subfields of a single rqitem.
        /// </summary>
        /// <remarks>
        /// The action reacts to URLs of type "~/rqds/{dbname}/{id}/{subfield}/{index:int}/{format?}".
        /// </remarks>
        /// <param name="dbname">
        /// "rqitems" | {user-dbname} 
        ///     Name of the requested database as indicated by the requested url.
        ///     Database "rqitems" contains all rqitems of all users, which may be accessed by current user.
        ///     Database {db-username} contains all rqitems of the user named {username}, which may be accessed by current user. 
        ///     
        ///     NOTE: The feature is currently not implemented. If implemented, the parameter has to be included in other actions, too.
        /// </param>
        /// <param name="id">
        /// Identification number of the requested rqitem.
        /// </param>
        /// <param name="subfield">
        /// Name of the requested subfield
        /// </param>
        /// <param name="index">
        /// Index of requested subfield item if subfield is an array. (Default = 0). 
        /// </param>
        /// <param name="format">
        /// null | rqi | rq | oai_dc | ...
        /// </param>
        /// <param name="verb">
        /// null | querylist | browselist | edititem
        /// </param>
        /// <param name="queryString"></param>
        /// <returns>
        /// The requested subfield of the requested rqitem according to requested data format ({format}) and accepted exchange format ({application/json} | {application/xml}).
        /// </returns>
        [Route("{dbname}/{id}/{subfield}/{index:int}/{format?}")]
        [HttpGet]
        public RQItem Get(string dbname, string id, string subfield, int index, string format = "", string verb = "")
        {
            throw new NotImplementedException("");
        }

        /// <summary>
        /// Controller action answering PUT web-api-requests to add a single rqitem to database.
        /// </summary>
        /// <remarks>
        /// The action reacts to URLs of type "~/rqds/{dbname}".
        /// </remarks>
        /// <param name="dbname">
        /// "rqitems" | {user-dbname} 
        ///     Name of the requested database as indicated by the requested url.
        ///     Database "rqitems" contains all rqitems of all users, which may be accessed by current user.
        ///     Database {db-username} contains all rqitems of the user named {username}, which may be accessed by current user. 
        ///     
        ///     NOTE: The feature is currently not implemented. If implemented, the parameter has to be included in other actions, too.
        /// </param>
        /// <param name="newRQItem">
        /// Rqitem to add to database.
        /// </param>
        /// <returns>
        /// On success the rqitem added to the database according to requested data format ({format}) and accepted exchange format ({application/json} | {application/xml}).
        /// On error an return message according to the accepted exchange format ({application/json} | {application/xml}).
        /// </returns>
        [Route("{dbname}")]
        [HttpPost]
        [RQAuthorize(Roles="admin, patron")]
        public RQItem Post(string dbname, [FromBody]RQItem newRQItem)
        {
            if (ModelState.IsValid)
            {
                RQItem rqitem = null;

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
                return Get("rqitems",rqitem.DocNo, "rqi", "edititem", "");
            }
            else
                throw new Exception("Fehler beim Erstellen.");
        }

        /// <summary>
        /// Controller action answering PUT web-api-requests to change a single rqitem in the database.
        /// </summary>
        /// <remarks>
        /// The action reacts to URLs of type "~/rqds/{dbname}/{id}".
        /// </remarks>
        /// <param name="dbname">
        /// "rqitems" | {user-dbname} 
        ///     Name of the requested database as indicated by the requested url.
        ///     Database "rqitems" contains all rqitems of all users, which may be accessed by current user.
        ///     Database {db-username} contains all rqitems of the user named {username}, which may be accessed by current user. 
        ///     
        ///     NOTE: The feature is currently not implemented. If implemented, the parameter has to be included in other actions, too.
        /// </param>
        /// <param name="id">
        /// Identification number of the rqitem to change.
        /// </param>
        /// <param name="changeRQItem">
        /// Rqitem to change in the database.
        /// </param>
        /// <returns>
        /// On success the rqitem changed in the database according to requested data format ({format}) and accepted exchange format ({application/json} | {application/xml}).
        /// On error an return message according to the accepted exchange format ({application/json} | {application/xml}).
        /// </returns>
        [Route("{dbname}/{id}")]
        [HttpPost]
        [RQAuthorize(Roles = "admin, patron")]
        public RQItem Post(string dbname, string id, [FromBody]RQItem changeRQItem)
        {
            if (ModelState.IsValid)
            {
                RQItemModelRepository repo = new RQItemModelRepository((new FormatParameter(FormatParameter.FormatEnum.rqi)));
                RQItemModel model = null;
                RQItem rqitem = null;
                try
                {
                    model = repo.GetModel("$access$" + id, UserState.States.EditState, true);
                    rqitem = model.RQItems.FirstOrDefault(p => p.DocNo == id);
                    rqitem.Change(changeRQItem);
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
                return Get("rqitems",id, "rqi", "edititem", "");
            }
            else
                throw new Exception("Fehler beim Update.");
        }

        /// <summary>
        /// Controller action answering PUT web-api-requests to delete a single rqitem from the database.
        /// </summary>
        /// <remarks>
        /// The action reacts to URLs of type "~/rqds/{dbname}/delete/{id}".
        /// </remarks>
        /// <param name="dbname">
        /// "rqitems" | {user-dbname} 
        ///     Name of the requested database as indicated by the requested url.
        ///     Database "rqitems" contains all rqitems of all users, which may be accessed by current user.
        ///     Database {db-username} contains all rqitems of the user named {username}, which may be accessed by current user. 
        ///     
        ///     NOTE: The feature is currently not implemented. If implemented, the parameter has to be included in other actions, too.
        /// </param>
        /// <param name="id">
        /// Identification number of the rqitem to delete.
        /// </param>
        /// <returns>
        /// On success the rqitem added to the database according to requested data format ({format}) and accepted exchange format ({application/json} | {application/xml}).
        /// On error an return message according to the accepted exchange format ({application/json} | {application/xml}).
        /// </returns>
        [Route("{dbname}/delete/{id}")]
        [RQAuthorize(Roles = "admin, patron")]
        public void Delete(string dbname, string id)
        {
            throw new NotImplementedException("Die Funktion zum Löschen eines Elements aus der Datenbank ist noch nicht verfügbar.");
        }
                
        /// <summary>
        /// Controller action answering GET web-api-requests for rqitems.
        /// </summary>
        /// <remarks>
        /// The action reacts to URLs of type "~/rqds/{dbname}/{format}".
        /// </remarks>
        /// <param name="dbname">
        /// "rqitems" | {user-dbname} 
        ///     Name of the requested database as indicated by the requested url.
        ///     Database "rqitems" contains all rqitems of all users, which may be accessed by current user.
        ///     Database {db-username} contains all rqitems of the user named {username}, which may be accessed by current user. 
        ///     
        ///     NOTE: The feature is currently not implemented. If implemented, the parameter has to be included in other actions, too.
        /// </param>
        /// <param name="format">
        /// null | rqi | rq | oai_dc | ...
        /// </param>
        /// <param name="verb">
        /// null | querylist | browselist.
        /// </param>
        /// <param name="queryString">
        /// query string to select the requested rqitems.
        /// </param>
        /// <returns>
        /// The list of requested rqitems according to requested data format ({format}) and accepted exchange format ({application/json} | {application/xml}).
        /// </returns>
        [Route("rqkos/{id}")]
        [HttpGet]
        public RQKosBranch Get(string id, string verb = "")
        {
            return new RQKosModel(id, verb).RQKosSet;
        }

        #endregion
    }
}