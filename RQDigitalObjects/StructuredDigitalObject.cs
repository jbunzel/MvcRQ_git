using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
//using System.Linq;
using System.Web;

namespace RQDigitalObjects
{

    /// <summary>
    /// Zusammenfassungsbeschreibung für StructuredDigitalObject
    /// </summary>
    public abstract class StructuredDigitalObject : DigitalObject
    {

#region protected members

        protected ArrayList m_elementarray = new ArrayList();

#endregion


#region public properties

        public ArrayList ElementList
        {
            get
            {
                return m_elementarray;
            }
        }

        public override int ElementCount
        {
            get
            {
                if (this.m_elementarray != null)
                    return this.m_elementarray.Count;
                else
                    return 0;
            }
        }

#endregion


#region public constructors

        /// <summary>
        /// Constructs a structured digital object
        /// </summary>
        /// <param name="sourcePath">
        /// Path to the source folder of the digital object  
        /// </param>
        /// <param name="targetFolder">
        /// Name of the subfolder on the media server, where the digital object shall be stored. 
        /// </param>
        public StructuredDigitalObject(string sourcePath, string targetFolder) : base (sourcePath, targetFolder) {}

#endregion


#region public methods

        /// <summary>
        /// Setzt das aktuelle Element (einer Kopie) des digitalen Objekts auf einen vorgegebenen Wert. 
        /// </summary>
        /// <param name="index">
        /// Ganzzahl, die die 0-basierte Position des neuen aktuellen Elements (in der zurück gegebenen Kopie des digitalen Objekts) angibt.
        /// </param>
        /// <returns>
        /// Gibt eine Kopie des aktuellen digitalen Objekts zurück, bei der das Element an der Psoition index das aktuelle Element ist.
        /// </returns>
        public abstract DigitalObject SelectElement(int index);

        public abstract string GenerateToc(string objectName, string serverDirectory);

#endregion

    }
}

