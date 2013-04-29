using System.Collections.Generic;
using Orchard;
using Orchard.Security.Permissions;
using Szmyd.Orchard.Modules.Menu.Models;

namespace Szmyd.Orchard.Modules.Menu.Providers
{
    internal class NavigationProvider : AbstractNavigationProvider {
        internal NavigationProvider(IEnumerable<IPermissionProvider> permissionProviders, IOrchardServices orchardServices) : base(permissionProviders, orchardServices) {
        }

        public override string MenuName { get; internal set; }
        public override IEnumerable<AdvancedMenuItemPart> Items {
            get; internal set; 
        }
    }
}