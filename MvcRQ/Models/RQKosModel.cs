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
        /// <param name="formatID">
        /// An empty string or "dt" if model returns input for DynaTree.
        /// </param>
        /// <remarks>
        /// Loads the subject class branch of major class with ID=itemID into the RQKosSet.
        /// If itemID is empty the uppermost class branch is loaded.
        /// </remarks>
        public RQKosModel(string itemID, string formatID)
        {
            if (itemID == null || itemID == String.Empty) itemID = "0";
            RQKosSet = new RQKosBranch(itemID, formatID);
            RQKosSet.Load();
        }
    }


    /// <summary>
    /// A single subject classification item of the RiQuest Knowledge Organisation System (KOS)  
    /// </summary>
    /// <remarks>
    /// The collection contains a description of the major subject class at the zero position followed by descriptions of the minor subject classes (subclasses) 
    /// </remarks>
    [CollectionDataContract]
    public class RQKosBranch : System.Collections.Generic.IEnumerable<RQKosItemTemplate>
    {
        private SubjClassBranch classBranch;
        private string _format;

        [DataMember]
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
                if (this._format == "dt")
                    return this.classBranch.count > 0 ? this.classBranch.count - 1 : 0;
                else
                    return this.classBranch.count;
            }
        }

        public RQKosBranch()
            : base()
        { }

        public RQKosBranch( string majClassID, string formatID)
            : base()
        {
            this.classBranch = new SubjClassBranch(ref majClassID);
            this._format = formatID;
        }

        public void Add(RQKosItemTemplate item)
        {
            SubjClass cl = item._class;

            this.classBranch.Add(ref cl);
        }

        public RQKosItemTemplate GetItem(int i)
        {
            if (this._format == "dt")
                return new RQKosItemDT(classBranch.get_Item(i + 1));
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
    }


    /// <summary>
    /// Enumeration class for RQKosBranch
    /// </summary>
    public class RQKosBranchEnum : IEnumerator<RQKosItemTemplate>
    {
        private RQKosBranch _itemSet;
        private int _curIndex;
        private RQKosItemTemplate _curItem;

        public RQKosBranchEnum(RQKosBranch kosSet)
        {
            _itemSet = kosSet;
            _curIndex = -1;
            _curItem = default(RQKosItemTemplate);
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
    }


    /// <summary>
    /// Description of a single subject class
    /// </summary>
    [XmlRoot]
    abstract public class RQKosItemTemplate
    {
        internal SubjClass _class { get; set; }


        //[DataMember]
        //public string ID
        //{
        //    get
        //    {
        //        return this._class.ClassID;
        //    }
        //    set
        //    {
        //        this._class.ClassID = ID;
        //    }
        //}


        //[DataMember]
        //public string ClassCode
        //{
        //    get
        //    {
        //        return this._class.ClassCode;
        //    }
        //    set
        //    {
        //        this._class.ClassCode = ClassCode;
        //    }
        //}


        //[DataMember]
        //public string ClassLongTitle
        //{
        //    get
        //    {
        //        return this._class.ClassLongTitle;
        //    }
        //    set
        //    {
        //        this._class.ClassLongTitle = ClassLongTitle;
        //    }
        //}

        
        public RQKosItemTemplate()
        {
            this._class = new SubjClass();
        }


        public RQKosItemTemplate(SubjClass thisClass)
        {
            this._class = thisClass;
        }
   
    }


    [KnownType(typeof(RQKosItem))]
    public class RQKosItem 
        : RQKosItemTemplate
    {
        [DataMember]
        public string ID
        {
            get
            {
                return this._class.ClassID;
            }
            set
            {
                this._class.ClassID = ID;
            }
        }


        [DataMember]
        public string ClassCode
        {
            get
            {
                return this._class.ClassCode;
            }
            set
            {
                this._class.ClassCode = ClassCode;
            }
        }


        [DataMember]
        public string ClassLongTitle
        {
            get
            {
                return this._class.ClassLongTitle;
            }
            set
            {
                this._class.ClassLongTitle = ClassLongTitle;
            }
        }


        public RQKosItem()
            : base() {}


        public RQKosItem(SubjClass thisClass)
            : base(thisClass) {}

    }


    [KnownType(typeof(RQKosItemDT))]
    public class RQKosItemDT
    : RQKosItemTemplate
    {

        private bool _isLazy = true;
        private string _tooltip = "";

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


        public RQKosItemDT()
            : base() { }


        public RQKosItemDT(SubjClass thisClass)
            : base(thisClass) { }

    }
}