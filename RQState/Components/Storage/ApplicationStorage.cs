using System.Collections;
using System.Web;

namespace RQState.Components.Storage
{
    public class ApplicationStorage<T> : HashStorage<T> where T : class
    {
        protected override Hashtable TypeStorage
        {
            get
            {
                string typeKey = typeof (T).FullName;
                return HttpContext.Current.Application[typeKey] as Hashtable;
            }
            set
            {
                string typeKey = typeof (T).FullName;
                HttpContext.Current.Application[typeKey] = value;
            }
        }
    }
}