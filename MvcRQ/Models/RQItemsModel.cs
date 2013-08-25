using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using RQLib.RQQueryResult;
using RQLib.RQQueryResult.RQDescriptionElements;
using RQLib.RQQueryForm;

using MvcRQ.Helpers;
using MvcRQ.Areas.DigitalObjects.Helpers;


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

        public bool IsEditable()
        {
            return this.RQItems.IsEditable();
        }

        public RQItem Add(RQItem newItem)
        {
            RQItem theItem = this.RQItems.Add(newItem);

            try
            {
                if (newItem.ClassificationFieldContent != "")
                    theItem.SetSavedFieldValue("Classification", "$$EMPTY$$");
                if (newItem.Abstract.Substring(newItem.Abstract.IndexOf("$$TOC$$=")) != "")
                    theItem.SetSavedFieldValue("TOC", "$$TOC$$=$$EMPTY$$");
            }
            catch { }
            return theItem;
        }

        public void Update()
        {
            try
            {
                this.RQItems.Update();
	        }
	        catch (Exception ex)
	        {
                throw new Exception(ex.Message, ex.InnerException);
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
        private List<RQItem> _rqitemsList = null;

        #endregion

        #region private methods

        private void BuildRQItemList()
        {
            this._rqitemsList = null;
            if (count > 0)
            {
                List<RQItem> li = new List<RQItem>(count);

                for (int i = 0; i < count; i++)
                {
                    RQItem rqi = this.GetItem(i);

                    if (MvcRQ.Helpers.AccessRightsResolver.HasViewAccess(rqi.AccessRights))
                        li.Add(rqi);
                }
                this._rqitemsList = li;
            }
        }
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
                if (this._rqitemsList == null)
                    this.BuildRQItemList();
                return this._rqitemsList;

                // List<RQItem> li = new List<RQItem>(count);

                // for (int i = 0; i < count; i++)
                // {
                //     RQItem rqi = this.GetItem(i);

                //     if (MvcRQ.Helpers.AccessRightsResolver.HasViewAccess(rqi.AccessRights))
                //         li.Add(rqi);
                // }
                //return li;
            }
            set
            {
            }
        }

        //[DataMember]
        //[DataType(DataType.Text)]
        //public string AccessRights
        //{
        //    get
        //    {
        //        return "TEST"; //MvcRQ.Helpers.AccessRightsResolver.ResolveItemAccessRights(this._resultItem.ItemDescription.Feld32);
        //    }
        //    set
        //    {
        //        this._resultItem.ItemDescription.Feld32 = value;
        //    }
        //}

        #endregion

        #region public constructors

        public RQItemSet(bool forEdit)
        {
            ItemResultSet = new RQResultSet(forEdit);
        }

        public RQItemSet()
            : base()
        {
            ItemResultSet = new RQResultSet();
        }

        #endregion

        #region public methods

        public bool IsEditable()
        {
            return this.ItemResultSet.IsEditable();
        }

        public RQItem Add(RQItem item)
        {
            return new RQItem(this.ItemResultSet.CreateItem(item._resultItem));
        }

        public RQItem GetItem(int i)
        {
            if (this._rqitemsList == null)
            {
                RQResultItem t1 = ItemResultSet.GetItem(i);
                RQItem t2 = new RQItem(ItemResultSet.GetItem(i));

                return t2;
            }
            else
                return this.RQItems.ElementAt(i);
            //return new RQItem(ItemResultSet.GetItem(i));
        }

        public void Update()
        {
            this.ItemResultSet.Update();
            if (this.UpdateDigitalObjectToC())
                this.ItemResultSet.Update();
            this.UpdateClassRelation();
        }

        public void UpdateClassRelation()
        {
            foreach (RQItem theItem in this.RQItems)
                theItem.UpdateClassRelation(this.ItemResultSet);
        }

        public bool UpdateDigitalObjectToC()
        {
            bool result = false;

            foreach (RQResultItem theItem in this.ItemResultSet)
            {
                var str1 = theItem.ItemDescription.Abstract;
                var str2 = theItem.ItemDescription.Signature;
                
                if (result = (result || DigitalObjectHelpers.UpdateDigitalObjectDirectory(theItem.ItemDescription.DocNo, ref str1, ref str2, theItem.Write())))
                {
                    theItem.ItemDescription.Abstract = str1;
                    theItem.ItemDescription.Signature = str2;
                }
            }
            return result;
        }

        public void Find(RQquery query)
        {
            try
            {
                this.ItemResultSet.Find(query);
                this.BuildRQItemList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        public RQItemSetEnum(RQItemSet itemSet)
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

        internal RQResultItem _resultItem { get; set; }
        internal StringDictionary _savedFieldValues = new StringDictionary();

        #endregion

        #region public members

        public enum DisplFormat
        {
            single_item,
            short_title
        }

        public enum DataFormat
        {
            intern,
            dc
        }

        #endregion

        #region private methods

        private RQDescriptionElement GetDescriptionElement(string fieldName)
        {
            System.Reflection.PropertyInfo info = this._resultItem.ItemDescription.GetType().GetProperty(fieldName);
            object o = info.GetValue(this._resultItem.ItemDescription, null);

            if (o.GetType().IsSubclassOf(typeof(RQDescriptionElement)))
                return (RQDescriptionElement)o;
            else
                return null;
        }

        private Object LoadLinkedData(RQDescriptionElement descriptionElement, int subFieldIndex)
        {
            if (descriptionElement != null)
            {
                switch (descriptionElement.GetType().Name)
                {
                    case "RQClassification":
                        RQClassification cl = (RQClassification)descriptionElement;

                        if (cl.items[subFieldIndex] != null)
                        {
                            RQLib.RQKos.Classifications.SubjClass sc = (RQLib.RQKos.Classifications.SubjClass)cl.items[subFieldIndex];

                            if (!cl.IsLinkedDataEnabled && !sc.IsComplete)
                            {
                                sc.EnableLinkedData();
                                sc.Load();
                                sc.DisableLinkedData();
                            }
                            return sc;
                        }
                        else
                            return null;
                    case "RQAuthors":
                        RQAuthors au = (RQAuthors)descriptionElement;

                        if (au.items[subFieldIndex] != null)
                        {
                            RQLib.RQKos.Persons.Person per = (RQLib.RQKos.Persons.Person)au.items[subFieldIndex];

                            if (!au.IsLinkedDataEnabled && !per.IsComplete)
                            {
                                per.EnableLinkedData();
                                per.Load();
                                per.DisableLinkedData();
                            }
                            return per;
                        }
                        else
                            return au.items[au.count - 1];

                    default:
                        return null;
                }
            }
            return null;
        }

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

        //[DataMember]
        //[DataType(DataType.MultilineText)]
        //[XmlElement]
        //public string Authors
        //{
        //    get
        //    {
        //        return this._resultItem.ItemDescription.Authors;
        //    }
        //    set
        //    {
        //        this._resultItem.ItemDescription.Authors = value;
        //    }
        //}

        [DataMember]
        [DataType(DataType.MultilineText)]
        [XmlElement]
        public string Authors
        {
            get
            {
                return this._resultItem.ItemDescription.Authors != null ? this._resultItem.ItemDescription.Authors.Content : "";
            }
            set
            {
                this._resultItem.ItemDescription.Authors = new RQAuthors(value);
            }
        }

        [DataMember]
        [XmlElement]
        public RQAuthors AuthorsEntity
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
                this._resultItem.ItemDescription.Classification = new RQClassification();
                this._resultItem.ItemDescription.Classification.Content = value;
            }
        }

        [DataMember]
        [DataType(DataType.Text)]
        public string AccessRights
        {
            get
            {
                String t = this._resultItem.ItemDescription.Feld32;

                if (t != null) t = t.Trim();
                if (String.IsNullOrEmpty(t))
                {
                    this._resultItem.ItemDescription.Feld32 = AccessRightsResolver.EncodeAccessRights(this._resultItem.IsExternalItem() ? "EXTERNAL" : "");
                    t = this._resultItem.ItemDescription.Feld32;
                }
                return AccessRightsResolver.ResolveItemAccessRights(AccessRightsResolver.DecodeAccessRights(t));
            }
            set
            {
                this._resultItem.ItemDescription.Feld32 = AccessRightsResolver.EncodeAccessRights(value);
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

        public RQItem(string itemId)
        {
            //Die Suchfunktionen sind sehr umständlich. Wünschenswert wäre: viewItem = new RQResultSet().Find("$access$"+rqitemID).GetItem(0);
            RQResultSet rs = new RQResultSet(true); //BUG: forEdit = true, da da die Lucene-Datenbank bei Suche nach DocNr keine Treffer liefert.
            RQquery qr = new RQquery("$access$" + itemId, "form", new System.Data.DataTable());

            qr.SetDefaultQueryFields();
            rs.Find(qr);
            this._resultItem = rs.GetItem(0);
        }

        #endregion

        #region public methods

        public void Change(NameValueCollection fromFields)
        {
            this._resultItem.Change(fromFields);
        }

        public void Change(RQItem fromItem)
        {
            string fromTocStr = "";
            string toCStr = "";

            if (fromItem.ClassificationFieldContent != this.ClassificationFieldContent)
                this._savedFieldValues.Add("Classification", this.ClassificationFieldContent);
            if ((fromTocStr = fromItem.GetToC()) != "") {
                // if fromItem has ToC
                if (((toCStr = this.GetToC()) != "") && (fromTocStr != toCStr))
                    // store old Toc if different from new
                    this._savedFieldValues.Add("TOC", this.GetToC());
                fromItem.Signature = DigitalObjectHelpers.UpdatePrimaryAccessLink(fromItem.ID, fromTocStr, fromItem.Signature);
            }
            this._resultItem.Change(fromItem._resultItem);
        }

        public void UpdateClassRelation(RQResultSet theResultSet)
        {
            string oldClass = this._savedFieldValues["Classification"];

            if (!string.IsNullOrEmpty(oldClass))
            {
                string msg = "";
                string newClass = this.ClassificationFieldContent;

                theResultSet.UpdateClassRelation(ref oldClass, ref newClass, ref msg);
            }
        }

        public XmlTextReader ConvertTo(string dataFormat)
        {
            return this._resultItem.ConvertTo(dataFormat);
        }

        public string ConvertToHTML(DisplFormat format)
        {
            var dSer = new System.Runtime.Serialization.DataContractSerializer(this.GetType());
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            var xTrf = new System.Xml.Xsl.XslCompiledTransform();
            var xTrfArg = new System.Xml.Xsl.XsltArgumentList();
            var mstr = new System.Xml.XmlTextWriter(new System.IO.MemoryStream(), System.Text.Encoding.UTF8);
            var doc = new System.Xml.XmlDocument();
            string xsltName = "";

            switch (format)
            {
                case DisplFormat.single_item:
                    xsltName = "~/xslt/ViewTransforms/RQI2SingleItemView.xslt";
                    break;
                case DisplFormat.short_title:
                    xsltName = "~/xslt/ViewTransforms/RQI2ShortTitleView.xslt";
                    break;
                default:
                    xsltName = "~/xslt/ViewTransforms/RQI2SingleItemView.xslt";
                    break;
            }
            dSer.WriteObject(ms, this);
            //TESTDATEI(EZEUGEN)
            //XmlDocument Doc = new XmlDocument();
            //ms.Seek(0, System.IO.SeekOrigin.Begin);
            //Doc.Load(ms);
            //Doc.Save("C:/MVCTest.xml");
            //ENDE TESTDATEI 
            System.IO.TextReader tr = new System.IO.StringReader(System.Text.Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Position));
            xTrf.Load(HttpContext.Current.Server.MapPath(xsltName));
            xTrfArg.AddParam("ApplPath", "", "http://" + HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST") + (HttpContext.Current.Request.ApplicationPath.Equals("/") ? "" : HttpContext.Current.Request.ApplicationPath));
            xTrfArg.AddParam("MyDocsPath", "", "http://" + HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST") + (HttpContext.Current.Request.ApplicationPath.Equals("/") ? "" : HttpContext.Current.Request.ApplicationPath));
            xTrf.Transform(new System.Xml.XPath.XPathDocument(tr), null, mstr);
            mstr.BaseStream.Flush();
            mstr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            doc.Load(mstr.BaseStream);

            //TESTDATEI EZEUGEN
            //doc.Save("C:/MVCTest.xml");
            //mstr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);

            var rd = new System.Xml.XmlTextReader(mstr.BaseStream);
            return doc.OuterXml;
        }

        public Object GetLinkedData(string fieldName, int subFieldIndex)
        {
            RQDescriptionElement de = this.GetDescriptionElement(fieldName);

            return LoadLinkedData(de, subFieldIndex);
        }

        public void LoadLinkedData(string fieldName)
        {
            RQDescriptionElement de = GetDescriptionElement(fieldName);
            int count = 1;

            if (de.GetType().BaseType == typeof(RQArrayDescriptionElement))
                count = ((RQArrayDescriptionElement)GetDescriptionElement(fieldName)).items.Count;
            for (int i = 0; i < count; i++)
                this.LoadLinkedData(de, i);
        }

        public string GetToC()
        {
            const string token = "$$TOC$$=";
            string toc = "";

            if (this.Abstract.Contains(token))
                toc = this.Abstract;
            if (this.Notes.Contains(token))
                toc = this.Notes;
            if (!string.IsNullOrEmpty(toc))
                return toc.Substring(toc.IndexOf(token) + token.Length);
            else
                return "";
        }

        public string GetSavedFieldValue(string fieldName)
        {
            if (this._savedFieldValues.ContainsKey(fieldName))
                return this._savedFieldValues[fieldName];
            else
                return "";
        }

        public string SetSavedFieldValue(string fieldName, string savedFieldValue)
        {
            string result = "";

            if (this._savedFieldValues.ContainsKey(fieldName))
            {
                result = this._savedFieldValues[fieldName];
                this._savedFieldValues[fieldName] = savedFieldValue;
            }
            else
                this._savedFieldValues.Add(fieldName, savedFieldValue);
            return result;
        }

        public static Boolean IsExternal(string docNo)
        {
            return RQResultItem.IsExternalItem(docNo);
        }

        #endregion
    }
}