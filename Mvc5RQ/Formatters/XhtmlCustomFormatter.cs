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
    public class XhtmlCustomFormatter : MediaTypeFormatter
    {
        #region private members

        private readonly string rqihtml = "application/x-riquest-internal";

        private bool _isAsync = false;

        Func<Type, bool> SupportedType = (type) =>
        {
            if (type == typeof(Mvc5RQ.Models.RQItemModel) || type == typeof(Mvc5RQ.Models.RQItem))
                return true;
            else
                return false;
        };

        #endregion

        #region private methods

        private Task GetWriteTask(Stream writeStream, Mvc5RQ.Models.RQItemModel value)
        {
            return new Task(() =>
                WriteXhtmlList(value, writeStream));
        }

        private Task GetWriteTask(Stream writeStream, Mvc5RQ.Models.RQItem value)
        {
            return new Task(() =>
                WriteXhtmlItem(value, writeStream));
        }

        private void WriteXhtmlList(Mvc5RQ.Models.RQItemModel rqItemModel, Stream writeStream)
        {
            if (rqItemModel != null)
            {
                System.Xml.XmlTextReader r = rqItemModel.RQItems.ConvertTo("rqi", 1, 0);

                try
                {
                    var xTrf = new System.Xml.Xsl.XslCompiledTransform(true);
                    var xSet = new System.Xml.Xsl.XsltSettings(enableDocumentFunction:true, enableScript:true);

                    r.MoveToContent();
                    xTrf.Load(rqItemModel.RQItems.FormatPreprocessor.XmlTransformPath, xSet, new System.Xml.XmlUrlResolver());
                    xTrf.Transform(new System.Xml.XPath.XPathDocument(r), rqItemModel.RQItems.FormatPreprocessor.XslTransformArg, writeStream);
                }
                catch
                {
                    throw new NotImplementedException("Could not find a RiQuest item with requested document number.");
                }
            }
        }

        private void WriteXhtmlItem(Mvc5RQ.Models.RQItem rqItem, Stream writeStream)
        {
            if (rqItem != null)
            {
                var dSer = new DataContractSerializer(typeof(Mvc5RQ.Models.RQItem));

                try
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    var xTrf = new System.Xml.Xsl.XslCompiledTransform();
                    var xSet = new System.Xml.Xsl.XsltSettings(enableDocumentFunction:true, enableScript:true);

                    dSer.WriteObject(ms, rqItem);
                    System.IO.TextReader tr = new System.IO.StringReader(System.Text.Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Position));
                    xTrf.Load(rqItem.FormatPreprocessor.XmlTransformPath, xSet, new System.Xml.XmlUrlResolver());
                    xTrf.Transform(new System.Xml.XPath.XPathDocument(tr), rqItem.FormatPreprocessor.XslTransformArg, writeStream);
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

        #endregion

        #region public constructors

        /// <summary>
        /// 
        /// </summary>
        public XhtmlCustomFormatter()
            : this(false)
        { }

        /// <summary>
        /// 
        /// </summary>
        public XhtmlCustomFormatter(bool isAsync)
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(rqihtml));
            IsAsync = isAsync;
            SupportedEncodings.Add(Encoding.UTF8); // new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }

        #endregion

        #region overrides

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

            if (type == typeof(Mvc5RQ.Models.RQItemModel))
                writeTask = GetWriteTask(writeStream, (Mvc5RQ.Models.RQItemModel)value);
            else
                writeTask = GetWriteTask(writeStream, (Mvc5RQ.Models.RQItem)value);
            if (_isAsync)
                writeTask.Start();
            else
                writeTask.RunSynchronously();
            return writeTask;
        }

        #endregion
    }
}