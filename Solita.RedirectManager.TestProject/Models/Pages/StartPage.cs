using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace Solita.RedirectManager.TestProject.Models.Pages
{
    [ContentType
        (DisplayName = "start page",
        GUID = "32660A34-5DD0-4AB1-94F4-0A826A146DA5")
        ]
    public class StartPage : PageData
    {
        public virtual XhtmlString PageContent { get; set; }
    }
}