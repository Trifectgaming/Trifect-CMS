using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Security.Permissions;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Szmyd.Orchard.Modules.Menu.Models;
using Szmyd.Orchard.Modules.Menu.Services;
using Szmyd.Orchard.Modules.Menu.Utilities;

namespace Szmyd.Orchard.Modules.Menu.Drivers
{
    public class AdvancedMenuItemPartDriver : ContentPartDriver<AdvancedMenuItemPart>
    {
        private readonly IEnumerable<IPermissionProvider> _permissionProviders;
        private readonly INotifier _notifier;
        private readonly IAdvancedMenuService _service;
        private readonly INavigationManager _nav;
        private readonly IContentManager _content;
        private readonly ITransactionManager _trans;
        private const string TemplateName = "Parts/Menu.AdvancedItem";

        public Localizer T { get; set; }

        public AdvancedMenuItemPartDriver(IEnumerable<IPermissionProvider> permissionProviders, INotifier notifier, IAdvancedMenuService service, INavigationManager nav, IContentManager content, ITransactionManager trans)
        {
            _permissionProviders = permissionProviders;
            _notifier = notifier;
            _service = service;
            _nav = nav;
            _content = content;
            _trans = trans;
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Editor(AdvancedMenuItemPart part, dynamic shapeHelper)
        {
            if(string.IsNullOrWhiteSpace(part.Position) || part.Position == "0") {
                var positions = _nav.BuildMenu(part.MenuName ?? "main");
                part.Position = positions.Any() ? PositionUtility.GetNext(_nav.BuildMenu(part.MenuName ?? "main")) : "1";
            }

            var routables = _content.Query<AutoroutePart, AutoroutePartRecord>().List().Select(r => r);
            part.AvailableItems = new SelectList(new[] { new SelectListItem { Text = T("(None)").Text, Value = default(int).ToString(CultureInfo.InvariantCulture) } }
                .Concat(routables.OrderBy(x => x.ContentItem.ContentType).ThenBy(x => x.Id).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(CultureInfo.InvariantCulture),
                    Text = string.Format("{1}:\t{0}", x.As<TitlePart>().Title, x.ContentItem.TypeDefinition.DisplayName),
                    Selected = x.Id == part.RelatedItemId
                })), "Value", "Text", part.RelatedItemId.ToString(CultureInfo.InvariantCulture));
            if (part.RelatedItem != null) {
                part.ChosenItemId = part.RelatedItemId;
            }

