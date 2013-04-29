using System.Collections.Generic;
using System.Linq;

using Orchard;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Szmyd.Orchard.Modules.Menu.Models;
using Szmyd.Orchard.Modules.Menu.Models.Menus;
using Szmyd.Orchard.Modules.Menu.Services;
using Szmyd.Orchard.Modules.Menu.ViewModels;

namespace Szmyd.Orchard.Modules.Menu.Drivers
{
    using Szmyd.Orchard.Modules.Menu.Utilities;

    public class MenuRelationPartDriver : ContentPartDriver<MenuRelationPart>
    {
        private readonly INotifier _notifier;
        private readonly IAdvancedMenuService _service;
        private readonly IOrchardServices _services;
        private readonly INavigationManager _nav;
        private const string TemplateName = "Parts/Menu.RelationPart";

        public Localizer T { get; set; }

        public MenuRelationPartDriver(INotifier notifier, IAdvancedMenuService service, IOrchardServices services, INavigationManager nav)
        {
            _notifier = notifier;
            _service = service;
            _services = services;
            _nav = nav;
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Editor(MenuRelationPart part, dynamic shapeHelper)
        {
            var availableMenus = _service.GetMenus().Select(m => {
                                                                var items = _nav.BuildMenu(m.Name);
                                                                return new MenuRelation {
                                                                    MenuName = m.Name,
                                                                    MenuText = part.As<TitlePart>().Title,
                                                                    Position = items.Any() ? PositionUtility.GetNext(items) : "1",
                                                                    Selected = false
                                                                };
                                                            });
            var currentMenus = _service.GetMenuItemsForContent(part.ContentItem).GroupBy(
                m => m.MenuName,
                item => new MenuRelation
                {
                    MenuName = item.MenuName,
                    MenuText = item.Text,
                    Position = item.Position,
                    Selected = true
                },
                (key, group) => group.First());

            part.Menus = currentMenus.Union(availableMenus, (c, a) => c.MenuName == a.MenuName);

            return ContentShape("Parts_Menu_RelationPart",
                    () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(MenuRelationPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if (updater.TryUpdateModel(part, Prefix, null, null))
            {
                if (part.SelectedMenus == null) part.SelectedMenus = new List<MenuRelation>();
                var currentMenus = _service.GetMenuItemsForContent(part.ContentItem).Select(m => new {m.MenuName, m.Id}).Distinct().ToList();
                var menusToDeleteFrom = currentMenus.Except(part.SelectedMenus.Select(m => new { m.MenuName, Id = 0 }), (i1, i2) => i1.MenuName == i2.MenuName);
                var menusToAddTo = part.SelectedMenus.Except(currentMenus.Select(c => new MenuRelation { MenuName = c.MenuName }), (i1, i2) => i1.MenuName == i2.MenuName).ToList();
                var menusToChange = part.SelectedMenus.Intersect(currentMenus.Select(c => new MenuRelation {MenuName = c.MenuName}), (i1, i2) => i1.MenuName == i2.MenuName);

                foreach (var menu in menusToAddTo)
                {
                    var menu1 = menu;
                    _services.ContentManager.Create<AdvancedMenuItemPart>("SimpleMenuItem", i =>
                    {
                        i.DisplayHref = true;
                        i.DisplayText = true;
                        i.MenuName = menu1.MenuName;
                        i.Position = menu1.Position;
                        i.Text = menu1.MenuText;
                        i.Record.RelatedContentId = part.ContentItem.Id;
                        i.Url = "/" + part.As<AutoroutePart>().Path;
                    });
                }
                foreach(var menu in menusToDeleteFrom) {
                    _service.DeleteMenuItem(menu.Id);
                }
                _notifier.Information(T("Menu relation edited successfully"));
            }
            else
            {
                _notifier.Error(T("Error during menu relation update!"));
            }
            return Editor(part, shapeHelper);
        }

    }
}