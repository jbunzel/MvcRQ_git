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

        public void Update()
        {
            this.RQItems.Update();
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

        public void Update()
        {
            this.ItemResultSet.Update();
        }

        public void Find(RQquery query)
        {
            this.ItemResultSet.Find(query);
        }

        public System.Xml.XmlTextReader ConvertTo(string format, int fromRecord, int maxRecord)
        {
            if (format == "rqListHTML" || format == "")
                return this.ItemResultSet.ConvertTo("rq", fromRecord, maxRecord);
            else
                return this.ItemResultSet.ConvertTo(format, fromRecord, maxRecord);
        }

        public System.Xml.XmlTextReader ConvertTo(string format)
        {
            return this.ConvertTo(format, 1, 0);
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
        [DataType(DataType.MultilineText)]
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
        [DataType(DataType.MultilineText)]
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
        [DataType(DataType.MultilineText)]
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
        [DataType(DataType.MultilineText)]
        public string Institutions
        {
            get
            {
                return this._resultItem.ItemDescription.Institutions;
            }
            set
            {
                this._resultItem.ItemDescription.Title = Institutions;
            }
        }

        [DataMember]
        [DataType(DataType.MultilineText)]
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
        [DataType(DataType.MultilineText)]
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
        [DataType(DataType.MultilineText)]
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
        [DataType(DataType.MultilineText)]
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
        [DataType(DataType.MultilineText)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
        public RQLib.RQQueryResult.RQDescriptionElements.RQClassification Classification
        {
            get
            {
                return this._resultItem.ItemDescription.Classification;
            }
            set
            {
                this._resultItem.ItemDescription.Classification = Classification;
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

        public void Change(System.Collections.Specialized.NameValueCollection fromFields)
        {
            this._resultItem.Change(fromFields);
        }

        public XmlTextReader ConvertTo(string format)
        {
           return this._resultItem.ConvertTo(format);
        }
    }
}