using System.Web.Mvc;
using EPiServer.Shell.Navigation;
using Solita.RedirectManager.Common;
using Solita.RedirectManager.Lang;
using Solita.RedirectManager.Models;

namespace Solita.RedirectManager.Controllers
{
    public class RedirectManagerController : Controller
    {
        public ActionResult Index()
        {
            var model = new RedirectManagerViewModel();
            return View(ModuleUtil.PathTo("Views/RedirectManager/Index.cshtml"), model);
        }
    }
}