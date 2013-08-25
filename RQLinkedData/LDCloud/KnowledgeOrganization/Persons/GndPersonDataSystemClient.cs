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
    public class GndPersonDataSystemClient : PersonDataSystemClient
    {
        #region public static methods

        static public string GetURI(string personCode)
        {
            //return (personCode != "") ? "http://d-nb.info/gnd/" + PersonDataSystemClient.AdaptPersonCode(PersonDataSystems.gnd, personCode) + "/about/rdf" : "http://d-nb.info/gnd";
            return (personCode != "") ? "http://d-nb.info/gnd/" + PersonDataSystemClient.AdaptPersonCode(PersonDataSystems.gnd, personCode) : "http://d-nb.info/gnd";
        }

        static public string GetPredicateURI(PersonDataPredicates predicate)
        {
            return GetPredicateURI(PersonDataSystems.gnd, predicate);
        }

        static public string AdaptClassNotation(string personCode)
        {
            return personCode;
        }

        #endregion

        #region public constructors

        public GndPersonDataSystemClient()
            : base()
        {
        }

        #endregion

        #region public method overrides

        public override void Load(System.Collections.Specialized.StringDictionary nodeDictionary)
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

        #endregion

        #region public methods

        public string[] GetPredicates(string PersonId)
        {
            try
            {
                string[] res;

                res = this.LDGraph().PredicateOf(PersonId);
                for (var i = 0; i < res.Count(); i++ )
                    res[i] = GetPredicateText(GetPredicate(res[i]));
                return res;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}