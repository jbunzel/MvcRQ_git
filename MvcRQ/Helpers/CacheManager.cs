using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MvcRQ.Helpers
{
    public class CacheManager
    {
        #region public members
        private const string RQ_QUERY_CACHE_PREFIX = "RQRY_";
        #endregion

        #region ICacheManager Members

        static public void Add(string key, object value)    
        {        
            HttpRuntime.Cache.Add(RQ_QUERY_CACHE_PREFIX + key, value, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);    
        }     
        
        static public bool Contains(string key)    
        {
            return HttpRuntime.Cache.Get(RQ_QUERY_CACHE_PREFIX + key) != null;    
        }     
        
        static public int Count()    
        {        
            return HttpRuntime.Cache.Count;    
        }     
        
        static public void Insert(string key, object value)    
        {
            HttpRuntime.Cache.Insert(RQ_QUERY_CACHE_PREFIX + key, value);    
        }     
        
        static public T Get<T>(string key)    
        {
            return (T)HttpRuntime.Cache.Get(RQ_QUERY_CACHE_PREFIX + key);    
        }     
        
        static public void Remove(string key)    
        {
            HttpRuntime.Cache.Remove(RQ_QUERY_CACHE_PREFIX + key);    
        }

        static public void Clear()
        {
            foreach (System.Collections.DictionaryEntry de in HttpRuntime.Cache)
            {
                if (((string) de.Key).StartsWith(RQ_QUERY_CACHE_PREFIX))
                    HttpRuntime.Cache.Remove(de.Key.ToString());
            }
        }
       
        #endregion
    }
}