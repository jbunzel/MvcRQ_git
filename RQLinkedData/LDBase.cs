using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace RQLinkedData
{
    public class LDBase : Graph
    {
        public LDBase()
            : base() 
        {
        }

        public void SetNamespace(string name, Uri uri)
        {
            this.NamespaceMap.AddNamespace(name, uri);
        }

        public void CreateTriple(string subj, string subjlit, string pred, string predlit, string obj, string objlit)
        {
            INode s;
            INode p;
            INode o;

            if (subj.StartsWith("http://"))
                s = this.CreateUriNode(new Uri(subj));
            else if (subjlit == null)
                s = this.CreateUriNode(subj);
            else if (subjlit.StartsWith("http://"))
                s = this.CreateLiteralNode(subj, new Uri(subjlit));
            else
                s = this.CreateLiteralNode(subj, subjlit);
            
            if (pred.StartsWith("http://"))
                p = this.CreateUriNode(new Uri(pred));
            else if (predlit == null)
                p = this.CreateUriNode(pred);
            else if (predlit.StartsWith("http://"))
                p = this.CreateLiteralNode(pred, new Uri(predlit));
            else
                p = this.CreateLiteralNode(pred, predlit);
            
            if (obj.StartsWith("http://"))
                o = this.CreateUriNode(new Uri(obj));
            else if (objlit == null)
                o = this.CreateUriNode(obj);
            else if (objlit.StartsWith("http://"))
                o = this.CreateLiteralNode(obj, new Uri(objlit));
            else
                o = this.CreateLiteralNode(obj, objlit);
            this.Assert(new Triple(s, p, o));
        }

        public void Load(string subjectUri)
        {
            try
            {
                Options.UriLoaderCaching = false;
                //UriLoader.Load(this, new Uri(subjectUri + "/about.rdf"));
                UriLoader.Load(this, new Uri(subjectUri));
            }
            catch
            { }
        }

        public string[] ObjectOf(string predicateUri)
        {
            string[] x = new string[] { "" };
            int i = 0;

            foreach (var t in GetTriplesWithPredicate(this.CreateUriNode(new Uri(predicateUri))))
            {
                x[i++] = t.Object.ToString();
                Array.Resize<string>(ref x, i + 1);
            }
            return x;
        }
        
        public string[] ObjectOf(string subjectUri, string predicateUri)
        {
            var subject = this.GetUriNode(new Uri(subjectUri));
            if (subject == null)
            {
                this.Load(subjectUri);
                subject = this.GetUriNode(new Uri(subjectUri));
            }
            var predicate = this.CreateUriNode(new Uri(predicateUri));
            var selector = new SubjectHasPropertySelector(subject, predicate);
           
            string[] x = new string[] {""};
            int i = 0;

            foreach (var t in this.GetTriples(selector))
            {
                if (t.Object.GetType() == typeof(LiteralNode))
                {
                    Array.Resize<string>(ref x, 3);
                    switch (((LiteralNode)t.Object).Language)
                    {
                        case "de":
                            x[0] = ((LiteralNode)t.Object).Value;
                            break;
                        case "en":
                            x[1] = ((LiteralNode)t.Object).Value;
                            break;
                        case "fr":
                            x[2] = ((LiteralNode)t.Object).Value;
                            break;
                    }
                }
                else
                {
                    x[i++] = t.Object.ToString();
                    Array.Resize<string>(ref x, i + 1);
                }
            }
            return x;
        }

        public void Write(System.Xml.XmlWriter writer)
        {
            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter(0,false);
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Xml.XmlReader nr;

            rdfxmlwriter.Save(this,tw);
            string str = tw.ToString().Remove(0, @"<?xml version=""1.0"" encoding=""utf-16"">".Length+3);
            nr = System.Xml.XmlNodeReader.Create(new System.IO.MemoryStream(System.Text.Encoding.Unicode.GetBytes(str)));
            writer.WriteNode(nr, true);
        }
    }
}