            var permissions = _permissionProviders.SelectMany(p => p.GetPermissions().Select(i => new {p.Feature, Permission = i}));
            part.AvailablePermissions = new SelectList(new[] { new SelectListItem { Text = T("(None)").Text, Value = "" } }
                .Concat(permissions
                    .OrderBy(x => x.Permission.Category)
                    .ThenBy(x => x.Feature.Descriptor.Id)
                    .ThenBy(x => x.Permission.Name)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Permission.Name,
                        Text = string.Format("{0}:\t{1}\t({2})", x.Permission.Category, x.Permission.Description, x.Feature.Descriptor.Name),
                        Selected = x.Permission.Name == part.RequiresPermission
                    })), "Value", "Text", part.RequiresPermission);

            part.AvailableAuthenticationLevels = new SelectList(new[] {
                new SelectListItem { Text = "Any", Value = AuthenticationLevel.Any.ToString() },
                new SelectListItem { Text = "Anonymous Only", Value = AuthenticationLevel.AnonymousOnly.ToString() },
                new SelectListItem { Text = "Authenticated Only", Value = AuthenticationLevel.AuthenticatedOnly.ToString() },
            }, "Value", "Text", part.AuthenticationLevel.ToString());

            return ContentShape("Parts_Menu_AdvancedItem",
                    () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(AdvancedMenuItemPart part, IUpdateModel updater, dynamic shapeHelper) {
            var ok = updater.TryUpdateModel(part, Prefix, null, null);
            var validated = ValidateItem(part, updater);
            if (!ok || !validated)
            {
                _notifier.Error(T("Error during menu update!"));
                _trans.Cancel();
            }
            else {
                if(part.ChosenItemId != default(int))
                    part.Record.RelatedContentId = part.ChosenItemId;
                _notifier.Information(T("Menu edited successfully"));
                _service.TriggerSignal();
            }
            
            
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(AdvancedMenuItemPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Text", part.Text);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Position", part.Position);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Url", part.Url);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MenuName", part.MenuName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("SubTitle", part.SubTitle);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Classes", part.Classes);
            context.Element(part.PartDefinition.Name).SetAttributeValue("DisplayText", part.DisplayText);
            context.Element(part.PartDefinition.Name).SetAttributeValue("DisplayHref", part.DisplayHref);
            context.Element(part.PartDefinition.Name).SetAttributeValue("RequiresPermission", part.RequiresPermission);
            context.Element(part.PartDefinition.Name).SetAttributeValue("AuthenticationLevel", part.AuthenticationLevel);
            context.Element(part.PartDefinition.Name).SetAttributeValue("IncludeReturnUrl", part.IncludeReturnUrl);

            if (part.RelatedItem != null) {
                var relatedItemIdentity = _content.GetItemMetadata(part.RelatedItem).Identity;
                context.Element(part.PartDefinition.Name).SetAttributeValue("RelatedItem", relatedItemIdentity.ToString());
            }
        }

        protected override void Importing(AdvancedMenuItemPart part, ImportContentContext context) {
            string partName = part.PartDefinition.Name;

            part.Text = DriverImportUtility.GetAttribute<string>(context, partName, "Text");
            part.Position = DriverImportUtility.GetAttribute<string>(context, partName, "Position");
            part.Url = DriverImportUtility.GetAttribute<string>(context, partName, "Url");
            part.MenuName = DriverImportUtility.GetAttribute<string>(context, partName, "MenuName");
            part.SubTitle = DriverImportUtility.GetAttribute<string>(context, partName, "SubTitle");
            part.Classes = DriverImportUtility.GetAttribute<string>(context, partName, "Classes");
            part.DisplayText = DriverImportUtility.GetAttribute<bool>(context, partName, "DisplayText");
            part.DisplayHref = DriverImportUtility.GetAttribute<bool>(context, partName, "DisplayHref");
            part.RequiresPermission = DriverImportUtility.GetAttribute<string>(context, partName, "RequiresPermission");
            part.AuthenticationLevel = DriverImportUtility.GetEnumAttribute<AuthenticationLevel>(context, partName, "AuthenticationLevel");
            part.IncludeReturnUrl = DriverImportUtility.GetAttribute<bool>(context, partName, "IncludeReturnUrl");
        }

        protected override void Imported(AdvancedMenuItemPart part, ImportContentContext context) {
            var relatedItemId = DriverImportUtility.GetAttribute<string>(context, part.PartDefinition.Name, "RelatedItem");
            if (!string.IsNullOrWhiteSpace(relatedItemId)) {
                var relatedItem = context.GetItemFromSession(relatedItemId);
                if (relatedItem != null) {
                    part.Record.RelatedContentId = relatedItem.Id;
                }
            }
        }

        private bool ValidateItem(AdvancedMenuItemPart part, IUpdateModel updater) {
            var retVal = true;

            if (string.IsNullOrWhiteSpace(part.Position) || part.Position == "0")
            {
                updater.AddModelError("Position", T("Position cannot be empty."));
                retVal = false;
            }

            if (string.IsNullOrWhiteSpace(part.Url) && part.ChosenItemId == default(int))
            {
                updater.AddModelError("Url", T("You have to provide either static Url or choose a content item"));
                updater.AddModelError("ChosenItemId", T("You have to provide either static Url or choose a content item"));
                retVal = false;
            }

            return retVal;
        }
    }
}
