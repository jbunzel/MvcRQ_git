using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.Mvc;
using System.Web;

using RQLib.RQQueryResult.RQDescriptionElements;
using RQLib.RQKos.Persons;
using RQLib.RQLD;

//using MvcRQ.Areas.LinkedDataCalls.Models;

namespace Mvc5RQ.Areas.LinkedDataCalls
{
  public class LinkedDataCallsService : ILinkedDataCallsService
  {
      private RQLDGraph GetLDGraph(RQDescriptionComponent entity)
      {
          RQLDGraph ldGraph = entity.DataClient.LDGraph;

          entity.EnableLinkedData();
          entity.Load();
          entity.DisableLinkedData();
          return ldGraph;
      }

      public LinkedDataCallsService()
      { }

      public string[] GetLinkedDataPredicates(string subjectId)
      {
          //Person pers = new Person(subjectId, Person.PersonDataSystems.gnd);
          //RQPersonGraph persGraph = (RQPersonGraph)(((LDPersonDataClient)pers.PersonDataClient).LDGraph);

          //pers.EnableLinkedData();
          //pers.Load();
          //pers.DisableLinkedData();
          //return persGraph.GetPredicates(pers.PersonID);

          Person pers = new Person(subjectId, Person.PersonDataSystems.gnd);

          return ((RQPersonGraph)GetLDGraph(pers)).GetPredicates(pers.PersonID);
      }

      public Dictionary<string, string> GetLinkedDataDictionary(string subjectId)
      {
          Person pers = new Person(subjectId, Person.PersonDataSystems.gnd);

          return ((RQPersonGraph)GetLDGraph(pers)).GetPredicateObjects(pers.PersonID);
      }

   }
}