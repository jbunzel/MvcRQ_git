using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;

using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Mvc5RQ.Exceptions
{
    public class MyJsonResult
    {
        public MyJsonResult() {}

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

    public class JsonContent : HttpContent
    {
        private readonly Newtonsoft.Json.Linq.JToken _value;

        public JsonContent(MyJsonResult value)
        {
            _value = Newtonsoft.Json.Linq.JObject.Parse(new JavaScriptSerializer().Serialize(value));
            Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        }

        protected override System.Threading.Tasks.Task SerializeToStreamAsync(System.IO.Stream stream, System.Net.TransportContext context)
        {
            var jw = new Newtonsoft.Json.JsonTextWriter(new System.IO.StreamWriter(stream))
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            };
            _value.WriteTo(jw);
            jw.Flush();
            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}
