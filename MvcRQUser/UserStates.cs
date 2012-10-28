using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RQState;
using RQState.Components;
using RQState.Components.Storage;

namespace MvcRQUser
{
    /// <summary>
    /// View State Storage
    /// </summary>
    public class v : StateObject<v, ConfigurableStorage <v>> 
    {
        //public string qs { get; set; } // QueryString

        public object q { get; set; }  // used to store query

        public v() : base() { }

        public v(Object Id)
            : base()
        {
            this.ID = Id;
        }
    }

    /// <summary>
    /// Guest Id State Storage
    /// </summary>
    public class u: StateObject<u, CookieStorage<u>>
    {
        public Guid ui { get; set; } // Guest ID
        
        public u() : base() { }

        public u(Object Id)
            : base()
        {
            this.ID = Id;
        }
    }

    public class GuestIdState : UserState
    {
        private u _stateStorage = null;

        public Guid GuestId
        {
            get
            {
                return this._stateStorage.ui;
            }
            set
            {
                this._stateStorage.ui = value;
                this.Save();
            }
        }

        public GuestIdState(Guid GuestId)
        {
            this._stateStorage = new u(UserState.StateTypeKey(UserState.States.GuestIdState));
            this.GuestId = GuestId;
            //this.Save();
        }

        public override void Save()
        {
            this._stateStorage.Save();
        }
    }

    public class ViewState : UserState
    {
        private v _stateStorage = null;

        //public string queryString
        //{
        //    get
        //    {
        //        return this._stateStorage.q.s;
        //    }
        //    set
        //    {
        //        this._stateStorage.qs = value;
        //        this.Save();
        //    }
        //}

        public object query
        {
            get
            {
                return this._stateStorage.q;
            }
            set
            {
                this._stateStorage.q = value;
                this.Save();
            }
        }

        public ViewState(UserState.States stateType) //string queryString)
        {
            this._stateStorage = new v(UserState.StateTypeKey(stateType));
            //this.queryString = queryString;
            //this.Save();
        }

        public override void Save()
        {
            this._stateStorage.Save();
        }
    }

    public class UserState
    {
        public enum States
        {
            ListViewState,
            ItemViewState,
            EditState,
            BrowseViewState,
            GuestIdState
        }

        public static string StateTypeKey(States stateType)
        {
            switch (stateType)
            {
                case States.ListViewState:
                    return "l";
                case States.ItemViewState:
                    return "i";
                case States.BrowseViewState:
                    return "b";
                case States.GuestIdState:
                    return "u";
                default:
                    return "";
            }
        }

        public UserState()
        {
        }

        public virtual void Save()
        { }

        public static UserState Get(UserState.States stateType)
        {
            if (stateType == States.GuestIdState)
            {
                return u.Get(UserState.StateTypeKey(stateType)) != null ? new GuestIdState(u.Get(UserState.StateTypeKey(stateType)).ui) : null;
            }
            else
            {
                v st = v.Get(UserState.StateTypeKey(stateType));

                if (st != null)
                {
                    ViewState vs = new ViewState(stateType); //, st.qs);
                    vs.query = st.q;
                    return vs;
                }
                else
                    return null;
            }
        }
    }
}
