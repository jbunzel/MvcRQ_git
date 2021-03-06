﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Mvc5RQ.Models;

namespace Mvc5RQ.Areas.UserSettings.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingsDBInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<SettingsDBContext>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(SettingsDBContext context)
        {
            var datafields = new List<DataField> 
            { 
                new DataField { Name = "Titel" },
                new DataField { Name = "Autoren" },
                new DataField { Name = "Ort" },
                new DataField { Name = "Verlag" },
                new DataField { Name = "Jahr" }
            };
            datafields.ForEach(s => context.DataFields.Add(s));
            context.SaveChanges();

            var databases = new List<Database> 
            { 
                new Database { Name = "Nationallizenzen", Uri="http://www.nlz.com", DataFields = new List<DataField>() }, 
                new Database { Name = "GBV Verbunddatenbank", Uri="http://www.gbv.de", DataFields = new List<DataField>() }
            };
            databases.ForEach(s => context.Databases.Add(s));
            context.SaveChanges();

            databases[0].DataFields.Add(datafields[0]);
            databases[0].DataFields.Add(datafields[1]);
            databases[0].DataFields.Add(datafields[2]);
            databases[0].DataFields.Add(datafields[3]);
            databases[0].DataFields.Add(datafields[4]);
            context.SaveChanges();

            databases[1].DataFields.Add(datafields[0]);
            databases[1].DataFields.Add(datafields[1]);
            databases[1].DataFields.Add(datafields[2]);
            databases[1].DataFields.Add(datafields[3]);
            databases[1].DataFields.Add(datafields[4]);
            context.SaveChanges();

            var sortOptions = new List<SortOption> 
            { 
                new SortOption { Name = "nach Aufstellung", XsltTransform = "Aufstellung.xslt" },
                new SortOption { Name = "nach Titel", XsltTransform = "Titel.xslt" },
                new SortOption { Name = "nach Fach", XsltTransform = "Fach.xslt" },
                new SortOption { Name = "nach Erscheinungsjahr", XsltTransform = "Erscheinungsjahr.xslt" },
            };
            sortOptions.ForEach(s => context.SortOptions.Add(s));
            context.SaveChanges();

            ApplicationUser primaryUser = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindByName("jbunzel@riquest.de");

            if (primaryUser != null)
            {
                Guid ui = new Guid(primaryUser.Id);
                var queryOptions = new List<QueryOptions> 
                { 
                    new QueryOptions { IncludeExternal = true, 
                                       Databases = new List<Database>(), 
                                       DataFields = new List<DataField>(), 
                                       SortOptionId = 1, UserId = ui } //"a47fb0d7-0c46-472e-b7b0-471e68a3cd80"
                };
                queryOptions.ForEach(s => context.QueryOptions.Add(s));
                context.SaveChanges();

                queryOptions[0].Databases.Add(databases[0]);
                queryOptions[0].Databases.Add(databases[1]);
                queryOptions[0].DataFields.Add(datafields[0]);
                queryOptions[0].DataFields.Add(datafields[1]);
                queryOptions[0].DataFields.Add(datafields[2]);
                queryOptions[0].DataFields.Add(datafields[3]);
                queryOptions[0].DataFields.Add(datafields[4]);
                context.SaveChanges();
            }
         }
    }
}