using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Linq;
using System.Web;


namespace RQDigitalObjects
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für DigitalObject
    /// </summary>
    public class DigitalObject
    {

#region private members

        protected DirectoryInfo m_objectdirectory;
        protected string m_olddirectoryname;
        protected string m_localContainer;    

#endregion


#region public members

        public enum DOType {
            mp3Text,
            mp3Music,
            mv4Video,
            pdfDocument
        }
        
        public enum DOIdentifier {
            filename,
            persistentURL,
            objectname
        }

#endregion


#region public properties

        public String DirectoryName
        {
            get
            {
                return this.m_objectdirectory.FullName;
            }
        }

        public virtual int ElementCount
        {
            get
            {
                return 1;
            }
        }

#endregion


#region public constructors

        /// <summary>
        /// Constructs a digital object
        /// </summary>
        /// <param name="sourcePath">
        /// Path to the source folder of the digital object  
        /// </param>
        /// <param name="targetFolder">
        /// Name of the subfolder on the media server, where the digital object shall be stored. 
        /// </param>
        public DigitalObject(string sourcePath, string targetFolder)
		{
			this.m_objectdirectory = new DirectoryInfo(sourcePath);
            this.m_olddirectoryname = this.m_objectdirectory.Name;
            this.m_localContainer = targetFolder;
		}

#endregion


#region public methods

        /// <summary>
        /// Reads the digital object from its source folder into memory
        /// </summary>
        public virtual void Read()
        {
        }

        /// <summary>
        /// Binds digital object to RQItem  
        /// </summary>
        /// <param name="itemId">
        /// Id of the RQItem to bind to
        /// </param>
        /// <param name="tocString">
        /// IN:     Toc from RQItem.      
        /// OUT:    Toc string to be stored in RQItem containing access structures to content elements of the digital object.
        /// </param>
        /// <param name="signatureStr">
        /// IN:     Signature from RQItem.
        /// OUT:    Signature string to be stored in RQItem containing the primary access link to the digital object
        /// </param>
        /// <param name="itemDescription">
        /// String dictionary with field names (keys) and field values (values) of the RQItem description.
        /// </param>
        /// <returns>
        /// True if tocString or signatureStr has been changed.
        /// </returns>
        public virtual bool BindTo(string itemId, ref string tocString, ref string signatureStr, StringDictionary itemDescription)
        {
            return false;
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
        public virtual String DigitalObjectIdentifier(DOIdentifier IdType)
        {
            return null;
        }

        public virtual String InsertPrimaryAccessLink(string fromToc, string toTarget) 
        {
            return toTarget;
        }

#endregion

    }
}