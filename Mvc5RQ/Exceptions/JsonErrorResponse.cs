using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters; 

namespace Mvc5RQ.Exceptions
{
    public static class JsonErrorResponse
    {
        public static HttpResponseMessage Redirect(string uri)
        {
            var ret = new HttpResponseMessage(System.Net.HttpStatusCode.Redirect);

            ret.Headers.Location = new Uri(uri);
            return ret;
        }

        public static HttpResponseMessage Create(Mvc5RQ.Models.RQKosBranch.RQKosBranchStatus status)
        {
            MyJsonResult json = MyJsonResult.CreateError(status);

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new JsonContent(json),
            };
        }

        public static HttpResponseMessage Create(System.Net.HttpStatusCode status, string message)
        {
            MyJsonResult json = MyJsonResult.CreateError(message);

            return new HttpResponseMessage(status)
            {
                Content = new JsonContent(json)
            };
        }

        public static HttpResponseMessage Create(Exception exception, string message)
        {
            MyJsonResult json;
            Exception iex = exception;

            while (iex != null)
            {
                if (!string.IsNullOrEmpty(iex.Message))
                    message += "\n - " + iex.Message;
                iex = iex.InnerException;
            }
            json = MyJsonResult.CreateError(message);
            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new JsonContent(json),
            };
        }

        public static HttpResponseMessage UnknownError()
        {
            MyJsonResult json = MyJsonResult.CreateError(RQResources.Views.Shared.SharedStrings.err_unknown);

            return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            {
                Content = new JsonContent(json)
            };
        }

        public static HttpResponseMessage NotFound()
        {
            MyJsonResult json = MyJsonResult.CreateError(RQResources.Views.Shared.SharedStrings.err_notfound);

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            {
                Content = new JsonContent(json)
            };
        }

        public static HttpResponseMessage NotImplemented()
        {
            MyJsonResult json = MyJsonResult.CreateError(RQResources.Views.Shared.SharedStrings.err_notimplemented);

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotImplemented)
            {
                Content = new JsonContent(json)
            };
        }

        public static HttpResponseMessage InvalidData()
        {
            MyJsonResult json = MyJsonResult.CreateError("Data supplied are invalid.");

            return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            {
                Content = new JsonContent(json)
            };
        }
    }

    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is NotImplementedException)
                throw new HttpResponseException(JsonErrorResponse.NotImplemented());
            else if (context.Exception is NotFoundException)
                throw new HttpResponseException(JsonErrorResponse.NotFound());
            else
            {
                //Log Critical errors
                //Debug.WriteLine(context.Exception);
                if (context.ActionContext.ControllerContext.ControllerDescriptor.ControllerName == "RQDS")
                    if (context.ActionContext.ActionDescriptor.ActionName == "Get")
                        switch (context.ActionContext.ActionDescriptor.ReturnType.Name)
                        {
                            case "RQItemModel":
                                throw new HttpResponseException(JsonErrorResponse.Create(context.Exception, RQResources.Views.Shared.SharedStrings.err_rqds_get_list));
                            case "RQItem":
                                throw new HttpResponseException(JsonErrorResponse.Create(context.Exception, RQResources.Views.Shared.SharedStrings.err_rqds_get_item));
                            default:
                                throw new HttpResponseException(JsonErrorResponse.Create(context.Exception, RQResources.Views.Shared.SharedStrings.error));
                        }
                    else if (context.ActionContext.ActionDescriptor.ActionName == "Post")
                        switch (context.ActionContext.ActionDescriptor.ReturnType.Name)
                        {
                            case "RQItemModel":
                                throw new HttpResponseException(JsonErrorResponse.Create(context.Exception, RQResources.Views.Shared.SharedStrings.err_rqds_post_add));
                            case "RQItem":
                                throw new HttpResponseException(JsonErrorResponse.Create(context.Exception, RQResources.Views.Shared.SharedStrings.err_rqds_post_update));
                            default:
                                throw new HttpResponseException(JsonErrorResponse.Create(context.Exception, RQResources.Views.Shared.SharedStrings.error));
                        }
                    else
                        throw new HttpResponseException(JsonErrorResponse.Create(context.Exception, RQResources.Views.Shared.SharedStrings.error));
                else
                    throw new HttpResponseException(JsonErrorResponse.Create(context.Exception, RQResources.Views.Shared.SharedStrings.error));
            }
        }
    }
}