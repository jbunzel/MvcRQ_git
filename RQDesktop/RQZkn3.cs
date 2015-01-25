using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.XPath;

namespace RQDesktop
{
    public class RQZkn3
    {
        #region private members

        string _extractPath = System.Web.HttpContext.Current.Server.MapPath("~/xml/zkn");

        #endregion

        #region private methods

        private void ClearDirectory(string path)
        {
            DirectoryInfo zknDirectoryInfo = new DirectoryInfo(path);

            foreach (FileInfo file in zknDirectoryInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in zknDirectoryInfo.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private string CopyXSLTFile(string path)
        {
            DirectoryInfo zknDirectoryInfo = new DirectoryInfo(path.Substring(0, path.LastIndexOf("\\")));
            string result = "";

            foreach (FileInfo file in zknDirectoryInfo.GetFiles("*.xsl"))
            {
                result = _extractPath + "/" + file.Name; 
                file.CopyTo(result, true);
            }
            return result;
        }

        #endregion

        #region public constructors

        public RQZkn3() { }

        #endregion

        #region public methods

        public void Load (string path)
        {
            this.ClearDirectory(_extractPath);
            ZipFile.ExtractToDirectory(path, _extractPath);
        }

        public IDictionary<string, string> ProjectList()
        {
            XPathNavigator projects = new XPathDocument(_extractPath + "/desktop.xml").CreateNavigator();
            XPathNodeIterator nodes = projects.Select("/desktops/desktop/@name");
            Dictionary<string, string> result = new Dictionary<string,string>();

            while (nodes.MoveNext())
                result.Add(nodes.CurrentPosition.ToString(), nodes.Current.ToString());
            return result;
        }

        public IDictionary<string, string> NoteList()
        {
            return null;
        }

        public XmlDocument ConvertProject(string xsltPath, string projectName)
        {
            var xTrf = new System.Xml.Xsl.XslCompiledTransform();
            var xTrfArg = new System.Xml.Xsl.XsltArgumentList();
            var xSet = new System.Xml.Xsl.XsltSettings(true, true);
            var mstr = new System.Xml.XmlTextWriter(new System.IO.MemoryStream(), System.Text.Encoding.UTF8);
            var doc = new System.Xml.XmlDocument();
            
            xTrf.Load(CopyXSLTFile(xsltPath), xSet, new XmlUrlResolver());
            xTrfArg.AddParam("DeskNr", "", projectName);
            xTrf.Transform(_extractPath + "/desktop.xml", xTrfArg, mstr);
            mstr.BaseStream.Flush();
            mstr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            doc.Load(mstr.BaseStream);
            return doc;
        }

        #endregion
    }
}
