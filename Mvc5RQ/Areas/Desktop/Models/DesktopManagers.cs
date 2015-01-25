using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using RQDesktop;

namespace Mvc5RQ.Areas.Desktop.Models
{
    
    
    public interface IDesktopManager
    {
        DesktopViewModel Load();
        void ConvertProject(string projectName);
    }

    public class ZKNDesktopManager : IDesktopManager
    {
        private RQZkn3 _zkn = null;

        public ZKNDesktopManager()
        {
            _zkn = new RQZkn3();
        }
        
        public DesktopViewModel Load()
        {
            _zkn.Load(Mvc5RQ.Areas.UserSettings.Models.DesktopOptions.ZKNFileAdress);
            return new DesktopViewModel() { ProjectTitles = _zkn.ProjectList(), BoxTitles = _zkn.NoteList() };
        }

        public void ConvertProject(string projectName)
        {
            int test; 
            string projectDirectory = "";
            string strandsFilePath = "";

            if (int.TryParse(projectName, out test))
                _zkn.ProjectList().TryGetValue(projectName, out projectDirectory);
            else
                projectDirectory = projectName;
            strandsFilePath = Mvc5RQ.Areas.UserSettings.Models.DesktopOptions.StrandsDirectory + "/" + projectDirectory + "/" + projectDirectory + ".xml";
            System.Xml.XmlDocument doc = _zkn.ConvertProject(HttpContext.Current.Server.MapPath("~/xslt/DesktopTransforms/DESK2STRAND.xsl"), projectName);
            if (System.IO.File.Exists(strandsFilePath)) System.IO.File.Delete(strandsFilePath); 
            doc.Save(strandsFilePath);
        }
    }

    public static class DesktopManagerFactory
    {
        public static ZKNDesktopManager Get<IDesktopManager>()
        {
            return new ZKNDesktopManager();
        }
    }
}