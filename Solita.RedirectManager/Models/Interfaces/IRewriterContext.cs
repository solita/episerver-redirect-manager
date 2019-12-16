using System;
using System.Data.Entity;
using Solita.RedirectManager.Models.EF;

namespace Solita.RedirectManager.Models.Interfaces
{
    public interface IRewriterContext
    {
        int SaveChanges();

        IDbSet<RewritemappingModel> Rewritemappings { get; set; }
    }

}
