using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Security;
using EPiServer.Shell.Navigation;
using Solita.RedirectManager.Common;
using Solita.RedirectManager.Lang;

namespace Solita.RedirectManager
{
    /// <summary>
    /// Creates a menu item which is shown only to users with access to the Redirect Manager
    /// </summary>
    [MenuProvider]
    public class MenuProvider : IMenuProvider
    {
        private const string Path = "/global/cms/redirectmanager";
        private readonly string _url;

        public MenuProvider(RequestContext requestContext)
        {
            _url = ModuleUtil.PathTo("RedirectManager");
        }

        public IEnumerable<MenuItem> GetMenuItems()
        {
            // RouteValueDictionary needs to contain the controller, otherwise epi cant identify the active site
            List<MenuItem> menuItemList = new List<MenuItem>();
            var newMenuItem = new UrlMenuItem(CmsMenuTitleProvider.CmsMenuTitle, Path, _url);
            newMenuItem.IsAvailable = CheckAccess;
            menuItemList.Add(newMenuItem);
            
            return menuItemList;
        }

        protected bool CheckAccess(RequestContext requestContext)
        {
            if (PrincipalInfo.Current != null)
                return PrincipalInfo.Current.HasPathAccess(_url);
            return false;
        }
    }
}