using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solita.RedirectManager.Models.EF;

namespace Solita.RedirectManager.Test
{
    [TestClass]
    public class RewritemapServiceTest
    {
        // This can be anything else than www.sample.com or null
        private const string CurrentHost = "localhost";

        private IRewritemapService _rewritemapService;

        [TestInitialize]
        public void Initialize()
        {
            var fakeContext = new FakeRewriterContext();
            _rewritemapService = new RewritemapService(fakeContext);

            var models = new[]
            {
                new RewritemappingModel {RequestPath = "slashless", RedirectUrl = "/newslashless"},
                new RewritemappingModel {RequestPath = "/basic/", RedirectUrl = "/newpath"},
                new RewritemappingModel {RequestPath = "/basic/subpath/", RedirectUrl = "/newsubpath/"},
                new RewritemappingModel {RequestPath = "/wildcard/", RedirectUrl = "/wildcard/", IsWildcard = true},
                new RewritemappingModel {RequestPath = "/wildcard/subpath/", RedirectUrl = "/wildcardsubpath/", IsWildcard = true},
                new RewritemappingModel {RequestPath = "/wildcard/subpath/subsubpath/", RedirectUrl = "/subsubpath/", IsWildcard = true},
                new RewritemappingModel {RequestPath = "/preservepath/", RedirectUrl = "/preservepath/", IsWildcard = true, PreservePath = true},
                new RewritemappingModel {RequestPath = "/preservepath/subpath/", RedirectUrl = "/preservesubpath/", IsWildcard = true, PreservePath = true},

                new RewritemappingModel {RequestHost = "www.sample.com", RequestPath = "slashless", RedirectUrl = "http://www.newsample.com/newslashless"},
                new RewritemappingModel {RequestHost = "www.sample.com", RequestPath = "/basic/", RedirectUrl = "http://www.newsample.com/newpath"},
                new RewritemappingModel {RequestHost = "www.sample.com", RequestPath = "/basic/subpath/", RedirectUrl = "http://www.newsample.com/newsubpath/"},
                new RewritemappingModel {RequestHost = "www.sample.com", RequestPath = "/wildcard/", RedirectUrl = "http://www.newsample.com/wildcard/", IsWildcard = true},
                new RewritemappingModel {RequestHost = "www.sample.com", RequestPath = "/wildcard/subpath/", RedirectUrl = "http://www.newsample.com/wildcardsubpath/", IsWildcard = true},
                new RewritemappingModel {RequestHost = "www.sample.com", RequestPath = "/wildcard/subpath/subsubpath/", RedirectUrl = "http://www.newsample.com/subsubpath/", IsWildcard = true},
                new RewritemappingModel {RequestHost = "www.sample.com", RequestPath = "/preservepath/", RedirectUrl = "http://www.newsample.com/preservepath/", IsWildcard = true, PreservePath = true},
                new RewritemappingModel {RequestHost = "www.sample.com", RequestPath = "/preservepath/subpath/", RedirectUrl = "http://www.newsample.com/preservesubpath/", IsWildcard = true, PreservePath = true},
            };

            _rewritemapService.Save(models);
        }

        [TestMethod]
        public void BasicRedirects()
        {
            Assert.AreEqual("/newpath", _rewritemapService.FindRedirectUrl(CurrentHost, "basic/"));
            Assert.AreEqual("/newsubpath/", _rewritemapService.FindRedirectUrl(CurrentHost, "basic/subpath/"));

            Assert.AreEqual("http://www.newsample.com/newpath", _rewritemapService.FindRedirectUrl("www.sample.com", "basic/"));
            Assert.AreEqual("http://www.newsample.com/newsubpath/", _rewritemapService.FindRedirectUrl("www.sample.com", "basic/subpath/"));
        }

