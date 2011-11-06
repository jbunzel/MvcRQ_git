using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using RQLib.RQQueryResult;
using RQLib.RQQueryForm;

namespace MvcRQ.Models
{
    public class RQItemModel
    {
        public RQItemSet RQItems { get; set; }

        public RQItemModel(RQquery query)
        {
            RQItems = new RQItemSet();
            query.QueryFieldList = RQItems.GetDataFieldTable();
            RQItems.Find(query);
        }

        public string toHTML(string format, int fromRecord, int toRecord)
        {
            return this.RQItems.toHTML(format, fromRecord, toRecord);
        }

        public string toHTML(string format, int fromRecord)
        {
            return this.toHTML(format, fromRecord, 0);
        }

        public string toHTML(string format)
        {
            return this.toHTML(format, 1, 0);
        }
    }

    [XmlRoot]
    public class RQItemSet : System.Collections.Generic.IEnumerable<RQItem>
    {
        private RQResultSet ItemResultSet;

        public int count
        {
            get
            {
                return ItemResultSet.count;
            }
        }

        public RQItemSet() :base()
        {
            ItemResultSet = new RQResultSet();
        }

        public void Add(RQItem item)
        {
            this.ItemResultSet.CreateItem(item._resultItem);
        }

        public RQItem GetItem(int i)
        {
            return new RQItem(ItemResultSet.GetItem(i));
        }

        public void Find(RQquery query)
        {
            this.ItemResultSet.Find(query);
        }

        public System.Xml.XmlTextReader ConvertTo(string format, int fromRecord, int toRecord)
        {
            switch (format)
            {
                case "rqListHTML":
                    return this.ItemResultSet.ConvertTo(RQLib.Globals.BibliographicFormats.RQintern);
                default:
                    return null;
            }
        }

        public string toHTML (string format, int fromRecord, int toRecord)
        {
            System.Xml.XmlTextReader r = this.ConvertTo(format, fromRecord, toRecord);

            try{
                var xTrf = new System.Xml.Xsl.XslCompiledTransform();
                var mstr = new XmlTextWriter(new System.IO.MemoryStream(), System.Text.Encoding.UTF8);
                XmlDocument doc = new XmlDocument();

                r.MoveToContent();
                xTrf.Load(HttpContext.Current.Server.MapPath("xslt/ViewTransforms/RQResultList2RQSorted.xslt"));
                xTrf.Transform(new System.Xml.XPath.XPathDocument(r), null, mstr);
                mstr.BaseStream.Flush();
                mstr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                doc.Load(mstr.BaseStream);

                //TESTDATEI EZEUGEN
                //doc.Save("C:/MVCTest.xml");
                //mstr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);

                XmlTextReader rd =new XmlTextReader(mstr.BaseStream);
                return doc.OuterXml;
            }
            catch
            {
                // RQItemSet ist leer
                return "";
            }
            //return r.ReadOuterXml();
        }
 
        public System.Data.DataTable GetDataFieldTable()
        {
            return this.ItemResultSet.GetDataFieldTable();
        }

        public System.Collections.Generic.IEnumerator<RQItem> GetEnumerator()
        {
            return new RQItemSetEnum(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new RQItemSetEnum(this);
        }
    }

    public class RQItemSetEnum : IEnumerator<RQItem>
    {
        private RQItemSet _itemSet;
        private int _curIndex;
        private RQItem _curItem;
 
        public RQItemSetEnum( RQItemSet itemSet)
        {
            _itemSet = itemSet;
            _curIndex = -1;
            _curItem = default(RQItem);
        }

        public bool MoveNext()
        {
            if (++_curIndex >= _itemSet.count)
            {
                return false;
            }
            else
            {
                _curItem = _itemSet.GetItem(_curIndex);
            }
            return true;
        }

        public RQItem Current
        {
            get
            {
                return this._curItem;
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return this._curItem;
            }
        }

        public void Reset()
        {
        }
        
        void IDisposable.Dispose()
        {
        }

    }

    [XmlRoot]
    public class RQItem
    {
        internal RQResultItem _resultItem {get; set; }

        [DataMember]
        public string ID
        {
            get
            {
                return this._resultItem.ItemDescription.ID;
            }
            set
            {
                this._resultItem.ItemDescription.Title = ID;
            }
        }

        [DataMember]
        public string DocNo
        {
            get
            {
                return this._resultItem.ItemDescription.DocNo;
            }
            set
            {
                this._resultItem.ItemDescription.Title = DocNo;
            }
        }

        [DataMember]
        public string Title
        {
            get
            {
                return this._resultItem.ItemDescription.Title;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Title;
            }
        }

        [DataMember]
        public string Authors
        {
            get
            {
                return this._resultItem.ItemDescription.Authors;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Authors;
            }
        }

        [DataMember]
        public string Source
        {
            get
            {
                return this._resultItem.ItemDescription.Source;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Source;
            }
        }

        [DataMember]
        public string Institutions
        {
            get
            {
                return this._resultItem.ItemDescription.Series;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Series;
            }
        }

        [DataMember]
        public string Series
        {
            get
            {
                return this._resultItem.ItemDescription.Series;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Series;
            }
        }

