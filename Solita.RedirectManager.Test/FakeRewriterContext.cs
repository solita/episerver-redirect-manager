using System.Data.Entity;
using Solita.RedirectManager.Models.EF;
using Solita.RedirectManager.Models.Interfaces;

namespace Solita.RedirectManager.Test
{
    internal class FakeRewriterContext : IRewriterContext
    {
        public FakeRewriterContext()
        {
            Rewritemappings = new TestDbSet<RewritemappingModel>();
        }

        public int SaveChanges()
        {
            return 1;
        }

        public IDbSet<RewritemappingModel> Rewritemappings { get; set; }
    }
}