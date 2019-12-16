using System;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using EPiServer.Logging.Compatibility;
using Solita.RedirectManager.Migrations;

namespace Solita.RedirectManager.Initialization
{
    public static class Migrations
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Migrations));
        private static bool _isInitialized = false;
        private static Object _lock = new Object();

        public static void Initialize()
        {
            if ( _isInitialized ) return;

            lock (_lock)
            {
                if ( _isInitialized ) return;

                try
                {
                    var assembly = Assembly.GetAssembly(typeof (Migrations));
                    var migrator =
                        new DbMigrator(new Configuration
                        {
                            MigrationsAssembly = assembly,
                            MigrationsNamespace = "Solita.RedirectManager.Migrations"
                        });

                    var list = migrator.GetPendingMigrations();
                    Log.DebugFormat("Migrations are pending " + list.Count().ToString( CultureInfo.InvariantCulture ) + " items." );
                    migrator.Update();
                }
                catch ( Exception e )
                {
                    Log.Error("Migrations failed!", e);
                }

                _isInitialized = true;
            }
        }
    }
}