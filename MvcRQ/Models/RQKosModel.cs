using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using RQLib.RQKos.Classifications;
using RQLib.RQQueryForm;


namespace MvcRQ.Models
{
    /// <summary>
    /// Model class for the RiQuest Knowledge Organiszation (KOS) Model
    /// </summary>
    public class RQKosModel
    {
        #region "public constructors"

        /// <summary>
        /// RQKosBranch
        /// </summary>
        public RQKosBranch RQKosSet { get; set; }
        
        /// <summary>
        /// Constructor of RQKosModel
        /// </summary>
        /// <param name="itemID">
        /// Valid RQKosItemId
        /// </param>
        /// <param name="serviceId">
        /// serviceId == "dt": DynyTree API.
        /// </param>
        /// <remarks>
        /// Loads the subject class branch of major class with ID=itemID into the RQKosSet.
        /// If itemID is empty the uppermost class branch is loaded.
        /// </remarks>
        public RQKosModel(string itemID, string serviceId)
        {
            if (itemID == null || itemID == String.Empty) itemID = "0";
            RQKosSet = new RQKosBranch(itemID, serviceId);
            RQKosSet.Load();
        }

        #endregion
    }

    /// <summary>
    /// A single subject classification item of the RiQuest Knowledge Organisation System (KOS)  
    /// </summary>
    /// <remarks>
    /// The collection contains a description of the major subject class at the zero position followed by descriptions of the minor subject classes (subclasses) 
    /// </remarks>
    [CollectionDataContract]
    [XmlRoot]
    [XmlInclude(typeof(RQKosItem))]
    [XmlInclude(typeof(RQKosItemRQLD))]
    public class RQKosBranch : System.Collections.Generic.IEnumerable<RQKosItemTemplate>
    {
        #region "private members"

        private SubjClassBranch classBranch;
        private string _service;

        #endregion

        #region "public properties"

        public SubjClassBranch ClassBranch
        {
            get
            {
                return this.classBranch;
            }
            set
            {
                this.classBranch = ClassBranch;
            }
        }
        
        public int count
        {
            get
            {
                if (this._service == "dt")
                    return this.classBranch.count > 0 ? this.classBranch.count - 1 : 0;
                else
                    return this.classBranch.count;
            }
        }

        #endregion

        #region "public constructors"

        public RQKosBranch()
            : base()
        { }

        public RQKosBranch( string majClassID, string serviceId)
            : base()
        {
            int numVal = -1;
   
            this._service = serviceId;
            try
            {
                numVal = Convert.ToInt32(majClassID);
            }
            catch { }
            if (numVal == -1)
            {
                string id = "";
                switch (majClassID.Substring(0, 4))
                {
                    case "rvk_":
                        id = majClassID.Substring(4, majClassID.Length - 4);
                        this.classBranch = new SubjClassBranch(ref id, SubjClass.ClassificationSystems.rvk);
                        break;
                    case "rqc_":
                        id = majClassID.Substring(4, majClassID.Length - 4);
                        this.classBranch = new SubjClassBranch(ref id, SubjClass.ClassificationSystems.rq);
                        break;
                    default:
                        this.classBranch = null;
                        break;
                }
            }
            else
                this.classBranch = new SubjClassBranch(ref majClassID);
        }
        
#endregion

        #region "public methods"

        public void Add(RQKosItemTemplate item)
        {
            SubjClass cl = item._class;

            this.classBranch.Add(cl);
        }

        public RQKosItemTemplate GetItem(int i)
        {
            if (this._service == "dt")
                return new RQKosItemDT(classBranch.get_Item(i + 1));
            else if (this._service == "rqld")
                return new RQKosItemRQLD(classBranch.get_Item(i));
            else
                return new RQKosItem(classBranch.get_Item(i));
        }

        public void Load()
        {
            this.classBranch.Load();
        }

        public System.Collections.Generic.IEnumerator<RQKosItemTemplate> GetEnumerator()
        {
            return new RQKosBranchEnum(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new RQKosBranchEnum(this);
        }

        #endregion
    }

    /// <summary>
    /// Enumeration class for RQKosBranch
    /// </summary>
    public class RQKosBranchEnum : IEnumerator<RQKosItemTemplate>
    {
        #region "private members"

        private RQKosBranch _itemSet;
        private int _curIndex;
        private RQKosItemTemplate _curItem;

        #endregion

        #region "public constructors"

