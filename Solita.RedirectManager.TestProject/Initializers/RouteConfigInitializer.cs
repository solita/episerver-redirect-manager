using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace Solita.RedirectManager.TestProject.Initializers
{
    [ModuleDependency(typeof (EPiServer.Web.InitializationModule))]
    public class RouteConfigInitializer : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
           RouteTable.Routes.MapMvcAttributeRoutes();
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}