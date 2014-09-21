using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mvc5RQ.Controllers
{
    //[Authorize]
    [RoutePrefix("rqld")]
    public class RQLDController : ApiController
    {
        // GET api/values
        [Route("{dbname}")]
        [HttpGet]
        public IEnumerable<string> Get(string dbname)
        {
            //if (System.Web.HttpContext.Current.Request.Headers.Get("Accept").ToLower().Contains("text/html"))
            //    System.Web.HttpContext.Current.Response.Redirect("http://www.riquest.de/rqld/rqkos");
            return new string[] { "rqld1", "rqld2" };
        }

        // GET api/values/5
        [Route("{dbname}/{id:int}")]
        public string Get(string dbname, int id)
        {
            return "rqld" + id;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
