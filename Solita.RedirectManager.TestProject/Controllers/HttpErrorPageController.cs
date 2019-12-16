using System.Net;
using System.Web.Mvc;
using EPiServer.ServiceLocation;
using Solita.RedirectManager.TestProject.Utils;

namespace Solita.RedirectManager.TestProject.Controllers
{
    public class HttpErrorPageController : Controller
    {
        [Route("error/http404")]
        public ActionResult Http404()
        {
            var rewritemapService = ServiceLocator.Current.GetInstance<RewritemapService>();

            var originalUrl = IisErrorUrlParser.GetOriginalUrl(System.Web.HttpContext.Current.Request.Url, 404);
            var redirectTo = rewritemapService.FindRedirectUrl(originalUrl.Host, originalUrl.PathAndQuery, true);
            if (redirectTo != null)
            {
                return RedirectPermanent(redirectTo);
            }
            Response.Clear();
            Response.StatusCode = (int) HttpStatusCode.NotFound;
            return Content("404 Not found");
        }
    }
}