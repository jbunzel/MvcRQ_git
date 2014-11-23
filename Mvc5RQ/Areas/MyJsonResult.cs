using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mvc5RQ.Areas
{
  public class MyJsonResult
  {

    public MyJsonResult()
    {

    }

    public string message { get; set; }
    public bool isSuccess { get; set; }
    public object data { get; set; }
    public object hints { get; set; }

    internal static MyJsonResult CreateSuccess(string message)
    {
      return new MyJsonResult()
      {
        message = message,
        isSuccess = true
      };
    }

    internal static MyJsonResult CreateError(string message)
    {
      return new MyJsonResult()
      {
        message = message,
        isSuccess = false
      };
    }

    internal static MyJsonResult CreateError(Exception ex)
    {
      return new MyJsonResult()
      {
        message = ex.Message,
        isSuccess = false,
        data = new { stacktrace = ex.StackTrace }
      };
    }

    internal static MyJsonResult CreateError(Mvc5RQ.Models.RQKosBranch.RQKosBranchStatus status)
    {
        return new MyJsonResult()
        {
            message = status.message,
            isSuccess = false,
            hints = status.hints,
        };
    }
  }
}
