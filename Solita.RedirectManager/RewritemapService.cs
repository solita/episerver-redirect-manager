using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.ServiceLocation;
using Solita.RedirectManager.Models.EF;
using Solita.RedirectManager.Models.Interfaces;

namespace Solita.RedirectManager
{
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Transient, ServiceType = typeof(IRewritemapService))]
    public class RewritemapService : IRewritemapService
    {
        private readonly IRewriterContext _context;

        public RewritemapService(IRewriterContext context)
        {
            _context = context;
        }

        public IEnumerable<RewritemappingModel> ListMappings()
        {
            return _context.Rewritemappings.OrderByDescending(s => s.CreatedAt).ToList();
        }

        public RewritemappingModel GetMapping(int id)
        {
            return _context.Rewritemappings.FirstOrDefault(m => m.Id == id);
        }

        public RewritemappingModel GetMapping(string host, string path)
        {
            var slashed = EnsureTrailingSlashForPath(EnsureLeadingSlash(path));

            return
                _context.Rewritemappings.FirstOrDefault(m =>
                    (m.RequestHost == null && host == null ||
                     m.RequestHost != null && m.RequestHost.Equals(host, StringComparison.InvariantCultureIgnoreCase)) &&
                    m.RequestPath.Equals(slashed, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Save(IEnumerable<RewritemappingModel> mappings)
        {
            foreach (var rewritemappingModel in mappings)
            {
                rewritemappingModel.RequestPath = EnsureLeadingSlash(rewritemappingModel.RequestPath);
                rewritemappingModel.RequestPath = EnsureTrailingSlashForPath(rewritemappingModel.RequestPath);

                _context.Rewritemappings.Add(rewritemappingModel);
            }

            _context.SaveChanges();
        }

        public void Save(RewritemappingModel rewritemappingModel)
        {
            rewritemappingModel.RequestPath = EnsureLeadingSlash(rewritemappingModel.RequestPath);
            rewritemappingModel.RequestPath = EnsureTrailingSlashForPath(rewritemappingModel.RequestPath);

            _context.Rewritemappings.Add(rewritemappingModel);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var mapping = _context.Rewritemappings.Find(id);
            _context.Rewritemappings.Remove(mapping);
            _context.SaveChanges();
        }

        public string FindRedirectUrl(string host, string pathAndQuery, bool updateLastUsed = false)
        {
            pathAndQuery = EnsureLeadingSlash(pathAndQuery);

            var mapping = FindMapping(host, pathAndQuery);
            if (mapping == null)
            {
                return null;
            }

            var redirectUrl = mapping.RedirectUrl;

            if (mapping.PreservePath && mapping.RequestPath.Length <= pathAndQuery.Length)
            {
                var remainingPath = pathAndQuery.Substring(mapping.RequestPath.Length);
                redirectUrl += remainingPath;
            }

            if (updateLastUsed)
            {
                mapping.LastUsed = DateTime.Now;
                _context.SaveChanges();
            }

            return redirectUrl;
        }

        private static string EnsureTrailingSlashForPath(string pathAndQuery)
        {
            var pathUrl = new UrlBuilder(pathAndQuery);
            if (!pathUrl.Path.EndsWith("/") && !pathUrl.Path.Contains("."))
            {
                pathUrl.Path += "/";
                pathAndQuery = pathUrl.ToString();
            }

            return pathAndQuery;
        }

        private static string EnsureLeadingSlash(string pathAndQuery)
        {
            return pathAndQuery.StartsWith("/")
                ? pathAndQuery
                : "/" + pathAndQuery;
        }

        private RewritemappingModel FindMapping(string host, string pathAndQuery)
        {
            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(pathAndQuery))
            {
                return null;
            }

            var slashedPath = EnsureTrailingSlashForPath(pathAndQuery);

            return
                _context.Rewritemappings
                    .Where(x => !string.IsNullOrEmpty(x.RequestPath))
                    .Where(x => string.IsNullOrEmpty(x.RequestHost) ||
                                x.RequestHost.Equals(host, StringComparison.InvariantCultureIgnoreCase))
                    .Where(x => x.IsWildcard
                        ? slashedPath.ToLower().StartsWith(x.RequestPath.ToLower()) // StringComparison not supported by EF here
                        : slashedPath.Equals(x.RequestPath, StringComparison.InvariantCulture))
                    .OrderBy(x => x.RequestHost == null) // true > false, hostless ones are last
                    .ThenByDescending(x => x.RequestPath.Length)
                    .FirstOrDefault();
        }
    }
}