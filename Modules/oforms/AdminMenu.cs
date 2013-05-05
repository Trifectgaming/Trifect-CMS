using oforms.Services;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace oforms
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("oforms")
                .Add(T("OForms"), "1.5", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Action("Index", "Admin", new { area = "oforms" }).Permission(StandardPermissions.SiteOwner);
        }
    }
}