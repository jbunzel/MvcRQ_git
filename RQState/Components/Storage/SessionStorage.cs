using System.Collections;
using System.Web;
using RQState.Components.Storage;

namespace RQState.Components.Storage
{
    public class SessionStorage<T> : HashStorage<T> where T : class
    {
        protected override Hashtable TypeStorage
        {
            get
            {
                string typeKey = typeof (T).FullName;
                return HttpContext.Current.Session[typeKey] as Hashtable;
            }
            set
            {
                string typeKey = typeof (T).FullName;
                HttpContext.Current.Session[typeKey] = value;
            }
        }
    }
}