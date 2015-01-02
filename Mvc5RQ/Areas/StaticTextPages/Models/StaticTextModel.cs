using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc5RQ.Areas.StaticTextPages.Models
{
    public class StaticTextModel
    {
        #region public properties

        public string TextHtml { get; set; }
          
        #endregion

        #region public constructors

        public StaticTextModel (string textResourceName, string xsltAdress)
        {
            var xRdr = System.Xml.Linq.XDocument.Parse((string)(RQResources.Views.Shared.SharedStrings.ResourceManager.GetObject(textResourceName)));

            var xTrf = new System.Xml.Xsl.XslCompiledTransform();
            var xTrfArg = new System.Xml.Xsl.XsltArgumentList();
            var xSet = new System.Xml.Xsl.XsltSettings(enableDocumentFunction:true, enableScript:true);
            var mstr = new System.IO.StringWriter(new System.Text.StringBuilder());

            xTrf.Load(HttpContext.Current.Server.MapPath(xsltAdress), xSet, new System.Xml.XmlUrlResolver());
            xTrf.Transform(xRdr.CreateReader(), xTrfArg, mstr);
            this.TextHtml = mstr.ToString();
        }

        #endregion
    }
}