using System.Web.Security;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

[InitializableModule]
[ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
public class AdminInitialization : IInitializableModule
{
    public void Initialize(InitializationEngine context)
    {
        var adminRole = "WebAdmins";
        var username = "devadmin";
        var password = "Solita123";
        var email = "admin@localhost.com";

        if (!Roles.RoleExists(adminRole))
        {
            Roles.CreateRole(adminRole);
        }

        if (Membership.GetUser(username) == null)
        {
            var user = Membership.CreateUser(username, password, email);
            user.IsApproved = true;
        }

        Roles.AddUserToRole(username, adminRole);
    }

    public void Uninitialize(InitializationEngine context)
    {
    }

    public void Preload(string[] parameters)
    {

    }
}