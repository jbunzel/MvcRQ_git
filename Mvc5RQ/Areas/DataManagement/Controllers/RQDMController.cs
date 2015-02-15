using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using Mvc5RQ.Helpers;
using Mvc5RQ.Exceptions;

using RQLib.RQDAL;

namespace Mvc5RQ.Areas.DataManagement.Controllers
{
    [RoutePrefix("rqdm")]
    [ExceptionHandling]
    public class RQDMController : ApiController
    {
        [HttpPost]
        [RQAuthorize(Roles="admin")]
        [Route("lucene/new")]
        public string LuceneNew()
        {
            new RQLuceneDBI().Reindex();
            return Mvc5RQ.Areas.DataManagement.Resources.DataManagement.dm_lucenenew_ok;
        }

        [HttpPost]
        [RQAuthorize(Roles="admin")]
        [Route("lucene/optimize")]
        public string LuceneOptimize()
        {
            new RQLuceneDBI().Optimize();
            return Mvc5RQ.Areas.DataManagement.Resources.DataManagement.dm_luceneoptimize_ok; 
        }

        [HttpPost]
        [RQAuthorize(Roles = "admin")]
        [Route("bookmarks/new")]
        public string New()
        {
            string path = "D:\\Users\\Public\\MyVLib\\My Virtual Subject Library\\";
            string name = "xml/dir.xml";
            RQBookmarkDAL VLD = new RQBookmarkDAL();
            System.IO.DirectoryInfo VLDir = new System.IO.DirectoryInfo(path);

            if (VLDir.Exists)
            {
                VLD.LoadBookmarks(ref path, ref name);
            }
            else
                throw new HttpResponseException(JsonErrorResponse.Create(System.Net.HttpStatusCode.NotFound, Mvc5RQ.Areas.DataManagement.Resources.DataManagement.dm_new_err_directory_not_found));
            return Mvc5RQ.Areas.DataManagement.Resources.DataManagement.dm_new_ok;
        }
    }
}