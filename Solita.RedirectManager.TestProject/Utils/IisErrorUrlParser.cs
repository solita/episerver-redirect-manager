using System;
using System.Web;

namespace Solita.RedirectManager.TestProject.Utils
{
    public static class IisErrorUrlParser
    {
        public static Uri GetOriginalUrl(HttpRequestBase request, int statusCode)
        {
            return GetOriginalUrl(request.Url, statusCode);
        }

        public static Uri GetOriginalUrl(Uri iisHttpErrorUri, int statusCode)
        {
            string iisErrorUrlPrefix = string.Format("?{0};", statusCode);
            string absoluteHost = string.Format($"{0}:{1}:{2}/", iisHttpErrorUri.Scheme, iisHttpErrorUri.Host, iisHttpErrorUri.Port);
            string relativeUrl = iisHttpErrorUri.Query
                .Replace(iisErrorUrlPrefix, "")
                .Replace(absoluteHost, "/"); 
            return new Uri(iisHttpErrorUri, relativeUrl); 
        }
    }
}