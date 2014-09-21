using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.Mvc;
using System.Web;

using Mvc5RQ.Areas.UserSettings.Models;

namespace Mvc5RQ.Areas.UserSettings
{
  public class UserSettingsService : IUserSettingsService
  {
      private SettingsDBContext db = new SettingsDBContext();

      private Guid GetGuestId()
      {
          UserState us = UserState.Get(UserState.States.GuestIdState);

          if (us == null) // Create a new guest user id.
              us = new GuestIdState(Guid.NewGuid());
          return ((GuestIdState)us).GuestId;
      }

      protected SortOption GetDefaultSortOption()
      {
          SortOption so = db.SortOptions.FirstOrDefault(SortOptions => SortOptions.Name.Equals("nach Fach"));

          if (so == null)
          {
              so = new SortOption
              {
                  Name = "nach Fach",
                  XsltTransform = ""
              };
              db.SortOptions.Add(so);
              db.SaveChanges();
          }
          return so;
      }

      protected QueryOptions GetQueryOptions()
      {
          MembershipUser user = Membership.GetUser();
          Guid ui;
          QueryOptions qo;

          if (user == null) 
              ui = GetGuestId();       // Get the guest user id.
          else
            ui = (Guid)user.ProviderUserKey;
          qo = db.QueryOptions.FirstOrDefault(QueryOptions => QueryOptions.UserId.Equals(ui));
          if (qo == null)
          {   // create the default user profile
              // SortOption so = db.SortOptions.FirstOrDefault(SortOptions => SortOptions.Name.Equals("nach Fach"));
              SortOption so = GetDefaultSortOption();

              qo = new QueryOptions
              {
                  IncludeExternal = false,
                  Databases = new List<Database>(),
                  DataFields = new List<DataField>(),
                  SortOptionId = so.SortOptionId,
                  UserId = ui
              };
              db.QueryOptions.Add(qo);
              db.SaveChanges();
          }
          return qo;
      }

      public UserSettingsService()
      { }

      public bool GetIncludeExternal()
      {
          return this.GetQueryOptions().IncludeExternal;
      }

      public bool ChangeIncludeExternal()
      {
          QueryOptions qo = this.GetQueryOptions();
              
          qo.IncludeExternal = qo.IncludeExternal == true ? false : true;
          db.SaveChanges();
          return qo.IncludeExternal;
      }

      public string[] GetAllExternalDatabases()
      {
          var s = db.Databases.Select(c => new { c.Name });
          List<string> ret = new List<string>(s.Count());

          foreach (var t in s)
          {
              ret.Add(t.Name);
          }
          return ret.ToArray<string>();
      }

      public string[] GetExternalDatabasesForUser()
      {
          List<string> ret = new List<string>();

          foreach (var t in this.GetQueryOptions().Databases)
          {
              ret.Add(t.Name);
          };
          return ret.ToArray<string>();
      }

      public void AddRemoveDatabaseForUser(string databasename, bool included)
      {
          QueryOptions qo = this.GetQueryOptions();

          if (included)
          {
              ICollection<Database> udb = qo.Databases;
              Database dbt = db.Databases.FirstOrDefault(c=>c.Name == databasename);

              udb.Add(dbt); 
          }
          else
          {
              Database udb = qo.Databases.FirstOrDefault(c => c.Name == databasename);

              qo.Databases.Remove(udb);
          }
          db.SaveChanges();
      }

      public ImportOptions GetImportOptions()
      {
          return new ImportOptions();
      }
   }
}
