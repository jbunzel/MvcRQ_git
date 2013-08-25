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
    public class ClassificationSystemClient
        : RQLinkedData.LDCloud.LinkedDataClient
    {
        public enum ClassificationSystems
        {
            ddc,
            rvk,
            jel,
            rq,
            oldrq,
            unknown
        }
        
        public enum ClassificationPredicates
        {
            alternative_label,
            broader_term,
            class_notation,
            language,
            license,
            narrower_term,
            preferred_label,
            version_of,
            type,
        }

        static public string GetURI(ClassificationSystems classSystem, string classNotation)
        {
            switch (classSystem)
            {
                case ClassificationSystems.rvk:
                    return RvkClassificationSystemClient.GetURI(classNotation);
                case ClassificationSystems.jel:
                    return JelClassificationSystemClient.GetURI(classNotation);
                case ClassificationSystems.rq:
                    return RqClassificationSystemClient.GetURI(classNotation);
                case ClassificationSystems.ddc:
                    return DdcClassificationSystemClient.GetURI(classNotation);
                default:
                    return "";
            }
        }
        
        static public string GetPredicate(ClassificationSystems classSystem, ClassificationPredicates predicate)
        {
            switch (predicate)
            {
                case ClassificationPredicates.type:
                    return "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";
                case ClassificationPredicates.preferred_label:
                    return "http://www.w3.org/2004/02/skos/core#prefLabel";
                case ClassificationPredicates.alternative_label:
                    return "http://www.w3.org/2004/02/skos/core#altLabel";
                case ClassificationPredicates.broader_term:
                    return "http://www.w3.org/2004/02/skos/core#broader";
                case ClassificationPredicates.narrower_term:
                    return "http://www.w3.org/2004/02/skos/core#narrower";
                case ClassificationPredicates.class_notation:
                    return "http://www.w3.org/2004/02/skos/core#notation";
                case ClassificationPredicates.language:
                    return "http://purl.org/dc/terms/language";
                default:
                    return "";
            }
        }
        
        static public string AdaptClassNotation(ClassificationSystems classSystem, string classNotation)
        {
            switch (classSystem)
            {
                case ClassificationSystems.rvk:
                    return RvkClassificationSystemClient.AdaptClassNotation(classNotation);
                case ClassificationSystems.jel:
                    return JelClassificationSystemClient.AdaptClassNotation(classNotation);
                case ClassificationSystems.rq:
                    return RqClassificationSystemClient.AdaptClassNotation(classNotation);
                case ClassificationSystems.ddc:
                    return DdcClassificationSystemClient.AdaptClassNotation(classNotation);
                default:
                    return classNotation;
            }
        }

        public ClassificationSystemClient()
        {
            
        }

        public virtual string GetPreferredLabel(string classNotation)
        {
            return "";
        }
    }
}