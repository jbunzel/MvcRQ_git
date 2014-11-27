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
    /// <summary>
    /// 
    /// </summary>
    public class RDFCustomFormatter : XmlMediaTypeFormatter
    {
        #region private members

        private bool _isAsync = false;

        Func<Type, bool> SupportedType = (type) =>
        {
            if (type == typeof(Mvc5RQ.Models.RQKosBranch))
                return true;
            else
                return false;
        };

        #endregion

        #region private methods

        private Task GetWriteTask(Stream writeStream, Mvc5RQ.Models.RQKosBranch value)
        {
            return new Task(() =>
                WriteRQKos(value, writeStream));
        }

        private void WriteRQKos(Mvc5RQ.Models.RQKosBranch rqKos, Stream writeStream)
        {
            if (rqKos != null)
            {
                var dSer = new XmlSerializer(typeof(Mvc5RQ.Models.RQKosBranch));

                try
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    var xTrf = new System.Xml.Xsl.XslCompiledTransform();
                    var xSet = new System.Xml.Xsl.XsltSettings(enableDocumentFunction: true, enableScript: true);

                    dSer.Serialize(ms, rqKos);
                    //dSer.WriteObject(ms, rqKos);
                    //TESTDATEI(EZEUGEN)
                    //System.Xml.XmlDocument Doc = new System.Xml.XmlDocument();
                    //ms.Seek(0, System.IO.SeekOrigin.Begin);
                    //Doc.Load(ms);
                    //Doc.Save("D:/MVCTest.xml");
                    //ENDE TESTDATEI 
                    System.IO.TextReader tr = new System.IO.StringReader(System.Text.Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Position));
                    xTrf.Load(rqKos.FormatPreprocessor.XmlTransformPath, xSet, new System.Xml.XmlUrlResolver());
                    xTrf.Transform(new System.Xml.XPath.XPathDocument(tr), rqKos.FormatPreprocessor.XslTransformArg, writeStream);
                }
                catch
                {
                    throw new NotImplementedException("Could not find a RiQuest item with requested document number.");
                }
            }
        }

        #endregion

        #region public properties

        /// <summary>
        /// 
        /// </summary>
        public bool IsAsync
        {
            get { return _isAsync; }
            set { _isAsync = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string XSLTransform { get; set; }

        #endregion

        #region public constructors

        /// <summary>
        /// 
        /// </summary>
        public RDFCustomFormatter()
            : this(false)
        { }

        /// <summary>
        /// 
        /// </summary>
        public RDFCustomFormatter(bool isAsync)
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/rdf+xml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/rdf+xml"));
            IsAsync = isAsync;
            SupportedEncodings.Add(Encoding.UTF8); // new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }

        #endregion

        #region public overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool CanReadType(Type type)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool CanWriteType(Type type)
        {
            return SupportedType(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="writeStream"></param>
        /// <param name="content"></param>
        /// <param name="transportContext"></param>
        /// <returns></returns>
        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            Task writeTask;

            writeTask = GetWriteTask(writeStream, (Mvc5RQ.Models.RQKosBranch)value);
            if (_isAsync)
                writeTask.Start();
            else
                writeTask.RunSynchronously();
            return writeTask;
        }

        #endregion
    }
}