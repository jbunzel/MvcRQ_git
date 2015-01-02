using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Net.Http;
using System.Web.Http;
//using System.Web.Mvc;
using Mvc5RQ.Helpers;
using RQLib.RQDAL;

namespace Mvc5RQ.Areas.DataManagement.Controllers
{
    [RoutePrefix("rqdm")]
    public class RQDMController : ApiController
    {
        [HttpPost]
        [RQAuthorize(Roles="admin")]
        [Route("lucene/new")]
        public string LuceneNew()
        {
            try
            {
                new RQLuceneDBI().Reindex();
            }
            catch (Exception ex)
            {
                throw new AccessViolationException(ex.Message);
            }
            return "The Lucene index has been successfully recreated and optimized!";
        }

        [HttpPost]
        [RQAuthorize(Roles="admin")]
        [Route("lucene/optimize")]
        public string LuceneOptimize()
        {
            try
            {
                new RQLuceneDBI().Optimize();
            }
            catch (Exception ex)
            {
                throw new AccessViolationException(ex.Message);
            }
            return "The Lucene index has been successfully optimized!"; 
        }

        [HttpPost]
        [RQAuthorize(Roles = "admin")]
        [Route("bookmarks/new")]
        public string New()
        {
            try
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
                    throw new AccessViolationException("Bookmark directory does not exist!");
            }
            catch (Exception ex)
            {
                throw new AccessViolationException(ex.Message);
            }
            return "Bookmark directory has been successfully indexed!";
        }
    }
}
