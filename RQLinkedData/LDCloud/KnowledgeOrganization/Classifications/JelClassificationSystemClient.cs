using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;


namespace RQLinkedData.LDCloud.KnowledgeOrganization.Classifications
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für ClassificationSystemClient
    /// </summary>
    public class JelClassificationSystemClient : ClassificationSystemClient
    {
         static public string GetURI(string classNotation)
        {
            return (classNotation != "") ? "http://zbw.eu/beta/external_identifiers/jel" + "#" + AdaptClassNotation(classNotation) : "http://zbw.eu/beta/external_identifiers/jel";
        }
        
        static public string GetPredicate(ClassificationPredicates predicate)
        {
            return GetPredicate(ClassificationSystems.jel, predicate);
        }
        
        static public string AdaptClassNotation(string classNotation)
        {
            return classNotation.Substring(0,3);
        }

        public JelClassificationSystemClient()
            :base()
        {
        }

        public override void Load(string uri)
        {
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "LinkedData/DataSets/jel.rdf"))
                this.LDGraph().LoadFile(AppDomain.CurrentDomain.BaseDirectory + "LinkedData/DataSets/jel.rdf");
            else
                this.Load(uri);
        }

        public override void Load( System.Collections.Specialized.StringDictionary nodeDictionary)
        {
            //string subj = "http://www.riquest.de/rqld/rqkos/rqc_" + nodeDictionary["ClassCode"];

            //this._ldbase.SetNamespace("xhv", new Uri("http://www.w3.org/1999/xhtml/vocab#"));
            //this._ldbase.SetNamespace("skos", new Uri("http://www.w3.org/2004/02/skos/core#"));
            //this._ldbase.SetNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            //this._ldbase.CreateTriple(subj, null, "rdf:type", null, "skos:concept", null);
            //this._ldbase.CreateTriple(subj, null, "xhv:licence", null, "http://creativecommons.org/licenses/by-nc-nd/3.0/", null);
            //this._ldbase.CreateTriple(subj, null, "skos:inScheme", null, "http://www.riquest.de/rqld/rqkos/rqc-scheme", null);
            //if (nodeDictionary["ClassCode"] != null )  this._ldbase.CreateTriple(subj, null, "skos:notation", null, nodeDictionary["ClassCode"], "http://www.riquest.de/rqld/rqkos/rqc-schema#Notation");
            //if (nodeDictionary["ClassLongTitle"] != null) this._ldbase.CreateTriple(subj, null, "skos:prefLabel", null, nodeDictionary["ClassLongTitle"], "de");
            //if (nodeDictionary["ClassShortTitle"] != null) this._ldbase.CreateTriple(subj, null, "skos:altLabel", null, nodeDictionary["ClassShortTitle"], "de");
            //if (nodeDictionary["Broader"] != null) this._ldbase.CreateTriple(subj, null, "skos:broader", null, "http://www.riquest.de/rqld/rqkos/rqc_" + nodeDictionary["Broader"], null);
        }

        public override string GetPreferredLabel(string classNotation)
        {
            try
            {
                string[] res;

                res = this.LDGraph().ObjectOf(JelClassificationSystemClient.GetURI(AdaptClassNotation(classNotation)), JelClassificationSystemClient.GetPredicate(ClassificationPredicates.preferred_label));
                return res[0].Substring(res[0].IndexOf("- ") + 2);
            }
            catch
            {
                return "";
            }
        }

    }
}