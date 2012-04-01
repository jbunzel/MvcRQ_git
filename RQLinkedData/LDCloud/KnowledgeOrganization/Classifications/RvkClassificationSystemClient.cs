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
    public class RvkClassificationSystemClient : ClassificationSystemClient
    {
        static public string GetURI(string classNotation)
        {
            return (classNotation != "") ? "http://data.bib.uni-mannheim.de/data/rvk" + "/" + ClassificationSystemClient.AdaptClassNotation(ClassificationSystems.rvk, classNotation) + "/about.rdf" : "http://data.bib.uni-mannheim.de/data/rvk";
        }
        
        static public string GetPredicate(ClassificationPredicates predicate)
        {
            return GetPredicate(ClassificationSystems.rvk, predicate);
        }
        
        static public string AdaptClassNotation(string classNotation)
        {
            string[] x = Regex.Split(classNotation, "[0-9]");
            string[] y = Regex.Split(classNotation, "[A-Z]");
            return (y[y.Length-1] == "") ? x[0] : x[0] + "_" +y[y.Length-1];
        }

        public RvkClassificationSystemClient()
            :base()
        {
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
            return "";
        }
    }
}