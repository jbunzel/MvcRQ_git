using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RQRepository.Handlers
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für AccessControlHandler
    /// </summary>
    public class AccessControl : IHttpModule
    {
        private bool IsRQDLSite(HttpRequest request)
        {
            string uripath = request.UrlReferrer.GetLeftPart(UriPartial.Path);

            if (uripath.StartsWith("http://www.riquest.de")) return true;
            if (uripath.StartsWith("http://www.riquest.net")) return true;
            if (uripath.StartsWith("http://www.strands.de")) return true;
            if (uripath.StartsWith("http://mydocs.strands.de")) return true;
            if (uripath.StartsWith("http://localhost/MvcRQ/ItemViewer")) return true;   // for debugging in development environment
            return false;
        }

        public AccessControl()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += delegate(Object sender, EventArgs ea)
            {
                HttpApplication ha = sender as HttpApplication;
                if (ha != null)
                {
                    string pattern = @"(?<id>[0-9]{5})(?<permits>[a|b|c]*)";
                    string path = ha.Server.MapPath(ha.Context.Request.CurrentExecutionFilePath);

                    if (ha.Context.Request.CurrentExecutionFilePath == "/") 
                        return;                                                                                                     // calls to base directory pass & return default.aspx
                    if (ha.Context.Request.ServerVariables["HTTP_USER_AGENT"].ToLower().Contains("googlebot")) 
                        return;                                                                                                     // document requests from googlebot are serviced    
                    if (   (ha.Context.Request.UrlReferrer != null) 
                        && !IsRQDLSite(ha.Context.Request) )                                                                        // document requests not from RQDL are redirected to the RQDL document description
                        ha.Context.Response.Redirect("http://www.riquest.de/rqitems/" + System.Text.RegularExpressions.Regex.Match(path, pattern).Groups["id"]);
                    if (   (ha.Context.Request.UrlReferrer == null)                                                                 
                        && (ha.Context.Request.ServerVariables["LOCAL_ADDR"] != ha.Context.Request.ServerVariables["REMOTE_ADDR"])) // document requests from unknown are redirected to the RQDL document description
                        ha.Context.Response.Redirect("http://www.riquest.de/rqitems/" + System.Text.RegularExpressions.Regex.Match(path, pattern).Groups["id"]);
                    if (System.IO.File.Exists(path))                                                                                // if document request is either from RQDL or from origin identical to target                                                                             
                        return;                                                                                                     // return requested file if credentials are OK 
                    else
                    {
                        string replacement = "${id}";                                                                               // otherwise try with unnecessary credentials removed 
                        path = System.Text.RegularExpressions.Regex.Replace(path, pattern, replacement);
                        
                        if (System.IO.File.Exists(path))
                        {
                            string uri = System.Text.RegularExpressions.Regex.Replace(ha.Context.Request.Url.LocalPath, pattern, replacement);

                            ha.Context.Server.Transfer(uri);
                        }
                        else
                        {
                            if (ha.Context.Request.CurrentExecutionFilePath.ToLower().StartsWith("/mydocs"))
                                ha.Context.Server.Transfer("/MyDocs/00000_Default/Default.html");
                            if (ha.Context.Request.CurrentExecutionFilePath.ToLower().StartsWith("/mymusic"))
                                ha.Context.Server.Transfer("/MyMusic/00000_Default/default.m3u");
                            if (ha.Context.Request.CurrentExecutionFilePath.ToLower().StartsWith("/myvideos"))
                                ha.Context.Server.Transfer("/MyVideos/00000_Default/default.m4v");
                        }
                    }
                }        
            };
        }

        public void Dispose()
        {
        }
    }
}