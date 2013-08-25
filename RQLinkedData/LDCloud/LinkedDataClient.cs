using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;


namespace RQLinkedData.LDCloud
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für ClassificationSystemClient
    /// </summary>
    public class LinkedDataClient
    {
        private LDBase _ldbase = null;
                
        protected virtual void Initialize()
        {
        }
        
        public LinkedDataClient()
        {
        }

        public LDBase LDGraph()
        {
            if (this._ldbase == null)
            {
                this._ldbase = new LDBase();
                this.Initialize();
            }
            return _ldbase;
        }

        public virtual void Load(string uri)
        {
            this.LDGraph().Load(uri);
        }

        public virtual void Load( System.Collections.Specialized.StringDictionary nodeDictionary)
        {
        }
                
        public void Write(System.Xml.XmlWriter writer)
        {
            this.LDGraph().Write(writer);
        }
    }
}