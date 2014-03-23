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
    /// Model class to present the RiQuest Knowledge Organiszation (RqKOS) 
    /// </summary>
    [DataContract]
    public class RQKosModel
    {
        #region "public properties"

        /// <summary>
        /// RQKosBranch
        /// </summary>
        [DataMember]
        public RQKosBranch RQKosSet { get; set; }

        #endregion

        #region "public constructors"

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
    /// Model class to edit the RiQuest Knowledge Organization (RqKOS)
    /// </summary>
    public class RQKosEditModel
    {
        #region private members

        private System.Collections.Generic.List<RQKosBranch> _mClassBranches = null;
        private RQKosBranch.RQKosBranchStatus _mEditStatus;
        private string _mEditClassID = "";

        #endregion

        #region private methods

        private void Clear()
        {
            if (this._mClassBranches != null)
                for (var i=0; i < this._mClassBranches.Count; i++)
                    if (this._mClassBranches[i] != null)
                    {
                        this._mClassBranches[i].ClassBranch.Clear();
                        this._mClassBranches[i] = null;
                    }
        }    

        private SubjClassBranch Find(string majClassID)
        {
            for (var i = 0; i < this._mClassBranches.Count; i++)
            {
                if (this._mClassBranches[i] != null)
                    if (this._mClassBranches[i].ClassBranch.MajorClassID == majClassID)
                        return this._mClassBranches[i].ClassBranch;
            }

            SubjClassBranch cb = new SubjClassBranch(ref majClassID);

            cb.Load();
            for (var i = 0; i < this._mClassBranches.Count; i++)
            {
                if (this._mClassBranches[i] == null)
                {
                    this._mClassBranches[i] = new RQKosBranch(cb);
                    return cb;
                }
            }
            this._mClassBranches.Add(new RQKosBranch(cb));
            return cb;
        }

        #endregion

        #region public properties

        public RQKosBranch RQKosEditSet
        {
            get
            {
                return new RQKosBranch(this.Find(this._mEditClassID));
            }
        }

        public RQKosBranch.RQKosBranchStatus RQKosEditStatus
        {
            get
            {
                return _mEditStatus;
            }
        }

        #endregion

        #region public constructors

        public RQKosEditModel()
        {
            this.Clear();
            _mClassBranches = new System.Collections.Generic.List<RQKosBranch>();
        }

        public RQKosEditModel(IEnumerable<RQKosTransfer> RQKosTransferBranch)
            :this()
        {
            RQKosBranch cb = new RQKosBranch(RQKosTransferBranch);

            this._mEditClassID = cb.ClassBranch.MajorClassID;
            this._mClassBranches.Add(cb);
        }

        public RQKosEditModel(string itemID)
            :this()
        {
            RQKosBranch cb = null;

            if (itemID == null || itemID == String.Empty) itemID = "0";
            cb = new RQKosBranch(itemID, "");
            cb.Load();
            this._mEditClassID = cb.ClassBranch.MajorClassID;
            this._mClassBranches.Add(cb);
        }

        #endregion

        #region public methods

        public bool IsCompatible()
        {
            SubjClassBranch editCB = this.Find(this._mEditClassID);
            RQKosBranch oldCB = new RQKosBranch(this._mEditClassID,"");
            bool retVal = true;
 
            oldCB.Load();
            for (var i = 1; i < editCB.count; i++)
            {
                if (oldCB.ClassBranch.get_Item(i + 1) != null)
                {
                    SubjClass sc = editCB.get_Item(i);

                    if ((sc.NrOfSubClasses > 0) && (sc.RefRVKSet != oldCB.ClassBranch.get_Item(i + 1).RefRVKSet))
                    {
                        if (!this.Find(sc.ClassID).IsFeasableWith(ref sc))
                            retVal = retVal && false;
                    }
                }
            }
            return retVal;
        }

        public RQKosBranch.RQKosBranchStatus IsValid()
        {
            SubjClassBranch editCB = this.Find(this._mEditClassID);
            bool retValue = false;
            
            if (editCB.IsValid())
                retValue = true;
            if (retValue = this.IsCompatible() ? retValue && true : false)
                return _mEditStatus = new RQKosBranch.RQKosBranchStatus() { isSuccess = true, message = "Class mapping is consistent!", hints = RQLib.EditGlobals.ReadHints() };
            else
                return _mEditStatus = new RQKosBranch.RQKosBranchStatus() { isSuccess = false, message = "Consistency errors in class mapping!", hints = RQLib.EditGlobals.ReadHints() };
        }

        public RQKosBranch AppendClass()
        {
            SubjClassBranch editCB = this.Find(this._mEditClassID);
            SubjClass newSC = new SubjClass();

            newSC.ClassificationSystem = SubjClass.ClassificationSystems.rq;
            newSC.NrOfClassDocs = 0;
            newSC.NrOfRefLinks = 0;
            newSC.NrOfSubClasses = 0;
            newSC.ParentClassID = this._mEditClassID;
            editCB.Add(newSC);
            return this.RQKosEditSet;
        }

        public bool Update()
        {
            bool retValue = false;
            SubjClassBranch editCB = this.Find(this._mEditClassID);
            
            if (editCB.IsValid())
                retValue = true;
            if (retValue = this.IsCompatible() ? retValue && true : false)
            {
                _mEditStatus = new RQKosBranch.RQKosBranchStatus() { isSuccess = true, message = "Class mapping is consistent!", hints = RQLib.EditGlobals.ReadHints() };
                editCB.Update();
            }
            else
                _mEditStatus = new RQKosBranch.RQKosBranchStatus() { isSuccess = false, message = "Consistency errors in class mapping!", hints = RQLib.EditGlobals.ReadHints() };
            return retValue;
        }

        public bool Delete()
        {
            bool retValue = false;
            SubjClassBranch editCB = this.Find(this._mEditClassID);

            try {
                _mEditStatus = new RQKosBranch.RQKosBranchStatus() { isSuccess = false, message = "Consistency errors in class mapping!", hints = RQLib.EditGlobals.ReadHints() };
                editCB.Delete();
            }
            catch {
                retValue = false;
            }
            return retValue;
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
    [XmlInclude(typeof(RQKosItem))]
    [XmlInclude(typeof(RQKosItemRQLD))]
    public class RQKosBranch : System.Collections.Generic.IEnumerable<RQKosItemTemplate>
    {
        #region "private members"

        private SubjClassBranch classBranch;
        private string _service;

        #endregion

        #region "public members"

        [DataContract]
        public struct RQKosBranchStatus
        {
            [DataMember]
            public bool isSuccess;
            [DataMember]
            public string message;
            [DataMember]
            public RQLib.EditGlobals.Hint[] hints;
        }

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
                    return (this.classBranch != null) ? this.classBranch.count : 0;
            }
        }

        #endregion

        #region "public constructors"

        public RQKosBranch()
            : base()
        { }

        public RQKosBranch(string majClassID, string serviceId)
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

        public RQKosBranch(IEnumerable<RQKosTransfer> newRQKosBranch)
            : base()
        {
            SubjClass[] classArray = new SubjClass[newRQKosBranch.Count()];

            for (int i = 0; i < newRQKosBranch.Count(); i++)
            {
                classArray[i] = new SubjClass();
                classArray[i].ClassCode = newRQKosBranch.ElementAt(i).ClassCode;
                classArray[i].ClassID = newRQKosBranch.ElementAt(i).ClassID;
                classArray[i].ClassShortTitle = newRQKosBranch.ElementAt(i).ClassName;
                classArray[i].ClassLongTitle = newRQKosBranch.ElementAt(i).ClassTitle;
                classArray[i].NrOfRefLinks = Convert.ToInt16(newRQKosBranch.ElementAt(i).NrOfDocuments);
                classArray[i].NrOfSubClasses = Convert.ToInt16(newRQKosBranch.ElementAt(i).NrOfSubclasses);
                classArray[i].ParentClassID = newRQKosBranch.ElementAt(i).ParentID;
                classArray[i].RefRVKSet = newRQKosBranch.ElementAt(i).RVKClassCodes;
                classArray[i].RefRVKClass = new RQLib.Utilities.LexicalClass(newRQKosBranch.ElementAt(i).RVKClassCodes);
                classArray[i].ClassDataClient = new RQClassificationDataClient();
            }
            this.classBranch = new SubjClassBranch(classArray);
        }

        public RQKosBranch(SubjClassBranch subjClassBranch)
            : base()
        {
            this.classBranch = subjClassBranch;
        }

        #endregion

        #region "public methods"

        public void Add(RQKosItemTemplate item)
        {
            SubjClass cl = item._class;

            cl.NrOfRefLinks = 0;
            cl.NrOfSubClasses = 0;
            cl.ParentClassID = this.classBranch.MajorClassID;
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

        //public RQKosBranchStatus CheckConsistency()
        //{
        //    string majClassId = this.classBranch.get_Item(0).ClassID;
        //    SubjClassManager sclm = new SubjClassManager();

        //    if (SubjClassManager.IsValid(this.classBranch))
        //        return new RQKosBranchStatus() { isSuccess = true, message = "Mapping is consistent!", hints = RQLib.EditGlobals.ReadHints() };
        //    else
        //        return new RQKosBranchStatus() { isSuccess = false, message = "Consistency Errors", hints = RQLib.EditGlobals.ReadHints() };
        //}

        public void Load()
        {
            this.classBranch.Load();
            if (this.classBranch.MajorClassID == "0")
            {
                SubjClass sc = this.classBranch.get_Item(0);

                sc.ClassCode = "NULL";
                sc.ClassShortTitle = "RQ Classification";
                sc.NrOfSubClasses = 15;
                sc.NrOfRefLinks = 0;
            }
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
    /// Helper Class to bind json input from browser to class RQKOSBranch
    /// Circumvention of problems arising by direct binding json to RQKosBranch
    /// </summary>
    public class RQKosTransfer
    {
        #region "public properties"

        [DataMember]
        public string ClassID { get; set; }

        [DataMember]
        public string ClassCode { get; set; }

        [DataMember]
        public string ParentID { get; set; }

        [DataMember]
        public string ClassName { get; set; }

        [DataMember]
        public string ClassTitle { get; set; }

        [DataMember]
        public string RVKClassCodes { get; set; }

        [DataMember]
        public string NrOfSubclasses { get; set; }

        [DataMember]
        public string NrOfDocuments { get; set; }

        #endregion

        #region "public constructors"

        public RQKosTransfer() { }

        #endregion
    }

    /// <summary>
    /// Description of a single subject class
    /// </summary>
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

    /// <summary>
    /// Description of a single subject class in editor variant
    /// </summary>
    [KnownType(typeof(RQKosItem))]
    public class RQKosItem
        : RQKosItemTemplate
    {
        #region "public properties"

        //[DataMember]
        //public SubjClass classification
        //{
        //    get
        //    {
        //        return this._class;
        //    }
        //    set 
        //    {
        //        this._class = classification;
        //    }
        //}

        [DataMember]
        public string ClassID
        {
            get
            {
                return this._class.ClassID;
            }
            set
            {
                this._class.ClassID = ClassID;
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
        public string ParentID
        {
            get
            {
                return this._class.ParentClassID;
            }
            set
            {
                this._class.ParentClassID = ParentID;
            }
        }

        [DataMember]
        public string ClassName
        {
            get
            {
                return this._class.ClassShortTitle;
            }
            set
            {
                this._class.ClassShortTitle = ClassName;
            }
        }

        [DataMember]
        public string ClassTitle
        {
            get
            {
                return this._class.ClassLongTitle;
            }
            set
            {
                this._class.ClassLongTitle = ClassTitle;
            }
        }

        [DataMember]
        public string RVKClassCodes
        {
            get
            {
                return this._class.RefRVKSet;
            }
            set
            {
                this._class.RefRVKSet = RVKClassCodes;
            }
        }

        [DataMember]
        public string NrOfSubclasses
        {
            get
            {
                return this._class.NrOfSubClasses.ToString();
            }
            set
            {
                this._class.NrOfSubClasses = int.Parse(NrOfSubclasses);
            }
        }

        [DataMember]
        public string NrOfDocuments
        {
            get
            {
                return this._class.NrOfClassDocs.ToString();
            }
            set
            {
                this._class.NrOfClassDocs = int.Parse(NrOfDocuments);
            }
        }

        #endregion

        #region "public constructors"

        public RQKosItem()
            : base() { }

        public RQKosItem(SubjClass thisClass)
            : base(thisClass) { }

        #endregion
    }

    /// <summary>
    /// Description of a single subject class in DynaTree variant
    /// </summary>
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
                        this._tooltip += " - show " + Convert.ToString(this._class.NrOfClassDocs) + " documents, " + Convert.ToString(this._class.NrOfRefLinks) + " bookmarks";
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

    /// <summary>
    /// Description of a single subject class in semantic web linked data variant
    /// </summary>
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