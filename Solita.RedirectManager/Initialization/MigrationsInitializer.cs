using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace Solita.RedirectManager.Initialization
{
    [InitializableModule]
    public class MigrationsInitializer : IInitializableModule
    {

        public void Initialize(InitializationEngine context)
        {
            Migrations.Initialize();
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}