using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Mvc5RQ.Formatters
{
    public class XMLCustomFormatter : XmlMediaTypeFormatter
    {
        public string XSLTransform { get; set; }

        public XMLCustomFormatter()
        {
            //SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
            //SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
        }
        
        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            if ((type == typeof(Mvc5RQ.Models.RQItemModel)) || (type == typeof(Mvc5RQ.Models.RQItem)))
                return true;
            else
                return false;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                if ((type == typeof(Mvc5RQ.Models.RQItemModel)) || (type == typeof(Mvc5RQ.Models.RQItem)))
                {
                    if (type == typeof(Mvc5RQ.Models.RQItemModel))
                        XSLTransform = ((Mvc5RQ.Models.RQItemModel)value).RQItems.preprocessor.XmlTransformPath;
                    else
                        //XSLTransform = ((Mvc5RQ.Models.RQItem)value).preprocessor.XmlTransformPath;
                    WriteXML(value, writeStream, content);
                }
            });
        }

        public void WriteXML(object value, Stream writeStream, HttpContent content)
        {
            if (value != null)
            {
                var dataType = value.GetType();
                // OMAR: For generic types, use DataContractSerializer because 
                // XMLSerializer cannot serialize generic interface lists or types.
                if (dataType.IsGenericType || 
                    dataType.GetCustomAttributes(typeof(DataContractAttribute), true).FirstOrDefault() != null)
                {
                    var dSer = new DataContractSerializer(dataType);

                    if ((XSLTransform == null) || (XSLTransform.Length == 0))
                        dSer.WriteObject(writeStream, value);
                    else
                    {
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        var xTrf = new System.Xml.Xsl.XslCompiledTransform();

                        dSer.WriteObject(ms, value);
                        //TESTDATEI(EZEUGEN)
                        //System.Xml.XmlDocument Doc = new System.Xml.XmlDocument();
                        //ms.Seek(0, System.IO.SeekOrigin.Begin);
                        //Doc.Load(ms);
                        //Doc.Save("D:/MVCTest.xml");
                        //ENDE TESTDATEI 
                        System.IO.TextReader tr = new System.IO.StringReader(Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Position));
                        xTrf.Load(XSLTransform);
                        xTrf.Transform(new System.Xml.XmlTextReader(tr), null, writeStream);
                    }
                }
                else
                {
                    var xSer = new XmlSerializer(dataType);

                    if ((XSLTransform == null) || (XSLTransform.Length == 0))
                        xSer.Serialize(writeStream, value);
                    else
                    {
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        var xTrf = new System.Xml.Xsl.XslCompiledTransform();

                        xSer.Serialize(ms, value);
                        //TESTDATEI(EZEUGEN)
                        //System.Xml.XmlDocument Doc = new System.Xml.XmlDocument();
                        //ms.Seek(0, System.IO.SeekOrigin.Begin);
                        //Doc.Load(ms);
                        //Doc.Save("D:/MVCTest.xml");
                        //ENDE TESTDATEI 
                        System.IO.TextReader tr = new System.IO.StringReader(Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Position));
                        xTrf.Load(XSLTransform);
                        xTrf.Transform(new System.Xml.XPath.XPathDocument(tr), null, writeStream);
                    }
                }
            }
        }
    }
}