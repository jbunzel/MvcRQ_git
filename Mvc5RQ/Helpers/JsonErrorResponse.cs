using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Mvc5RQ.Helpers
{
    public static class JsonErrorResponse
    {
        public static HttpResponseMessage Create(Mvc5RQ.Models.RQKosBranch.RQKosBranchStatus status) 
        { 
            Mvc5RQ.Areas.MyJsonResult result = Mvc5RQ.Areas.MyJsonResult.CreateError(status);
            string json = new JavaScriptSerializer().Serialize(result);
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(json),
            };
        }

        public static HttpResponseMessage Redirect(string uri)
        {
            var ret = new HttpResponseMessage(System.Net.HttpStatusCode.Redirect);

            ret.Headers.Location = new Uri(uri);
            return ret;
        }
    }
}