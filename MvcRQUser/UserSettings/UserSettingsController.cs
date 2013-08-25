using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcRQUser.UserSettings
{
    public class UserSettingsController : Controller
    {
        /// <summary>
        /// The settings service which should be used for any provider interaction
        /// </summary>
        public IUserSettingsService _settingsService { get; set; }

        public static void RegisterMe()
        {
            var routes = RouteTable.Routes;

            using (routes.GetWriteLock())
            {
                routes.MapRoute("UserSettingsRoute",
                  "UserSettings/{action}",
                  new { controller = "UserSettings", action = "GetIncludeExternal" },
                  new string[] { "MvcRQUser.UserSettings" });
            }
        }

        /// <summary>
        /// Initialize controller and the settings service
        /// </summary>
        /// <param name="requestContext">Current http request context</param>
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (this._settingsService == null) this._settingsService = new UserSettingsService();
        }
                
        /// <summary>
        /// Returns the includeExternal user settings
        /// </summary>
        /// <returns>includeExternal Settings</returns>
        public JsonResult GetIncludeExternal()
        {
            return Json(this._settingsService.GetIncludeExternal());
        }

        public JsonResult ChangeIncludeExternal()
        {
            return Json(this._settingsService.ChangeIncludeExternal());
        }

        public JsonResult GetExternalDatabaseStatus()
        {
            //if (string.IsNullOrEmpty(username))
                //throw new ArgumentException("No user name specified in request");

            var allDatabases = this._settingsService.GetAllExternalDatabases();
            var userDatabases = this._settingsService.GetExternalDatabasesForUser();

            var result = new MyJsonResult()
            {
                data = from database in allDatabases
                       select new
                       {
                           databasename = database,
                           included = userDatabases.Contains(database)
                       },
                isSuccess = true
            };
            return Json(result);
        }

        public JsonResult AddRemoveDatabaseForUser(string databasename, bool included)
        {
            MyJsonResult result;

            try
            {
                var action = included ? "added" : "removed";
                var msg = string.Format("The database {0} has been {1}.", databasename, action);

                _settingsService.AddRemoveDatabaseForUser(databasename, included);
                result = MyJsonResult.CreateSuccess(msg);
            }
            catch (ArgumentException ex)
            {
                result = MyJsonResult.CreateError("Could not remove database: " + ex.Message);
            }
            return Json(result);
        }
    }
}
