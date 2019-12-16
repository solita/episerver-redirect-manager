using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using EPiServer.ServiceLocation;
using Solita.RedirectManager.Models.Interfaces;

namespace Solita.RedirectManager.Models.EF
{
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Transient, ServiceType = typeof(IRewriterContext))]
    public class RewriterContext : DbContext, IRewriterContext
    {
        public IDbSet<RewritemappingModel> Rewritemappings { get; set; }

        public RewriterContext() : base("EPiServerDB")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                var updateInfo = entry.Entity as IUpdateInfo;
                if (updateInfo != null && (entry.State == EntityState.Added || entry.State == EntityState.Modified))
                {
                    updateInfo.UpdatedAt = DateTime.Now;
                    if (entry.State == EntityState.Added)
                    {
                        updateInfo.CreatedAt = DateTime.Now;
                    }
                }
            }

            return base.SaveChanges();
        }
    }
}