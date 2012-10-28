using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RQLib.RQQueryForm;

using MvcRQ.Areas.UserSettings;

namespace MvcRQ.Helpers
{
    /// <summary>
    /// Manages query state parameters
    /// </summary>
    public class StateStorage
    {
        static ViewState state = null;

        /// <summary>
        /// Builds the actual query. 
        /// </summary>
        /// <param name="queryString">
        /// queryString == null | ""        : Rebuilds actual query from stored query state parameters or returns the default query.
        /// queryString != null             : Builds a new query from queryString and stores it in query state parameters. 
        /// </param>
        /// <param name="stateType">
        /// stateType == ListViewState      : (Re)builds actual query for ListView. 
        /// stateType == BrowseViewState    : (Re)builds actual query for BrowseView.
        /// stateType == ItemViewState      : (Re)builds actual query for SingleItemView.
        /// </param>
        /// <returns>
        /// Actual query.
        /// </returns>
        public static RQquery GetQueryFromState(string queryString, UserState.States stateType)
        {
            RQquery query = null;

            state = (ViewState)UserState.Get(stateType);
            if (state == null)
            {
                if (string.IsNullOrEmpty(queryString))
                    query = new RQquery("Recent Additions", "recent");
                else
                    query = new RQquery(queryString);
                state = new ViewState(stateType); //, queryString);
                state.query = query;
                //state.Save();
            }
            else
            {
                string querytest;
                UserSettingsService us = new UserSettingsService();

                query = (RQquery)state.query;
                if (! string.IsNullOrEmpty(queryString) && queryString.StartsWith("$") && queryString.LastIndexOf("$") > 1)
                    querytest = queryString.Substring(0, queryString.LastIndexOf("$")+1) + query.QueryString;
                else
                    querytest = query.QueryString;
                if (   (! string.IsNullOrEmpty(queryString) && querytest != queryString) 
                    || (query.QueryExternal == "" && us.GetIncludeExternal() == true) 
                    || (query.QueryExternal != "" && us.GetIncludeExternal() == false) )
                {
                    query = new RQquery(! string.IsNullOrEmpty(queryString) ? queryString : query.QueryString);
                    query.QueryExternal = us.GetIncludeExternal() == true ? "003" : "";
                    //((RQquery)state.query).QueryString = queryString != "" ? queryString : query.QueryString;
                    state.query = query;
                    //state.Save();
                }
                //if (string.IsNullOrEmpty(queryString))
                //    query = new RQquery((state.queryString == "" ? "Recent Additions" : state.queryString), (state.queryString == "" ? "recent" : (stateType == UserState.States.BrowseViewState ? "browse" : "form")));
                //else
                //{
                //    query = new RQquery(queryString);
                //    state.queryString = queryString;
                //    state.Save();
                //}
            }
            //query.QueryExternal = new MvcRQUser.UserSettings.UserSettingsService().GetIncludeExternal() == true ? "003" : "";
            return query;
        }
    }
}