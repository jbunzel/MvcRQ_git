using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Mvc5RQ.Exceptions;
using Mvc5RQ.Areas.Desktop.Models;

namespace Mvc5RQ.Areas.Desktop.Controllers
{
    /// <summary>
    /// Riquest Desktop Service (RQDT)
    /// </summary>
    [RoutePrefix("rqdt")]
    [ExceptionHandling]
    public class RQDTController : ApiController
    {
        #region private members

        private IDesktopManager _desktopManager;

        #endregion

        #region public members

        public IDesktopManager DesktopManager
        {
            get
            {
                return _desktopManager ?? Mvc5RQ.Areas.Desktop.Models.DesktopManagerFactory.Get<IDesktopManager>();
            }
            private set
            {
                _desktopManager = value;
            }
        }

        #endregion

        #region public controllers

        public RQDTController()
        {
        }

        public RQDTController(IDesktopManager desktopManager)
        {
            DesktopManager = desktopManager;
        }

        #endregion

        #region action methods

        [Route("")]
        [HttpGet]
        public DesktopViewModel Get()
        {
            return DesktopManager.Load();
        }

        [Route("{id}")]
        [HttpGet]
        public DesktopViewModel Get(string id )
        {
            return null;
        }

        [Route("export")]
        [HttpPost]
        public IHttpActionResult Export([FromBody]string projectName)
        {
            DesktopManager.ConvertProject(projectName);
            return Ok();
        }

        #endregion
        
    }
}
