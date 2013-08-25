using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using RQDigitalObjects;
using RQDigitalObjects.GraphObjects; 

namespace RQDigitalObjects.GraphObjects.PDF
{
	
    /// <summary>
	/// Diese Klasse liest ein Verzeichnis mit M4V-Dateien
	/// </summary>
	public class PdfCollection : StructuredDigitalObject
	{
       
#region private members

        private String m_filename;
        private string albumDocNr;
        private int actlTrack = 0;

#endregion


#region public properties


#endregion


#region public constructors

        /// <summary>
        /// Constructs a PDF-Collection.
        /// </summary>
        /// <param name="sourcePath">
        /// Path to the source folder of the PDF-Collection.
        /// (The source folder is assumed to be a subfolder of the document media server (default: http://[hostname]/MyDocs)).
        /// </param>
        public PdfCollection(string path)
            : base(path.Replace("XXXXX_",""), "") {}

#endregion


#region private methods

        /// <summary>
        /// Liest alle im Album-Verzeichnis enthaltenen Dateien, die dem Suchmuster entsprechen,
        /// ein und liest Dateiname aus.
        /// </summary>
        /// <param name="search">Suchpattern für die Dateinamen.</param>
        private void ReadAlbum(String search)
        {
            FileInfo[] allFiles = base.m_objectdirectory.GetFiles(search);

            for (int i = 0; i < allFiles.Length; i++)
            {
                var sd = new StringDictionary();

                sd.Add("PDFFileName", allFiles[i].Name);
                sd.Add("title", allFiles[i].Name);
                base.m_elementarray.Add(sd);
            }
        }

        private string GetTagValue(int track, string key)
        {
            if (track > 0) track = track - 1;

            StringDictionary tags = GetTagList(track);

            if ((tags != null) && (tags.ContainsKey(key)))
                return tags[key];
            else
                return "";
        }

        private StringDictionary GetTagList(int track)
        {
            if (track < base.m_elementarray.Count)
                return (StringDictionary)base.m_elementarray[track];
            else
                return null;
        }

        private void RenameAlbum(string newName)
        {
            String newpath = base.m_objectdirectory.FullName.Replace(base.m_objectdirectory.Name, newName);

            base.m_objectdirectory.MoveTo(newpath);
        }

        private string GenerateAccessLink(string resultItemID)
        {
            return resultItemID + "_" + base.m_olddirectoryname + "/" + resultItemID + "_" + this.SelectElement(0).DigitalObjectIdentifier(DigitalObject.DOIdentifier.filename);
        }

#endregion


#region public methods

        /// <summary>
        /// Reads the digital object from its source folder into memory
        /// </summary>
        public override void Read()
        {
            ReadAlbum("*.pdf");
        }

