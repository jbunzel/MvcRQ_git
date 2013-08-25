using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace MvcRQ.Areas.LinkedDataCalls
{
  public interface ILinkedDataCallsService
  {
      string[] GetLinkedDataPredicates(string subjectID);

  }
}
