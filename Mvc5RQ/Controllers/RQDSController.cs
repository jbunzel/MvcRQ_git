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
    /// <summary>
    /// 
    /// </summary>
    //[Authorize]
    [RoutePrefix("rqds")]
    public class RQDSController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbname"></param>
        /// <param name="format"></param>
        /// <param name="verb"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        [Route("{dbname}/{format}")]
        [HttpGet]
        public RQItemModel Get(string dbname, string format, string verb = "", string queryString = "")
        {
            //if (System.Web.HttpContext.Current.Request.Headers.Get("Accept").ToLower().Contains("text/html"))
            //{

            try 
            {
                RQItemModelRepository repo = new RQItemModelRepository(new FormatParameter((FormatParameter.FormatEnum)Enum.Parse(typeof(FormatParameter.FormatEnum), format)));
                
                return repo.GetModel(queryString, Areas.UserSettings.UserState.StateType(verb));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbname"></param>
        /// <param name="id"></param>
        /// <param name="format"></param>
        /// <param name="verb"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        [Route("{dbname}/{id}/{format}")]
        [HttpGet]
        public RQItem Get(string dbname, string id, string format, string verb = "", string queryString = "")
        {
            RQItemModelRepository repo = new RQItemModelRepository(new FormatParameter((FormatParameter.FormatEnum)Enum.Parse(typeof(FormatParameter.FormatEnum), format + ((format == "rqi") ? "_single_item" : ""))));

            return repo.GetRQItem(id);
        }

        //// POST api/values
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //public void Delete(int id)
        //{
        //}
    }

    //public class TextResult : IHttpActionResult
    //{
    //    string _value;
    //    HttpRequestMessage _request;

    //    public TextResult(string value, HttpRequestMessage request)
    //    {
    //        _value = value;
    //        _request = request;
    //    }

    //    public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
    //    {
    //        var response = new HttpResponseMessage()
    //        {
    //            Content = new StringContent(_value),
    //            RequestMessage = _request
    //        };
    //        return Task.FromResult(response);
    //    }
    //}

    //public class HtmlActionResult : IHttpActionResult
    //{
    //    //private const string ViewDirectory = @"E:\dev\ConsoleApplication8\ConsoleApplication8";
    //    private readonly string _view;
    //    private readonly dynamic _model;

    //    public HtmlActionResult(string viewName, dynamic model)
    //    {
    //        _view = LoadView(viewName);
    //        _model = model;
    //    }

    //    public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
    //    {
    //        var response = new HttpResponseMessage(HttpStatusCode.OK);
    //        var parsedView = RazorEngine.Razor.Parse(_view, _model);
    //        response.Content = new StringContent(parsedView);
    //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
    //        return Task.FromResult(response);
    //    }

    //    private static string LoadView(string name)
    //    {
    //        var view = File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath(name));
    //        return view;
    //    }
    //}
}