        [TestMethod]
        public void WildcardRedirects()
        {
            Assert.AreEqual("/wildcard/", _rewritemapService.FindRedirectUrl(CurrentHost, "wildcard/"));
            Assert.AreEqual("/wildcard/", _rewritemapService.FindRedirectUrl(CurrentHost, "wildcard/foofoo123"));
            Assert.AreEqual("/wildcardsubpath/", _rewritemapService.FindRedirectUrl(CurrentHost, "/wildcard/subpath/"));
            Assert.AreEqual("/wildcardsubpath/", _rewritemapService.FindRedirectUrl(CurrentHost, "/wildcard/subpath/foofoo123"));
            Assert.AreEqual("/subsubpath/", _rewritemapService.FindRedirectUrl(CurrentHost, "/wildcard/subpath/subsubpath/"));
            Assert.AreEqual("/subsubpath/", _rewritemapService.FindRedirectUrl(CurrentHost, "/wildcard/subpath/subsubpath/foofoo"));

            Assert.AreEqual("http://www.newsample.com/wildcard/", _rewritemapService.FindRedirectUrl("www.sample.com", "wildcard/"));
            Assert.AreEqual("http://www.newsample.com/wildcard/", _rewritemapService.FindRedirectUrl("www.sample.com", "wildcard/foofoo123"));
            Assert.AreEqual("http://www.newsample.com/wildcardsubpath/", _rewritemapService.FindRedirectUrl("www.sample.com", "wildcard/subpath/"));
            Assert.AreEqual("http://www.newsample.com/wildcardsubpath/", _rewritemapService.FindRedirectUrl("www.sample.com", "wildcard/subpath/foofoo123"));
            Assert.AreEqual("http://www.newsample.com/subsubpath/", _rewritemapService.FindRedirectUrl("www.sample.com", "wildcard/subpath/subsubpath/"));
            Assert.AreEqual("http://www.newsample.com/subsubpath/", _rewritemapService.FindRedirectUrl("www.sample.com", "wildcard/subpath/subsubpath/foofoo"));
        }

        [TestMethod]
        public void PreservePathRedirects()
        {
            Assert.AreEqual("/preservepath/", _rewritemapService.FindRedirectUrl(CurrentHost, "preservepath/"));
            Assert.AreEqual("/preservepath/foofoo", _rewritemapService.FindRedirectUrl(CurrentHost, "preservepath/foofoo"));
            Assert.AreEqual("/preservepath/foofoo/", _rewritemapService.FindRedirectUrl(CurrentHost, "preservepath/foofoo/"));
            Assert.AreEqual("/preservepath/foofoo?query=123", _rewritemapService.FindRedirectUrl(CurrentHost, "preservepath/foofoo?query=123"));

            Assert.AreEqual("http://www.newsample.com/preservepath/", _rewritemapService.FindRedirectUrl("www.sample.com", "preservepath/"));
            Assert.AreEqual("http://www.newsample.com/preservepath/foofoo", _rewritemapService.FindRedirectUrl("www.sample.com", "preservepath/foofoo"));
            Assert.AreEqual("http://www.newsample.com/preservepath/foofoo/", _rewritemapService.FindRedirectUrl("www.sample.com", "preservepath/foofoo/"));
            Assert.AreEqual("http://www.newsample.com/preservepath/foofoo?query=123", _rewritemapService.FindRedirectUrl("www.sample.com", "preservepath/foofoo?query=123"));
        }

        [TestMethod]
        public void RequestPathSlashies()
        {
            var mappings = _rewritemapService.ListMappings().Where(m => m.RequestPath.Contains("slashless")).ToList();
            Assert.AreEqual(2, mappings.Count);

            var mapping = mappings.First();
            Assert.AreEqual("/slashless/",  mapping.RequestPath);

            Assert.AreEqual("/newslashless", _rewritemapService.FindRedirectUrl(CurrentHost, "slashless"));
            Assert.AreEqual("/newslashless", _rewritemapService.FindRedirectUrl(CurrentHost, "/slashless"));
            Assert.AreEqual("/newslashless", _rewritemapService.FindRedirectUrl(CurrentHost, "/slashless/"));
            Assert.AreEqual("http://www.newsample.com/newslashless", _rewritemapService.FindRedirectUrl("www.sample.com", "slashless"));
            Assert.AreEqual("http://www.newsample.com/newslashless", _rewritemapService.FindRedirectUrl("www.sample.com", "/slashless"));
            Assert.AreEqual("http://www.newsample.com/newslashless", _rewritemapService.FindRedirectUrl("www.sample.com", "/slashless/"));
        }

        [TestMethod]
        public void WildcardPathSlashies()
        {
            Assert.AreEqual("/wildcard/", _rewritemapService.FindRedirectUrl(CurrentHost, "wildcard"));
            Assert.AreEqual("http://www.newsample.com/wildcard/", _rewritemapService.FindRedirectUrl("www.sample.com", "wildcard"));
            Assert.AreEqual("http://www.newsample.com/preservepath/", _rewritemapService.FindRedirectUrl("www.sample.com", "preservepath"));
        }

        [TestMethod]
        public void LastUsedUpdated()
        {
            var oldLastUsed = _rewritemapService.GetMapping(null, "basic").LastUsed;
            _rewritemapService.FindRedirectUrl(CurrentHost, "basic/", true);
            var newLastUsed = _rewritemapService.GetMapping(null, "basic").LastUsed;

            Assert.AreNotEqual(oldLastUsed, newLastUsed);
            Assert.IsTrue(DateTime.Now - newLastUsed < TimeSpan.FromSeconds(1));
        }
    }
}
