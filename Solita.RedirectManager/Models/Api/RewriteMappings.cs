using System.Collections.Generic;
using System.Linq;

namespace Solita.RedirectManager.Models.Api
{
    public class RewriteMappings
    {
        public List<RewritemappingDto> Rewritemappings { get; set; }

        public RewriteMappings()
        {
            Rewritemappings = new List<RewritemappingDto>();
        }

        public RewriteMappings(IEnumerable<RewritemappingDto> rewriteMappings)
        {
            Rewritemappings = rewriteMappings.ToList();
        }
    }
}