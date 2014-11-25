using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Mvc5RQ.Models;
using Mvc5RQ.Helpers;

namespace Mvc5RQ.Controllers
{
    //[Authorize]
    [RoutePrefix("rqld")]
    public class RQLDController : ApiController
    {
        private string GetClientIp(HttpRequestMessage request)
        {
            //if (request.Properties.ContainsKey("MS_HttpContext"))
            //{
            //    return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            //}
            //else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            //{
            //    RemoteEndpointMessageProperty prop;
            //    prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
            //    return prop.Address;
            //}
            //else
            //{
            //    return null;
            //}

            return ((System.Web.HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
        }
        
        /// <summary>
        /// Controller action answering GET linked data requests for rqkos.
        /// </summary>
        /// <remarks>
        /// The action reacts to URLs of type "~/rqld/rqkos/{id}".
        /// </remarks>
        /// <param name="id">
        /// </param>
        /// <param name="verb">
        /// </param>
        /// <returns>
        /// </returns>
        [Route("rqkos/{id}")]
        [HttpGet]
        public RQKosBranch Get(string id)
        {
            if (System.Web.HttpContext.Current.Request.Headers.Get("Accept").ToLower().Contains("text/html"))
                throw new HttpResponseException(JsonErrorResponse.Redirect(Request.RequestUri.ToString().Replace("rqld/rqkos", "rqkos/RQKosLD")));
            else
                return new RQKosModel(id, "rqld").RQKosSet;

        }

        //// GET api/values
        //[Route("{dbname}")]
        //[HttpGet]
        //public IEnumerable<string> Get(string dbname)
        //{
        //    //if (System.Web.HttpContext.Current.Request.Headers.Get("Accept").ToLower().Contains("text/html"))
        //    //    System.Web.HttpContext.Current.Response.Redirect("http://www.riquest.de/rqld/rqkos");
        //    return new string[] { "rqld1", "rqld2" };
        //}        // POST api/values
        
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/values/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE api/values/5
        //public void Delete(int id)
        //{
        //}
    }
}
