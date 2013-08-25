using System;
using System.Collections.Generic;
using System.Web;

namespace RQState.Components.Storage
{
    public class CookieStorage<T> : IStorage<T> where T : class
    {
        private HttpServerUtility Utility
        {
            get { return HttpContext.Current.Server; }
        }

        private static HttpCookie CookieIn
        {
            get { return HttpContext.Current.Request.Cookies[typeof (T).FullName]; }
        }

        private static HttpCookie CookieOut
        {
            get
            {
                HttpContext.Current.Response.Cookies[typeof (T).FullName].Expires = DateTime.Now.AddYears(1);
                return HttpContext.Current.Response.Cookies[typeof (T).FullName];
            }
        }

        public void Clear()
        {
            CookieOut.Values.Clear();
        }

        public void Delete(object key)
        {
            foreach (string currentKey in CookieIn.Values.Keys)
            {
                if (key.ToString() != currentKey)
                   CookieOut[currentKey] = CookieIn[currentKey];
            }
            CookieIn.Values.Clear();
            foreach (string currentKey in CookieOut.Values.Keys)
            {
                   CookieIn[currentKey] = CookieOut[currentKey];
            }
        }

        public T Get(object key)
        {
            if (CookieIn == null)
                return null;
            string decodedXml = CookieIn[key.ToString()];
            string xml = Utility.UrlDecode(decodedXml);
            return Serializer<T>.Deserialize(xml);
        }

        public List<T> GetAll(Type type)
        {
            List<T> list = new List<T>();
            if (CookieIn == null)
                return list;

            foreach (string key in CookieIn.Values.Keys)
            {
                string decodedXml = CookieIn.Values[key];
                if (string.IsNullOrEmpty(decodedXml))
                    continue;
                string xml = Utility.UrlDecode(decodedXml);
                T obj = Serializer<T>.Deserialize(xml);
                list.Add(obj);
            }
            return list;
        }

        public void Save(object key, T obj)
        {
            string xml = Serializer<T>.Serialize(obj);
            if (CookieIn != null)
                foreach (string currentKey in CookieIn.Values.Keys)
                {
                    CookieOut[currentKey] = CookieIn[currentKey];
                }
            CookieOut[key.ToString()] = Utility.UrlEncode(xml);
        }
    }
}