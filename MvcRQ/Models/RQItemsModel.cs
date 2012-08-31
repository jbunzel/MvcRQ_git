using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using RQLib.RQQueryResult;
using RQLib.RQQueryResult.RQDescriptionElements;
using RQLib.RQQueryForm;

namespace MvcRQ.Models
{
    [DataContract()]
    public class RQItemModel
    {
        #region public properties

        [DataMember()]
        public RQItemSet RQItems { get; set; }

        #endregion

        #region public constructors
        
        public RQItemModel(RQquery query, bool forEdit)
        {
            RQItems = new RQItemSet(forEdit);
            query.QueryFieldList = RQItems.GetDataFieldTable();
            RQItems.Find(query);
        }

        public RQItemModel(RQquery query)
        {
            RQItems = new RQItemSet();
            query.QueryFieldList = RQItems.GetDataFieldTable();
            RQItems.Find(query);
        }

        public RQItemModel()
        { }

        #endregion

        #region public methods

        public RQItem Add(RQItem newItem)
        {
            return this.RQItems.Add(newItem);
        }

        public void Update()
        {
            try
            {
                this.RQItems.Update();
                //TODO: RQItems.Update() does not care for update of table "Systematik" (former updateClassRelations)
	        }
	        catch (Exception)
	        {
		        throw;
	        }
        }

        #endregion
    }

    [DataContract()]
    [XmlRoot]
    public class RQItemSet : System.Collections.Generic.IEnumerable<RQItem>
    {
        #region private members

        private RQResultSet ItemResultSet;

        #endregion

        #region public properties

        public int count
        {
            get
            {
                return ItemResultSet.count;
            }
        }

        [DataMember]
        [XmlElement]
        public List<RQItem> RQItems
        {
            get
            {
                List<RQItem> li = new List<RQItem>(count);

                for (int i = 0; i < count; i++ )
                    li.Add(this.GetItem(i));
                return li;
            }

            set
            {
            }
        }

        #endregion

        #region public constructors

        public RQItemSet(bool forEdit)
        {
            ItemResultSet = new RQResultSet(forEdit);
        }
        
        public RQItemSet() :base()
        {
            ItemResultSet = new RQResultSet();
        }

        #endregion

        #region public methods

        public RQItem Add(RQItem item)
        {
            return new RQItem(this.ItemResultSet.CreateItem(item._resultItem));
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

        #endregion
    }

    public class RQItemSetEnum : IEnumerator<RQItem>
    {
        #region private members

        private RQItemSet _itemSet;
        private int _curIndex;
        private RQItem _curItem;

        #endregion

        #region public constructors

        public RQItemSetEnum( RQItemSet itemSet)
        {
            _itemSet = itemSet;
            _curIndex = -1;
            _curItem = default(RQItem);
        }

        #endregion

        #region public methods

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

        #endregion
    }

    [DataContract()]
    [XmlRoot]
    public class RQItem
    {
        #region internal members

        internal RQResultItem _resultItem {get; set; }
        
        #endregion

        #region public properties

        [DataMember]
        [XmlElement]
        public string ID
        {
            get
            {
                return this._resultItem.ItemDescription.ID;
            }
            set
            {
                this._resultItem.ItemDescription.ID = value;
            }
        }

        [DataMember]
        [XmlElement]
        public string DocNo
        {
            get
            {
                return this._resultItem.ItemDescription.DocNo;
            }
            set
            {
                this._resultItem.ItemDescription.DocNo = value;
            }
        }

        [DataMember]
        [DataType(DataType.MultilineText)]
        [XmlElement]
        public string Title
        {
            get
            {
                return this._resultItem.ItemDescription.Title;
            }
            set
            {
                this._resultItem.ItemDescription.Title = value;
            }
        }

        [DataMember]
        [DataType(DataType.MultilineText)]
        [XmlElement]
        public string Authors
        {
            get
            {
                return this._resultItem.ItemDescription.Authors;
            }
            set
            {
                this._resultItem.ItemDescription.Authors = value;
            }
        }

