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
        public enum PersonDataSystems
        {
            gnd,
            wikipedia,
            rq,
            unknown
        }
        
        public enum PersonDataPredicates
        {
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
            switch (predicate)
            {
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

        static public string GetPredicateText(PersonDataPredicates predicate)
        {
            switch (predicate)
            {
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

        static public PersonDataPredicates GetPredicate(string predicateURI)
        {
            switch (predicateURI)
            {
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

        public PersonDataSystemClient()
        {
        }
    }
}