using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc5RQ.Areas.Desktop.Models
{
    public class DesktopViewModel
    {
        public IDictionary<string, string> ProjectTitles {get; set; }
        public IDictionary<string, string> BoxTitles { get; set; }
    }
    
    public class Project
    {
        public string Title { get; set; }
        public IList<Note> Listing { get; set; }
    }

    public class Note
    {
        public string Title { get; set; }
        public string Content { get; set; } 
    }

    public class Notebox
    {
        public IList<Note> Listing { get; set; }
    }
}