        [DataMember]
        [DataType(DataType.MultilineText)]
        [XmlElement]
        public string Source
        {
            get
            {
                return this._resultItem.ItemDescription.Source;
            }
            set
            {
                this._resultItem.ItemDescription.Source = value;
            }
        }

        [DataMember]
        [DataType(DataType.MultilineText)]
        [XmlElement]
        public string Institutions
        {
            get
            {
                return this._resultItem.ItemDescription.Institutions;
            }
            set
            {
                this._resultItem.ItemDescription.Institutions = value;
            }
        }

        [DataMember]
        [DataType(DataType.MultilineText)]
        [XmlElement]
        public string Series
        {
            get
            {
                return this._resultItem.ItemDescription.Series;
            }
            set
            {
                this._resultItem.ItemDescription.Series = value;
            }
        }

        [DataMember]
        [DataType(DataType.MultilineText)]
        [XmlElement]
        public string IndexTerms
        {
            get
            {
                return this._resultItem.ItemDescription.IndexTerms;
            }
            set
            {
                this._resultItem.ItemDescription.IndexTerms = value;
            }
        }

        [DataMember]
        [DataType(DataType.MultilineText)]
        [XmlElement]
        public string Subjects
        {
            get
            {
                return this._resultItem.ItemDescription.Subjects;
            }
            set
            {
                this._resultItem.ItemDescription.Subjects = value;
            }
        }

        [DataMember]
        [DataType(DataType.MultilineText)]
        [XmlElement]
        public string AboutPersons
        {
            get
            {
                return this._resultItem.ItemDescription.AboutPersons;
            }
            set
            {
                this._resultItem.ItemDescription.AboutPersons = value;
            }
        }

