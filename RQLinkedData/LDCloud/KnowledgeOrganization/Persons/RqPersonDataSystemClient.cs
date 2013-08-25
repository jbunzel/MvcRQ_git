using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;


namespace RQLinkedData.LDCloud.KnowledgeOrganization.Persons
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für ClassificationSystemClient
    /// </summary>
    public class RqPersonDataSystemClient : PersonDataSystemClient
    {
        static public string GetURI(string personCode)
        {
            return (personCode != "") ? "http://mvcrq.strands.de/rqld/rqkos" + "/rqc_" + personCode : "http://mvcrq.strands.de/rqld/rqkos";
        }
        
        static public string GetPredicateURI(PersonDataPredicates predicate)
        {
            return GetPredicateURI(PersonDataSystems.rq, predicate);
        }
        
        static public string AdaptClassNotation(string personCode)
        {
            return personCode;
        }

        public RqPersonDataSystemClient()
            :base()
        {
        }

        public override void Load( System.Collections.Specialized.StringDictionary nodeDictionary)
        {
            //string subj = GetURI(nodeDictionary["ClassCode"]);

            //this.LDGraph().SetNamespace("xhv", new Uri("http://www.w3.org/1999/xhtml/vocab#"));
            //this.LDGraph().SetNamespace("skos", new Uri("http://www.w3.org/2004/02/skos/core#"));
            //this.LDGraph().SetNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            //this.LDGraph().CreateTriple(subj, null, "rdf:type", null, "skos:concept", null);
            //this.LDGraph().CreateTriple(subj, null, "xhv:licence", null, "http://creativecommons.org/licenses/by-nc-nd/3.0/", null);
            //this.LDGraph().CreateTriple(subj, null, "skos:inScheme", null, "http://mvcrq.strands.de/rqld/rqkos/rqc-scheme", null);
            //if (nodeDictionary["ClassCode"] != null) this.LDGraph().CreateTriple(subj, null, "skos:notation", null, nodeDictionary["ClassCode"], "http://mvcrq.strands.de/rqld/rqkos/rqc-schema#Notation");
            //if (nodeDictionary["ClassLongTitle"] != null) this.LDGraph().CreateTriple(subj, null, "skos:prefLabel", null, nodeDictionary["ClassLongTitle"], "de");
            //if (nodeDictionary["ClassShortTitle"] != null) this.LDGraph().CreateTriple(subj, null, "skos:altLabel", null, nodeDictionary["ClassShortTitle"], "de");
            //if (nodeDictionary["Broader"] != null) this.LDGraph().CreateTriple(subj, null, "skos:broader", null, "http://mvcrq.strands.de/rqld/rqkos/rqc_" + nodeDictionary["Broader"], null);
        }

    }
}