﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.34209
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mvc5RQ.Areas.DataManagement.Resources {
    using System;
    
    
    /// <summary>
    ///   Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    /// </summary>
    // Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    // -Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    // Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    // mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class DataManagement {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal DataManagement() {
        }
        
        /// <summary>
        ///   Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Mvc5RQ.Areas.DataManagement.Resources.DataManagement", typeof(DataManagement).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        ///   Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Refresh Bookmarks index ähnelt.
        /// </summary>
        public static string dm_bookmarks_reindex {
            get {
                return ResourceManager.GetString("dm_bookmarks_reindex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Saves the old Bookmarks index and creates a new one. ähnelt.
        /// </summary>
        public static string dm_bookmarks_reindex_title {
            get {
                return ResourceManager.GetString("dm_bookmarks_reindex_title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Delete Lucene index ähnelt.
        /// </summary>
        public static string dm_lucene_delete {
            get {
                return ResourceManager.GetString("dm_lucene_delete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Deletes the Lucene index. ähnelt.
        /// </summary>
        public static string dm_lucene_delete_title {
            get {
                return ResourceManager.GetString("dm_lucene_delete_title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Update Lucene index ähnelt.
        /// </summary>
        public static string dm_lucene_index {
            get {
                return ResourceManager.GetString("dm_lucene_index", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Updates the Lucene index without deleting the present one. ähnelt.
        /// </summary>
        public static string dm_lucene_index_title {
            get {
                return ResourceManager.GetString("dm_lucene_index_title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Optimize Lucene index ähnelt.
        /// </summary>
        public static string dm_lucene_optimize {
            get {
                return ResourceManager.GetString("dm_lucene_optimize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Optimizes Lucene index segments. ähnelt.
        /// </summary>
        public static string dm_lucene_optimize_title {
            get {
                return ResourceManager.GetString("dm_lucene_optimize_title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Refresh Lucene index ähnelt.
        /// </summary>
        public static string dm_lucene_reindex {
            get {
                return ResourceManager.GetString("dm_lucene_reindex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Deletes and recreates the Lucene index. ähnelt.
        /// </summary>
        public static string dm_lucene_reindex_title {
            get {
                return ResourceManager.GetString("dm_lucene_reindex_title", resourceCulture);
            }
        }
    }
}
