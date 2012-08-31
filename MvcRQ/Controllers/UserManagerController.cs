using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcRQ.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [MvcRQ.Helpers.RQAuthorize(Roles="admin")]    
    public class UserManagerController : BaseController
    {
        //
        // GET: /UserManager/

        /// <summary>
        /// Index Action
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

    }
}
