using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace Mvc5RQ.Areas.LinkedDataCalls
{
  public interface ILinkedDataCallsService
  {
      string[] GetLinkedDataPredicates(string subjectID);

      Dictionary<string, string> GetLinkedDataDictionary(string subjectID);

  }
}
