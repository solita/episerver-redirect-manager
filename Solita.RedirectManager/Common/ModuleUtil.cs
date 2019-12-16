using EPiServer.Shell;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Framework.Web.Resources;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;
using Solita.RedirectManager.Controllers;

namespace Solita.RedirectManager.Common
{
    public static class ModuleUtil
    {
        public static string PathTo(string relativePath)
        {
            return Paths.ToResource(typeof (RedirectManagerController), relativePath.TrimStart('~', '/'));
        }

        public static IEnumerable<ModuleViewModel> GetModuleSettings()
        {
            // Adapted from EPiServer.Shell.UI.Bootstrapper.CreateViewModel
            // Creates a data structure that contains module resource paths (JS and CSS) and settings.

            var modules = ServiceLocator.Current.GetInstance<ModuleTable>();
            var resourceService = ServiceLocator.Current.GetInstance<IClientResourceService>();

            var moduleList = modules.GetModules()
                .Select(m => m.CreateViewModel(modules, resourceService))
                .OrderBy(mv => mv.ModuleDependencies != null ? mv.ModuleDependencies.Count : 0)
                .ToList();

            return moduleList;
        }
    }
}