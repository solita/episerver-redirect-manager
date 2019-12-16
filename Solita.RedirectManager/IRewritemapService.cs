using System.Collections.Generic;
using Solita.RedirectManager.Models.EF;

namespace Solita.RedirectManager
{
    public interface IRewritemapService
    {
        IEnumerable<RewritemappingModel> ListMappings();
        RewritemappingModel GetMapping(int id);
        RewritemappingModel GetMapping(string host, string path);
        void Save(IEnumerable<RewritemappingModel> mappings);
        void Save(RewritemappingModel rewritemappingModel);
        void Delete(int id);

        string FindRedirectUrl(string host, string pathAndQuery, bool updateLastUsed = false);
    }
}
