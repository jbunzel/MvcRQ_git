using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RQLib.RQQueryForm;

namespace MvcRQ.Helpers
{
    public class s : RQState.Components.StateObject<s, RQState.Components.Storage.ConfigurableStorage<s>>
    {
        public string qs { get; set; } // QueryString

        public s() : base() { }

        public s(Object Id)
            : base()
        {
            this.ID = Id;
        }
    }

    public class StateStorage
    {
        static s state = null;

        public enum States
        {
            ListViewState,
            ItemViewState,
            EditState,
            BrowseViewState
        }

        private static string StateTypeKey(States stateType)
        {
            switch (stateType)
            {
                case States.ListViewState:
                    return "l";
                case States.ItemViewState:
                    return "i";
                case States.BrowseViewState:
                    return "b";
                default:
                    return "";
            }
        }

        public static RQquery GetQueryFromState(string queryString, States stateType)
        {
            RQquery query = null;

            state = RQState.Components.StateObject<s, RQState.Components.Storage.ConfigurableStorage<s>>.Get(StateTypeKey(stateType));
            if (state == null)
            {
                if (string.IsNullOrEmpty(queryString))
                    query = new RQquery("Recent Additions", "recent");
                else
                    query = new RQquery(queryString);
                state = new s(StateTypeKey(stateType));
                state.qs = queryString;
                state.Save();
            }
            else
            {
                if (string.IsNullOrEmpty(queryString))
                    query = new RQquery((state.qs == "" ? "Recent Additions" : state.qs), (state.qs == "" ? "recent" : state.qs));
                else
                {
                    query = new RQquery(queryString);
                    state.qs = queryString;
                    state.Save();
                }
            }
            return query;
        }
    }
}