        [DataMember]
        public string IndexTerms
        {
            get
            {
                return this._resultItem.ItemDescription.IndexTerms;
            }
            set
            {
                this._resultItem.ItemDescription.Title = IndexTerms;
            }
        }

        [DataMember]
        public string Subjects
        {
            get
            {
                return this._resultItem.ItemDescription.Subjects;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Subjects;
            }
        }

        [DataMember]
        public string AboutPersons
        {
            get
            {
                return this._resultItem.ItemDescription.AboutPersons;
            }
            set
            {
                this._resultItem.ItemDescription.Title = AboutPersons;
            }
        }

        [DataMember]
        public string Abstract
        {
            get
            {
                return this._resultItem.ItemDescription.Abstract;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Abstract;
            }
        }

        [DataMember]
        public string Edition
        {
            get
            {
                return this._resultItem.ItemDescription.Edition;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Edition;
            }
        }

        [DataMember]
        public string ISDN
        {
            get
            {
                return this._resultItem.ItemDescription.ISDN;
            }
            set
            {
                this._resultItem.ItemDescription.Title = ISDN;
            }
        }

        [DataMember]
        public string Coden
        {
            get
            {
                return this._resultItem.ItemDescription.Coden;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Coden;
            }
        }

        [DataMember]
        public string Locality
        {
            get
            {
                return this._resultItem.ItemDescription.Locality;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Locality;
            }
        }

        [DataMember]
        public string Publisher
        {
            get
            {
                return this._resultItem.ItemDescription.Publisher;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Publisher;
            }
        }

        [DataMember]
        public string PublTime
        {
            get
            {
                return this._resultItem.ItemDescription.PublTime;
            }
            set
            {
                this._resultItem.ItemDescription.Title = PublTime;
            }
        }

        [DataMember]
        public string Volume
        {
            get
            {
                return this._resultItem.ItemDescription.Volume;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Volume;
            }
        }

        [DataMember]
        public string Issue
        {
            get
            {
                return this._resultItem.ItemDescription.Issue;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Issue;
            }
        }

        [DataMember]
        public string Pages
        {
            get
            {
                return this._resultItem.ItemDescription.Pages;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Pages;
            }
        }

        [DataMember]
        public string Language
        {
            get
            {
                return this._resultItem.ItemDescription.Language;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Language;
            }
        }

        [DataMember]
        public string Signature
        {
            get
            {
                return this._resultItem.ItemDescription.Signature;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Signature;
            }
        }

        [DataMember]
        public string DocTypeCode
        {
            get
            {
                return this._resultItem.ItemDescription.DocTypeCode;
            }
            set
            {
                this._resultItem.ItemDescription.Title = DocTypeCode;
            }
        }

        [DataMember]
        public string DocTypeName
        {
            get
            {
                return this._resultItem.ItemDescription.DocTypeName;
            }
            set
            {
                this._resultItem.ItemDescription.Title = DocTypeName;
            }
        }

        [DataMember]
        public string WorkType
        {
            get
            {
                return this._resultItem.ItemDescription.WorkType;
            }
            set
            {
                this._resultItem.ItemDescription.Title = WorkType;
            }
        }

        [DataMember]
        public string AboutLocation
        {
            get
            {
                return this._resultItem.ItemDescription.AboutLocation;
            }
            set
            {
                this._resultItem.ItemDescription.Title = AboutLocation;
            }
        }

        [DataMember]
        public string AboutTime
        {
            get
            {
                return this._resultItem.ItemDescription.AboutTime;
            }
            set
            {
                this._resultItem.ItemDescription.Title = AboutTime;
            }
        }

        [DataMember]
        public string CreateLocation
        {
            get
            {
                return this._resultItem.ItemDescription.CreateLocation;
            }
            set
            {
                this._resultItem.ItemDescription.Title = CreateLocation;
            }
        }

        [DataMember]
        public string CreateTime
        {
            get
            {
                return this._resultItem.ItemDescription.CreateTime;
            }
            set
            {
                this._resultItem.ItemDescription.Title = CreateTime;
            }
        }

        [DataMember]
        public string Notes
        {
            get
            {
                return this._resultItem.ItemDescription.Notes;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Notes;
            }
        }

        [DataMember]
        public string Classification
        {
            get
            {
                return this._resultItem.ItemDescription.Classification;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Classification;
            }
        }
              
        public RQItem()
        {
            this._resultItem = new RQResultItem();
        }

        public RQItem(RQResultItem resultItem)
        {
            this._resultItem = resultItem;
        }

        public XmlTextReader ConvertTo(string format)
        {
            return null; // this._resultItem.ConvertTo(format);
        }

    }

    //[DataContract]
    //public class ItemDescElement
    //{
    //    [Required]
    //    [DataMember]
    //    public string ID { get; set; }
        
    //    [StringLength(255), Required]
    //    [DataMember]
    //    public string Name { get; set; }
        
    //    [DataMember]
    //    public int Value1 { get; set; }
        
    //    [DataMember]
    //    public double Value2 { get; set; }
    //}
}