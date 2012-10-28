using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MvcRQ.Helpers
{
    public class CacheManager
    {    
        #region ICacheManager Members     
        
        static public void Add(string key, object value)    
        {        
            HttpRuntime.Cache.Add(key, value, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);    
        }     
        
        static public bool Contains(string key)    
        {        
            return HttpRuntime.Cache.Get(key) != null;    
        }     
        
        static public int Count()    
        {        
            return HttpRuntime.Cache.Count;    
        }     
        
        static public void Insert(string key, object value)    
        {        
            HttpRuntime.Cache.Insert(key, value);    
        }     
        
        static public T Get<T>(string key)    
        {        
            return (T)HttpRuntime.Cache.Get(key);    
        }     
        
        static public void Remove(string key)    
        {        
            HttpRuntime.Cache.Remove(key);    
        }

        static public void Clear()
        {
            foreach (System.Collections.DictionaryEntry de in HttpRuntime.Cache)
            {
                Remove((string)de.Key);
            }
        }
       
        #endregion
    }
}