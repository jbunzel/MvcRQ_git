using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Runtime.Serialization;
using System.Xml;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MvpRestApiLib
{
    // Source: http://www.hackersbasement.com/csharp/post/2009/06/07/XmlResult-for-ASPNet-MVC.aspx
    public class XmlResult : ActionResult
    {
        public XmlResult() { }
        public XmlResult(object data) { this.Data = data; }

        public string ContentType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public object Data { get; set; }

        public string XSLTransform { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
                response.ContentType = this.ContentType;
            else
                response.ContentType = "text/xml";

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            if (this.Data != null)
            {
                if (this.Data is XmlNode)
                    response.Write(((XmlNode)this.Data).OuterXml);
                else if (this.Data is XNode)
                    response.Write(((XNode)this.Data).ToString());
                else
                {
                    var dataType = this.Data.GetType();
                    // OMAR: For generic types, use DataContractSerializer because 
                    // XMLSerializer cannot serialize generic interface lists or types.
                    if (dataType.IsGenericType || 
                        dataType.GetCustomAttributes(typeof(DataContractAttribute), true).FirstOrDefault() != null)
                    {
                        var dSer = new DataContractSerializer(dataType);

                        if ((XSLTransform == null) || (XSLTransform.Length == 0))
                            dSer.WriteObject(response.OutputStream, this.Data);
                        else
                        {
                            System.IO.MemoryStream ms = new System.IO.MemoryStream();
                            var xTrf = new System.Xml.Xsl.XslCompiledTransform();

                            dSer.WriteObject(ms, this.Data);
                            System.IO.TextReader tr = new System.IO.StringReader(Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Position));
                            xTrf.Load(HttpContext.Current.Server.MapPath(XSLTransform));
                            xTrf.Transform(new System.Xml.XmlTextReader(tr), null, response.OutputStream);
                        }
                    }
                    else
                    {
                        var xSer = new XmlSerializer(dataType);

                        if ((XSLTransform == null) || (XSLTransform.Length == 0))
                            xSer.Serialize(response.OutputStream, this.Data);
                        else
                        {
                            System.IO.MemoryStream ms = new System.IO.MemoryStream();
                            var xTrf = new System.Xml.Xsl.XslCompiledTransform();
                            //var mstr = new XmlTextWriter(new System.IO.MemoryStream(), Encoding.UTF8);

                            //xSer.Serialize(mstr, this.Data);
                            xSer.Serialize(ms, this.Data);
                            System.IO.TextReader tr = new System.IO.StringReader(Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Position));
                            xTrf.Load(HttpContext.Current.Server.MapPath(XSLTransform));
                            //mstr.BaseStream.Flush();
                            //mstr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                            //xTrf.Transform(new System.Xml.XPath.XPathDocument(mstr.BaseStream), null, response.OutputStream);
                            xTrf.Transform(new System.Xml.XPath.XPathDocument(tr), null, response.OutputStream);
                        }
                    }
                }
            }
        }
    }
}
