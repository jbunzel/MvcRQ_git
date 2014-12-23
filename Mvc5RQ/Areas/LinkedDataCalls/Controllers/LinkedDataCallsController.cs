using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mvc5RQ.Areas.LinkedDataCalls.Controllers
{
    public class LinkedDataCallsController : Controller
    {
        //
        // GET: /LinkedDataCalls/LinkedDataCalls/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// The LinkedData service which should be used for any provider interaction
        /// </summary>
        public ILinkedDataCallsService _LinkedDataService { get; set; }

        /// <summary>
        /// Initialize controller and the LinkedData service
        /// </summary>
        /// <param name="requestContext">Current http request context</param>
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (this._LinkedDataService == null) this._LinkedDataService = new LinkedDataCallsService();
        }

        /// <summary>
        /// Returns the includeExternal user LinkedData
        /// </summary>
        /// <returns>includeExternal LinkedData</returns>
        public JsonResult GetLinkedDataPredicates( string id)
        {
            //var allPredicates = this._LinkedDataService.GetLinkedDataPredicates(id);
            var allPredicates = this._LinkedDataService.GetLinkedDataDictionary(id);

            var result = new MyJsonResult()
            {
                data = from predicate in allPredicates
                       select new
                       {
                           predicatename = predicate.Key,
                           objectvalue = predicate.Value,
                           included = false
                       },
                isSuccess = true
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
