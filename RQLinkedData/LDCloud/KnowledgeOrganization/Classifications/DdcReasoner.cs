using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Inference;


namespace RQLinkedData.LDCloud.KnowledgeOrganization.Classifications
{
    static class DdcReasoner
    {
        static public SimpleN3RulesReasoner rs = null;
            
        static private Graph LoadN3(string path)
        {
            var g = new Graph();
            var parser = new Notation3Parser();

            try
            {
                parser.Load(g, path);
                return g;
            }
            catch 
            {
                return null;
            }
        }

        static public void Init()
        {
            if (rs == null)
            {
                rs = new VDS.RDF.Query.Inference.SimpleN3RulesReasoner();
                var ruleGraph = LoadN3(AppDomain.CurrentDomain.BaseDirectory + "LinkedData/N3Rules/test.n3");

                rs.Initialise(ruleGraph);
            }
        }
    }
}
