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
    [RoutePrefix("rqld")]
    public class RQLDController : ApiController
    {
        #region private methods

        private RQKosFormat.FormatEnum GetFormat()
        {
            System.Net.Http.Formatting.IContentNegotiator negotiator = this.Configuration.Services.GetContentNegotiator();
            System.Net.Http.Formatting.ContentNegotiationResult result = negotiator.Negotiate(typeof(RQKosBranch), this.Request, this.Configuration.Formatters);
            if (result != null)
            {
                RQKosFormat.FormatEnum format = RQKosFormat.FormatEnum.undefined;
                    
                switch (result.MediaType.MediaType)
                {
                    case "application/turtle":
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
                        //format = RQKosFormat.FormatEnum.turtle;
                        //break;
                    case "text/turtle":
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
                        //format = RQKosFormat.FormatEnum.turtle;
                        //break;
                    case "application/rdf+n3":
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
                        //format = RQKosFormat.FormatEnum.turtle;
                        //break;
                    case "text/rdf+n3":
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
                        //format = RQKosFormat.FormatEnum.turtle;
                        //break;
                    case "application/rdf+xml":
                        format = RQKosFormat.FormatEnum.rdf;
                        break;
                    case "text/rdf":
                        format = RQKosFormat.FormatEnum.rdf;
                        break;
                    default:
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
                        //format = RQKosFormat.FormatEnum.undefined;
                        //break;
                }
                return format;
            }
            else
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
        }

        #endregion 

        #region public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("rqkos")]
        [HttpGet]
        public RQKosBranch Get()
        {
            if (System.Web.HttpContext.Current.Request.Headers.Get("Accept").ToLower().Contains("text/html"))
                throw new HttpResponseException(JsonErrorResponse.Redirect(Request.RequestUri.ToString().Replace("rqld/rqkos", "rqkos/RQKosLD/0")));
            else
                return new RQKosModel(null, "rqld", this.GetFormat()).RQKosSet;
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
                return new RQKosModel(id, "rqld", this.GetFormat()).RQKosSet;
        }

        #endregion
    }
}
