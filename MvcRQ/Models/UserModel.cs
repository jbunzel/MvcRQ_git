using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcRQ.Models
{
    public class User
    {
        public virtual int UserId { get; set; }
        public virtual int SortOptionId { get; set; }
        public virtual int UserDBId { get; set; }
        public virtual List<UserDB> UserDBs { get; set; } 
        // User Interface Preferences 
        public virtual bool UseExternalDBs { get; set; }
        public virtual SortOption DfltSortOption { get; set; }
    }

    public class SortOption
    {
        public virtual int SortOptionId { get; set; }
        public virtual string Name { get; set; }
        public virtual string XsltTransform { get; set; }
    }

    public class UserDB
    {
        public virtual int UserDBId { get; set; }
        public virtual string Name { get; set; }
        public virtual List<User> Users { get; set; }
    }
}