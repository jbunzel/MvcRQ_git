using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Mvc5RQ.Areas.UserSettings.Models
{
    public class SettingsDBContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        //System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<MvcRQ.Models.SettingsDBContext>());

        public DbSet<QueryOptions> QueryOptions { get; set; }

        public DbSet<SortOption> SortOptions { get; set; }

        public DbSet<Database> Databases { get; set; }

        public DbSet<DataField> DataFields { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<QueryOptions>()
                .HasRequired(p => p.SortOption);
            modelBuilder.Entity<QueryOptions>()
                .HasMany(c => c.Databases);
            modelBuilder.Entity<QueryOptions>()
                .HasMany(c => c.DataFields);
            modelBuilder.Entity<Database>()
                .HasMany(c => c.DataFields); 
        }
    }
}