        public RQKosBranchEnum(RQKosBranch kosSet)
        {
            _itemSet = kosSet;
            _curIndex = -1;
            _curItem = default(RQKosItemTemplate);
        }

        #endregion

        #region "public methods"

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

        public RQKosItemTemplate Current
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

    /// <summary>
    /// Description of a single subject class
    /// </summary>
    [XmlRoot]
    abstract public class RQKosItemTemplate
    {
        #region "private properties"

        //internal SubjClass _class { get; set; }
        internal SubjClass _class { get; set; }
        
        #endregion

        #region "public constructors"

        public RQKosItemTemplate()
        {
            this._class = new SubjClass();
        }

        public RQKosItemTemplate(SubjClass thisClass)
        {
            this._class = thisClass;
        }

        #endregion
    }

    [KnownType(typeof(RQKosItem))]
    public class RQKosItem 
        : RQKosItemTemplate
    {
        #region "public properties"

        [DataMember]
        public SubjClass classification
        {
            get
            {
                return this._class;
            }
            set 
            {
                this._class = classification;
            }
        }
        
        #endregion

        #region "public constructors"

        public RQKosItem()
            : base() {}
        
        public RQKosItem(SubjClass thisClass)
            : base(thisClass) { }

        #endregion
    }

    [KnownType(typeof(RQKosItemDT))]
    public class RQKosItemDT
    : RQKosItemTemplate
    {
        #region "private members"

        private bool _isLazy = true;
        private string _tooltip = "";
        
        #endregion

        #region "public properties"

        [DataMember]
        public string title
        {
            get
            {
                return this._class.ClassShortTitle;
            }
            set
            {
                this._class.ClassShortTitle = title;
            }
        }
        
        [DataMember]
        public bool isFolder
        {
            get
            {
                if (this._class.NrOfSubClasses > 0)
                    return true;
                else
                    return false;
            }
            set
            {
            }
        }
        
        [DataMember]
        public bool isLazy
        {
            get
            {
                return this._isLazy;
            }
            set
            {
                this._isLazy = isLazy;
            }
        }
        
        [DataMember]
        public string key
        {
            get
            {
                return this._class.ClassID + "$" + this._class.ClassCode;
            }
            set
            {
                int si = key.IndexOf("$");

                this._class.ClassID = key.Substring(0, si) + key.Substring(si + 1);
            }
        }
        
        [DataMember]
        public string unselectable
        {
            get
            {
                if (this._class.NrOfClassDocs + this._class.NrOfRefLinks > 0)
                    return "false";
                else
                    return "true";
            }
            set
            {
            }
        }
        
        [DataMember]
        public string tooltip
        {
            get
            {
                this._tooltip = this._class.ClassCode + " " + this._class.ClassShortTitle;
                if (this._class.NrOfSubClasses > 0)
                    if (this._class.NrOfSubClasses == 1)
                        this._tooltip += " : expand 1 subclass";
                    else
                        this._tooltip += " : expand " + this._class.NrOfSubClasses + " subclasses";
                if (this._class.NrOfClassDocs + this._class.NrOfRefLinks > 0)
                    if (this._class.NrOfClassDocs + this._class.NrOfRefLinks == 1)
                        this._tooltip += " - show 1 document";
                    else
                        this._tooltip += " - show " + Convert.ToString(this._class.NrOfClassDocs + this._class.NrOfRefLinks) + " documents"; 
                return this._tooltip;
            }
            set
            {
                this._tooltip = tooltip;
            }
        }

        #endregion

        #region "public constructors"
        
        public RQKosItemDT()
            : base() { }


        public RQKosItemDT(SubjClass thisClass)
            : base(thisClass) { }

        #endregion
    }

    [KnownType(typeof(RQKosItemRQLD))]
    [KnownType(typeof(RQLib.RQLD.RQClassificationGraph))]
    public class RQKosItemRQLD
    : RQKosItemTemplate
    {
        #region "private members"

        #endregion

        #region "public properties"

        [DataMember]
        public RQLib.RQLD.RQClassificationGraph RDF
        {
            get
            {
                if ((this._class.ClassID != null) && (this._class.ClassID != "0"))
                    return this._class.RDFGraph;
                else
                    return null;
            }
            set
            {
                this._class.ClassShortTitle = "test";
            }
        }

        #endregion

        #region "public constructors"

        public RQKosItemRQLD()
            : base() { }


        public RQKosItemRQLD(SubjClass thisClass)
            : base(thisClass) { }

        #endregion
    }
}