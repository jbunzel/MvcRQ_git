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

        //static public string GetPredicate(PersonDataPredicates predicate)
        //{
        //    return GetPredicateURI(PersonDataSystems.gnd, predicate);
        //}

        static public string GetPredicateURI(PersonDataPredicates predicate)
        {
            switch (predicate)
            {
                case PersonDataPredicates.preferred_label:
                    return "http://d-nb.info/standards/elementset/gnd#preferredNameForThePerson";
                case PersonDataPredicates.date_of_birth:
                    return "http://d-nb.info/standards/elementset/gnd#dateOfBirth";
                case PersonDataPredicates.date_of_death:
                    return "http://d-nb.info/standards/elementset/gnd#dateOfDeath";
                case PersonDataPredicates.familial_relationship:
                    return "http://d-nb.info/standards/elementset/gnd#familialRelationship";
                case PersonDataPredicates.gender:
                    return "http://d-nb.info/standards/elementset/gnd#gender";
                case PersonDataPredicates.geographic_area:
                    return "http://d-nb.info/standards/elementset/gnd#geographicAreaCode";
                case PersonDataPredicates.language:
                    return "http://d-nb.info/standards/elementset/gnd#languageCode";
                case PersonDataPredicates.license:
                    return "";
                case PersonDataPredicates.place_of_birth:
                    return "http://d-nb.info/standards/elementset/gnd#placeOfBirth";
                case PersonDataPredicates.place_of_death:
                    return "http://d-nb.info/standards/elementset/gnd#placeOfDeath";
                case PersonDataPredicates.profession_or_occupation:
                    return "http://d-nb.info/standards/elementset/gnd#professionOrOccupation";
                case PersonDataPredicates.subject_of_occupation:
                    return "http://d-nb.info/standards/elementset/gnd#gndSubjectCategory";
                case PersonDataPredicates.type:
                    return "";
                case PersonDataPredicates.version_of:
                    return "";
                default:
                    return "";
            }
        }

        static public PersonDataPredicates GetPredicate(string predicateURI)
        {
            switch (predicateURI)
            {
                case "http://d-nb.info/standards/elementset/gnd#preferredNameForThePerson":
                    return PersonDataPredicates.preferred_label;
                case "http://d-nb.info/standards/elementset/gnd#dateOfBirth":
                    return PersonDataPredicates.date_of_birth;
                case "http://d-nb.info/standards/elementset/gnd#dateOfDeath":
                    return PersonDataPredicates.date_of_death;
                case "http://d-nb.info/standards/elementset/gnd#familialRelationship":
                    return PersonDataPredicates.familial_relationship;
                case "http://d-nb.info/standards/elementset/gnd#gender":
                    return PersonDataPredicates.gender;
                case "http://d-nb.info/standards/elementset/gnd#geographicAreaCode":
                    return PersonDataPredicates.geographic_area;
                case "http://d-nb.info/standards/elementset/gnd#languageCode":
                    return PersonDataPredicates.language;
                case "http://d-nb.info/standards/elementset/gnd#placeOfBirth":
                    return PersonDataPredicates.place_of_birth;
                case "http://d-nb.info/standards/elementset/gnd#placeOfDeath":
                    return PersonDataPredicates.place_of_death;
                case "http://d-nb.info/standards/elementset/gnd#professionOrOccupation":
                    return PersonDataPredicates.profession_or_occupation;
                case "http://d-nb.info/standards/elementset/gnd#gndSubjectCategory":
                    return PersonDataPredicates.subject_of_occupation;
                default:
                    return PersonDataPredicates.unknown;
            }
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

        public override string GetPreferredLabel(string personID)
        {
            try
            {
                string[] res;

                //DdcReasoner.rs.Apply(this.LDGraph());
                res = this.LDGraph().ObjectOf(personID, GndPersonDataSystemClient.GetPredicateURI(PersonDataPredicates.preferred_label));
                return res[0];
            }
            catch
            {
                return "";
            }
        }

        public override string[] GetPredicates(string personId)
        {
            try
            {
                string[] pr;
                List<string> res = new List<string>();

                pr = this.LDGraph().PredicateOf(personId);
                for (var i = 0; i < pr.Count(); i++)
                {
                    string pt = GetPredicateText(GetPredicate(pr[i]));

                    if (! string.IsNullOrEmpty(pt))
                        res.Add(pt);
                }
                return res.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public override Dictionary<string, string> GetPredicateObjects(string personId)
        {
            try
            {
                string[] pr;
                var res = new Dictionary<string, string>();

                pr = this.LDGraph().PredicateOf(personId);
                for (var i = 0; i < pr.Count(); i++)
                {
                    PersonDataPredicates p = GetPredicate(pr[i]);
                    string pt = GetPredicateText(p);

                    if (!string.IsNullOrEmpty(pt))
                    {
                        string[] tr = this.LDGraph().ObjectOf(personId, GetPredicateURI(p));

                        for (var j = 0; j < tr.Count(); j++)
                            if (!string.IsNullOrEmpty(tr[j]))
                            {
                                res.Add(pt, tr[j]);
                                break;
                            }
                    }
                }
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