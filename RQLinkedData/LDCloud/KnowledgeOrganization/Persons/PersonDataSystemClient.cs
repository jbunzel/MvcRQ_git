using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using RQLinkedData.LDCloud;

namespace RQLinkedData.LDCloud.KnowledgeOrganization.Persons
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für ClassificationSystemClient
    /// </summary>
    public class PersonDataSystemClient
        : LinkedDataClient
    {
        #region public members

        public enum PersonDataSystems
        {
            gnd,
            wikipedia,
            rq,
            unknown
        }
        
        public enum PersonDataPredicates
        {
            preferred_label,
            date_of_birth,
            place_of_birth,
            date_of_death,
            place_of_death,
            profession_or_occupation,
            familial_relationship,
            subject_of_occupation,
            geographic_area,
            gender,
            language,
            license,
            version_of,
            type,
            unknown,
        }

        #endregion

        #region public constructors

        public PersonDataSystemClient()
        {
        }

        #endregion

        #region public static methods

        static public string GetURI(PersonDataSystems personDataSystem, string personCode)
        {
            switch (personDataSystem)
            {
                case PersonDataSystems.gnd:
                    return GndPersonDataSystemClient.GetURI(personCode);
                case PersonDataSystems.rq:
                    return RqPersonDataSystemClient.GetURI(personCode);
                case PersonDataSystems.wikipedia:
                    return null;
                default:
                    return "";
            }
        }
        
        static public string GetPredicateURI(PersonDataSystems personDataSystem, PersonDataPredicates predicate)
        {
            switch (personDataSystem)
            {
                case PersonDataSystems.gnd:
                    return GndPersonDataSystemClient.GetPredicateURI(predicate);
                case PersonDataSystems.rq:
                    return RqPersonDataSystemClient.GetPredicateURI(predicate);
                case PersonDataSystems.wikipedia:
                    return null;
                default:
                    return "";
            }
        }

        static public string GetPredicateText(PersonDataPredicates predicate)
        {
            switch (predicate)
            {
                case PersonDataPredicates.preferred_label:
                    return "Bezeichnung:";
                case PersonDataPredicates.date_of_birth:
                    return "Geburtsdatum";
                case PersonDataPredicates.date_of_death:
                    return "Sterbedatum";
                case PersonDataPredicates.familial_relationship:
                    return "Familiäre Beziehungen";
                case PersonDataPredicates.gender:
                    return "Geschlecht";
                case PersonDataPredicates.geographic_area:
                    return "Geographischer Bezug";
                case PersonDataPredicates.language:
                    return "Sprache";
                case PersonDataPredicates.license:
                    return "Daten-Lizenz";
                case PersonDataPredicates.place_of_birth:
                    return "Geburtsort";
                case PersonDataPredicates.place_of_death:
                    return "Sterbeort";
                case PersonDataPredicates.profession_or_occupation:
                    return "Profession";
                case PersonDataPredicates.subject_of_occupation:
                    return "Klassifikation";
                case PersonDataPredicates.type:
                    return GetPredicateURI(PersonDataSystems.gnd, predicate);
                case PersonDataPredicates.version_of:
                    return GetPredicateURI(PersonDataSystems.gnd, predicate);
                default:
                    return GetPredicateURI(PersonDataSystems.gnd, predicate);
            }
        }

        static public PersonDataPredicates GetPredicate(PersonDataSystems personDataSystem, string predicateURI)
        {
            switch (personDataSystem)
            {
                case PersonDataSystems.gnd:
                    return GndPersonDataSystemClient.GetPredicate(predicateURI);
                case PersonDataSystems.rq:
                    return RqPersonDataSystemClient.GetPredicate(personDataSystem,predicateURI); //Methode in RqPersonDataSystemClient AUSBESSERN
                case PersonDataSystems.wikipedia:
                    return PersonDataPredicates.unknown;
                default:
                    return PersonDataPredicates.unknown;
            }
        }

        static public string AdaptPersonCode(PersonDataSystems personDataSystem, string personCode)
        {
            switch (personDataSystem)
            {
                case PersonDataSystems.gnd:
                    return GndPersonDataSystemClient.AdaptClassNotation(personCode);
                case PersonDataSystems.rq:
                    return RqPersonDataSystemClient.AdaptClassNotation(personCode);
                case PersonDataSystems.wikipedia:
                    return null;
                default:
                    return personCode;
            }
        }

        #endregion

        #region public overridable methods

        public virtual string GetPreferredLabel(string personID)
        {
            return null;
        }

        public virtual string[] GetPredicates(string personId)
        {
            return null;
        }

        public virtual Dictionary<string, string> GetPredicateObjects(string personId)
        {
            return null;
        }

        #endregion
    }
}