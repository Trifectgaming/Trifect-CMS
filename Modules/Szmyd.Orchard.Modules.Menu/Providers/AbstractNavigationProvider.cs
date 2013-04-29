using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Localization;
using Orchard.Security.Permissions;
using Orchard.UI.Navigation;
using Szmyd.Orchard.Modules.Menu.Models;
using Szmyd.Orchard.Modules.Menu.Utilities;

namespace Szmyd.Orchard.Modules.Menu.Providers
{
    /// <summary>
    /// Abstract class for building hierarchical menus
    /// </summary>
    internal abstract class AbstractNavigationProvider : INavigationProvider
    {
        private readonly IEnumerable<IPermissionProvider> _permissionProviders;
        private readonly IOrchardServices _orchardServices;

        internal AbstractNavigationProvider(IEnumerable<IPermissionProvider> permissionProviders, IOrchardServices orchardServices) {
            _permissionProviders = permissionProviders;
            _orchardServices = orchardServices;
        }

        #region Implementation of INavigationProvider

        public abstract string MenuName { get; internal set; }

        public virtual void GetNavigation(NavigationBuilder builder) {
            BuildHierarchy(_permissionProviders.SelectMany(p => p.GetPermissions()).ToList(), Items, builder);
        }

        #endregion

        public abstract IEnumerable<AdvancedMenuItemPart> Items { get; internal set; }

        private void BuildHierarchy(IList<Permission> permissions, IEnumerable<AdvancedMenuItemPart> items, NavigationBuilder builder, int level = 1)
        {
            // Filtering item collection to only the current level
            foreach (var menuPart in items
                .Where(i => i.Position.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Count() == level)
                .OrderBy(m => m.Position, new PositionComparer()))
            {
                if (menuPart == null) continue;
                var part = menuPart;

                // Choosing descendants of a current item
                var descendants = items.Where(i =>
                    i.Position.StartsWith(part.Position + ".")
                    && i.Position.Trim() != part.Position.Trim());

                if (UserCanSeeMenuItem(menuPart)) {
                    if (part.RelatedItem == null) {
                        builder.Add(new LocalizedString(HttpUtility.HtmlEncode(part.Text)), part.Position,
                                    item => AddMenuItem(item, part, descendants, permissions, level, UrlStrategy));
                    }
                    else {
                        builder.Add(new LocalizedString(HttpUtility.HtmlEncode(part.Text)), part.Position,
                                    item => AddMenuItem(item, part, descendants, permissions, level, RelatedItemStrategy));
                    }
                }
            }
        }

        private void AddMenuItem(NavigationItemBuilder item, AdvancedMenuItemPart part, IEnumerable<AdvancedMenuItemPart> descendants, IList<Permission> permissions, int level, Action<NavigationItemBuilder, AdvancedMenuItemPart> menuItemLocationStrategy) {
            item.IdHint(part.ContentItem.Id.ToString());
            menuItemLocationStrategy(item, part);
            if (!string.IsNullOrWhiteSpace(part.RequiresPermission)) {
                var permission = permissions.FirstOrDefault(p => p.Name == part.RequiresPermission);
                if (permission != null) item.Permission(permission);
            }
            if (descendants.Any())
                BuildHierarchy(permissions, descendants, item, level + 1);
        }

        private void UrlStrategy(NavigationItemBuilder item, AdvancedMenuItemPart part) {
            var targetUrlString = part.Url;

            if (part.IncludeReturnUrl) {
                var queryString = "?";
                var returnUrl = HttpUtility.UrlEncode(_orchardServices.WorkContext.HttpContext.Request.Path);

                if (targetUrlString.Contains("?")) {
                    queryString = "&";
                }
                targetUrlString = String.Format("{0}{1}ReturnUrl={2}", targetUrlString, queryString, returnUrl);
            }
            item.Url(targetUrlString);
        }

        private void RelatedItemStrategy(NavigationItemBuilder item, AdvancedMenuItemPart part) {
            if (part.IncludeReturnUrl) {
                var returnUrl = _orchardServices.WorkContext.HttpContext.Request.Path;
                part.RouteValues.Add("ReturnUrl", returnUrl);
            }
            item.Action(part.RouteValues);
        }

        private bool UserCanSeeMenuItem(AdvancedMenuItemPart menuItem) {
            bool canSeeItem = menuItem.AuthenticationLevel == AuthenticationLevel.Any;

            if (!canSeeItem) {
                var currentUser = _orchardServices.WorkContext.CurrentUser;
                canSeeItem = (currentUser == null && menuItem.AuthenticationLevel == AuthenticationLevel.AnonymousOnly) ||
                            (currentUser != null && menuItem.AuthenticationLevel == AuthenticationLevel.AuthenticatedOnly);
            }

            return canSeeItem;
        }
    }
}