        /// <summary>
        /// Binds pdf collection to RQItem  
        /// </summary>
        /// <param name="itemId">
        /// Id of the RQItem to bind to
        /// </param>
        /// <param name="tocString">
        /// IN:     Toc from RQItem.      
        /// OUT:    Toc string to be stored in RQItem containing access structures to the parts of the pdf collection.
        /// </param>
        /// <param name="signatureStr">
        /// IN:     Signature from RQItem.
        /// OUT:    Signature string to be stored in RQItem containing the primary (i. e. first part) access link to the pdf collection.
        /// </param>
        /// <param name="itemDescription">
        /// String dictionary with field names (keys) and field values (values) of the RQItem description.
        /// </param>
        /// <returns>
        /// True if tocString or signatureStr has been changed.
        /// </returns>
        public override bool BindTo(string itemId, ref string tocString, ref string signatureStr, StringDictionary itemDescription)
        {
            bool result = false;

            if (tocString.Contains("XXXXX_"))
            {
                this.Read();
                if (this.ElementCount > 0)
                {
                    bool[] includeElements = new bool[this.ElementCount];
                    string newSigStr = "JB: MyDoc=" + this.GenerateAccessLink(itemId) + "; ";
                    string newTocStr = tocString.Replace("XXXXX_", itemId + "_");

                    if (itemDescription.ContainsKey("Abstract"))
                        itemDescription["Abstract"] = newTocStr;
                    tocString = newTocStr;
                    signatureStr += signatureStr.Contains(newSigStr) ? "" : " " + newSigStr;
                    try
                    {                    
                        this.RenameAlbum(itemId + "_" + base.m_olddirectoryname);
                    }
                    catch (Exception ex) 
                    { 
                        // Don't care. Normally no access rights to video media server.
                    }
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a digital object identifier
        /// </summary>
        /// <param name="IdType">
        /// Type of the requested digital object identifier.
        /// </param>
        /// <returns>
        /// Digital object identifier string.
        /// </returns>
        public override string DigitalObjectIdentifier(DigitalObject.DOIdentifier IdType)
        {
            switch (IdType)
            {
                case DOIdentifier.filename:
                    return this.GetTagValue(actlTrack, "PDFFileName");
                case DOIdentifier.objectname:
                    return this.GetTagValue(actlTrack, "title");
            }
            return base.DigitalObjectIdentifier(IdType);
        }

        public override String InsertPrimaryAccessLink(string fromToc, string toTarget)
        {
            string strRegex = @"MyDoc=(?<res>(\d|X){5}_[_/\w]+\.\w{3});";
            RegexOptions myRegexOptions = RegexOptions.None;
            Regex myRegex = new Regex(strRegex, myRegexOptions);
            string firstAdress = "";

            foreach (Match myMatch in myRegex.Matches(fromToc))
            {
                if (myMatch.Success)
                {
                    firstAdress = myMatch.Groups["res"].Value;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(firstAdress))
            {
                strRegex = @"(\w+: )?MyDoc=(?<res>(\d|X){5}_[_/\w]+(\.\w{3})?);\x20?";
                myRegex = new Regex(strRegex, myRegexOptions);
                foreach (Match myMatch in myRegex.Matches(toTarget))
                    if ((myMatch.Success) && (myMatch.Groups["res"].Value == firstAdress))
                        return toTarget;
                toTarget = toTarget.Insert(0, "JB: MyDoc=" + firstAdress + "; ");
            }
            return toTarget;
        }

        /// <summary>
        /// Setzt das aktuelle Element (einer Kopie) des digitalen Objekts auf einen vorgegebenen Wert. 
        /// </summary>
        /// <param name="index">
        /// Ganzzahl, die die 0-basierte Position des neuen aktuellen Elements (in der zurück gegebenen Kopie des digitalen Objekts) angibt.
        /// </param>
        /// <returns>
        /// Gibt eine Kopie des aktuellen digitalen Objekts zurück, bei der das Element an der Psoition index das aktuelle Element ist.
        /// </returns>
        public override DigitalObject SelectElement(int index)
        {
            PdfCollection newdo = (PdfCollection)this.MemberwiseClone();
            newdo.actlTrack = index;
            return newdo;
        }

        public override string GenerateToc(string objectName, string serverDirectory)
        {
            string result = "$$TOC$$=";
            string dummyId = "";
            string strRegex = @"(?<res>\d{5})_";
            RegexOptions myRegexOptions = RegexOptions.None;
            Regex myRegex = new Regex(strRegex, myRegexOptions);
            Match MyMatch = myRegex.Match(objectName);

            if (!MyMatch.Success)
                dummyId = "XXXXX_";
            objectName = dummyId + objectName;
            this.Read();
            for (int i = 0; i < this.ElementCount; i++)
            {
                result += this.SelectElement(i + 1).DigitalObjectIdentifier(DigitalObject.DOIdentifier.objectname);
                result += serverDirectory + objectName + "/" + dummyId + this.SelectElement(i + 1).DigitalObjectIdentifier(DigitalObject.DOIdentifier.filename) + "; ";
            }
            return result;
        }

#endregion

    }

}