        [DataMember]
        [DataType(DataType.MultilineText)]
        [XmlElement]
        public string Abstract
        {
            get
            {
                return this._resultItem.ItemDescription.Abstract;
            }
            set
            {
                this._resultItem.ItemDescription.Abstract = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string Edition
        {
            get
            {
                return this._resultItem.ItemDescription.Edition;
            }
            set
            {
                this._resultItem.ItemDescription.Edition = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string ISDN
        {
            get
            {
                return this._resultItem.ItemDescription.ISDN;
            }
            set
            {
                this._resultItem.ItemDescription.ISDN = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string Coden
        {
            get
            {
                return this._resultItem.ItemDescription.Coden;
            }
            set
            {
                this._resultItem.ItemDescription.Coden = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string Locality
        {
            get
            {
                return this._resultItem.ItemDescription.Locality;
            }
            set
            {
                this._resultItem.ItemDescription.Locality = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string Publisher
        {
            get
            {
                return this._resultItem.ItemDescription.Publisher;
            }
            set
            {
                this._resultItem.ItemDescription.Publisher = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string PublTime
        {
            get
            {
                return this._resultItem.ItemDescription.PublTime;
            }
            set
            {
                this._resultItem.ItemDescription.PublTime = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string Volume
        {
            get
            {
                return this._resultItem.ItemDescription.Volume;
            }
            set
            {
                this._resultItem.ItemDescription.Volume = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string Issue
        {
            get
            {
                return this._resultItem.ItemDescription.Issue;
            }
            set
            {
                this._resultItem.ItemDescription.Issue = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string Pages
        {
            get
            {
                return this._resultItem.ItemDescription.Pages;
            }
            set
            {
                this._resultItem.ItemDescription.Pages = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string Language
        {
            get
            {
                return this._resultItem.ItemDescription.Language;
            }
            set
            {
                this._resultItem.ItemDescription.Language = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string Signature
        {
            get
            {
                return this._resultItem.ItemDescription.Signature;
            }
            set
            {
                this._resultItem.ItemDescription.Signature = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string DocTypeCode
        {
            get
            {
                return this._resultItem.ItemDescription.DocTypeCode;
            }
            set
            {
                this._resultItem.ItemDescription.DocTypeCode = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string DocTypeName
        {
            get
            {
                return this._resultItem.ItemDescription.DocTypeName;
            }
            set
            {
                this._resultItem.ItemDescription.DocTypeName = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string WorkType
        {
            get
            {
                return this._resultItem.ItemDescription.WorkType;
            }
            set
            {
                this._resultItem.ItemDescription.WorkType = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string AboutLocation
        {
            get
            {
                return this._resultItem.ItemDescription.AboutLocation;
            }
            set
            {
                this._resultItem.ItemDescription.AboutLocation = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string AboutTime
        {
            get
            {
                return this._resultItem.ItemDescription.AboutTime;
            }
            set
            {
                this._resultItem.ItemDescription.AboutTime = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string CreateLocation
        {
            get
            {
                return this._resultItem.ItemDescription.CreateLocation;
            }
            set
            {
                this._resultItem.ItemDescription.CreateLocation = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        [XmlElement]
        public string CreateTime
        {
            get
            {
                return this._resultItem.ItemDescription.CreateTime;
            }
            set
            {
                this._resultItem.ItemDescription.CreateTime = value;
            }
        }

        [DataMember()]
        [DataType(DataType.Text)]
        [XmlElement]
        public string Notes
        {
            get
            {
                return this._resultItem.ItemDescription.Notes;
            }
            set
            {
                this._resultItem.ItemDescription.Notes = value;
            }
        }

        [DataMember]
        [XmlElement]
        public RQClassification Classification
        {
            get
            {
                return this._resultItem.ItemDescription.Classification;
            }
            set
            {
                this._resultItem.ItemDescription.Classification = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        public string ClassificationFieldContent
        {
            get
            {
                return this._resultItem.ItemDescription.Classification != null ? this._resultItem.ItemDescription.Classification.Content : "";
            }
            set
            {
                this._resultItem.ItemDescription.Classification = new RQClassification(value);
            }
        }

        #endregion

        #region public constructors

        public RQItem()
        {
            this._resultItem = new RQResultItem();
        }

        public RQItem(RQResultItem resultItem)
        {
            this._resultItem = resultItem;
        }

        #endregion

        #region public methods

        public void Change(System.Collections.Specialized.NameValueCollection fromFields)
        {
            this._resultItem.Change(fromFields);
        }

        public void Change(RQItem fromItem)
        {
            this._resultItem.Change(fromItem._resultItem);
        }

        public XmlTextReader ConvertTo(string format)
        {
           return this._resultItem.ConvertTo(format);
        }

        public RQDescriptionElement GetDescriptionElement(string fieldName)
        {
            System.Reflection.PropertyInfo info = this._resultItem.ItemDescription.GetType().GetProperty(fieldName);
            object o = info.GetValue(this._resultItem.ItemDescription, null);

            if (o.GetType().IsSubclassOf(typeof(RQDescriptionElement)))
                return (RQDescriptionElement) o;
            else
                return null;
        }

        public Object GetField(string fieldName, int subFieldIndex)
        {
            RQDescriptionElement de = this.GetDescriptionElement(fieldName);

            if (de != null)
            {
                switch (de.GetType().Name)
                {
                    case "RQClassification":
                        RQClassification cl = (RQClassification)de;

                        if (cl.items[subFieldIndex] != null)
                        {
                            RQLib.RQKos.Classifications.SubjClass sc = cl.items[subFieldIndex];

                            if (!cl.IsLinkedDataEnabled)
                            {
                                sc.EnableLinkedData();
                                sc.Load();
                                sc.DisableLinkedData();
                            }
                            return sc;
                        }
                        else
                            return cl.items[cl.count - 1];
                    default:
                        return de;
                }
            }
            return null;
        }

        #endregion
    }
}