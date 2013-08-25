using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.Mvc;
using System.Web;

using RQLib.RQKos.Persons;
using RQLib.RQLD;

//using MvcRQ.Areas.LinkedDataCalls.Models;

namespace MvcRQ.Areas.LinkedDataCalls
{
  public class LinkedDataCallsService : ILinkedDataCallsService
  {
      public LinkedDataCallsService()
      { }

      public string[] GetLinkedDataPredicates(string subjectId)
      {
          Person pers = new Person(subjectId, Person.PersonDataSystems.gnd);
          //PersonDataClient persDC = pers.PersonDataClient;
          //RQPersonGraph persGraph = (RQPersonGraph)(((LDPersonDataClient)persDC).LDGraph);
          RQPersonGraph persGraph = (RQPersonGraph)(((LDPersonDataClient)pers.PersonDataClient).LDGraph);

          pers.EnableLinkedData();
          pers.Load();
          pers.DisableLinkedData();
          return persGraph.GetPredicates(pers.PersonID);
      }
   }
}