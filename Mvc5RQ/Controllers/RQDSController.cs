using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Mvc5RQ.Models;

using System.Threading;
using System.Threading.Tasks;

using System.IO;
using System.Net.Http.Headers;

namespace Mvc5RQ.Controllers
{
    //[Authorize]
    [RoutePrefix("rqds")]
    public class RQDSController : ApiController
    {
        /// <summary>
        /// Returns a list if RQItems
        /// </summary>
        /// <param name="dbname"></param>
        /// <returns></returns>
        [Route("{dbname}/{format}")]
        [HttpGet]
        public RQItemModel Get(string dbname)
        {
            //if (System.Web.HttpContext.Current.Request.Headers.Get("Accept").ToLower().Contains("text/html"))
            //{
                RQItemModel model = new Mvc5RQ.Models.RQItemModel(new RQLib.RQQueryForm.RQquery("Bunzel"), false, new ModelParameters(ModelParameters.FormatEnum.oai_dc));

            //    return new HtmlActionResult("~/Views/RQItems/ServRQItem.cshtml", model);
            //}
            //else
            //    return this.Content<string>(HttpStatusCode.OK, "", new System.Net.Http.Formatting.XmlMediaTypeFormatter(), "text/html");
                return model;
        }

        // GET api/values/5
        [Route("{dbname}/{id:int}/{format}")]
        [HttpGet]
        public RQItem Get(string dbname, int id)
        {
            RQItemModel model = new Mvc5RQ.Models.RQItemModel(new RQLib.RQQueryForm.RQquery("08100"), false, new ModelParameters(ModelParameters.FormatEnum.oai_dc));

            return model.RQItems.GetItem(0);
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

    public class TextResult : IHttpActionResult
    {
        string _value;
        HttpRequestMessage _request;

        public TextResult(string value, HttpRequestMessage request)
        {
            _value = value;
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                Content = new StringContent(_value),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }
    }

    public class HtmlActionResult : IHttpActionResult
    {
        //private const string ViewDirectory = @"E:\dev\ConsoleApplication8\ConsoleApplication8";
        private readonly string _view;
        private readonly dynamic _model;

        public HtmlActionResult(string viewName, dynamic model)
        {
            _view = LoadView(viewName);
            _model = model;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var parsedView = RazorEngine.Razor.Parse(_view, _model);
            response.Content = new StringContent(parsedView);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return Task.FromResult(response);
        }

        private static string LoadView(string name)
        {
            var view = File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath(name));
            return view;
        }
    }
}
