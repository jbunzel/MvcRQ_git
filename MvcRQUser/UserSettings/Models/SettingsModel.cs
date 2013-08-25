using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcRQUser.Models
{
    public class QueryOptions
    {
        public virtual int QueryOptionsId { get; set; }
        public virtual Guid UserId { get; set; }                                                                // There is a one-to-one relationship between user settings and users. 
        public virtual int SortOptionId { get; set; }                                                           // Default sort option. 
        public virtual SortOption SortOption { get; set; }                                                      // Navigational property.
        public virtual bool IncludeExternal { get; set; }                                                       // If true, external databases are included in searches. 
        public virtual ICollection<Database> Databases { get; set; }                                            // External databases to be included in searches.
        public virtual ICollection<DataField> DataFields { get; set; }                                          // Default search fields.
    }

    public class SortOption
    {
        public virtual int SortOptionId { get; set; }
        public virtual string Name { get; set; }
        public virtual string XsltTransform { get; set; }
    }

    public class Database
    {
        public virtual int DatabaseId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Uri { get; set; }
        public virtual ICollection<DataField> DataFields { get; set; }
    }

    public class DataField
    {
        public virtual int DataFieldId { get; set; }
        public virtual string Name { get; set; }
    }
}