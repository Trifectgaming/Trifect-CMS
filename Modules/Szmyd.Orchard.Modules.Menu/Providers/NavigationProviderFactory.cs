using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.Security.Permissions;
using Orchard.UI.Navigation;
using Szmyd.Orchard.Modules.Menu.Services;

namespace Szmyd.Orchard.Modules.Menu.Providers
{
    [OrchardSuppressDependency("Orchard.Core.Navigation.Services.MainMenuNavigationProvider")]
    public class NavigationProviderFactory : INavigationProviderFactory
    {
        private readonly IEnumerable<IPermissionProvider> _permissionProviders;
        private readonly Work<IAdvancedMenuService> _menuService;
        private readonly IEnumerable<INavigationProvider> _providers;
        private readonly IOrchardServices _orchardServices;

        public NavigationProviderFactory(
            IEnumerable<IPermissionProvider> permissionProviders,
            Work<IAdvancedMenuService> menuService,
            IEnumerable<INavigationProvider> providers,
            IOrchardServices orchardServices)
        {
            _permissionProviders = permissionProviders;
            _menuService = menuService;
            _providers = providers;
            _orchardServices = orchardServices;
        }

        /// <summary>
        /// Gets providers for the dynamically created menus. 
        /// todo: Providers are cached for boosting performance. Cache gets refreshed every time the menus are modified.
        /// </summary>
        public IEnumerable<INavigationProvider> Providers
        {
            get
            {
                foreach (var p in _providers)
                {
                    yield return p;
                }
                foreach (var p in _menuService.Value.GetMenus()
                    .Select(m => new NavigationProvider(_permissionProviders, _orchardServices)
                    {
                        MenuName = m.Name,
                        Items = _menuService.Value.GetMenuItems(m.Name)
                    })) {
                    yield return p;
                }
            }
        }
    }
}