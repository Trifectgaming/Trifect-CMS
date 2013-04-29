using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.UI.Notify;
using Szmyd.Orchard.Modules.Menu.Enums;
using Szmyd.Orchard.Modules.Menu.Models;
using Szmyd.Orchard.Modules.Menu.Utilities;

namespace Szmyd.Orchard.Modules.Menu.Drivers
{
    
    public class MenuStylingPartDriver : ContentPartDriver<MenuStylingPart>
    {
        private readonly INotifier _notifier;
        private const string TemplateName = "Parts/Menu.Styling";

        public Localizer T { get; set; }

        public MenuStylingPartDriver(INotifier notifier)
        {
            _notifier = notifier;
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Display(MenuStylingPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_Menu_Styling",
                () => shapeHelper.Parts_Menu_Styling(ContentPart: part));
        }

        protected override DriverResult Editor(MenuStylingPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Menu_Styling",
                    () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(MenuStylingPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if (updater.TryUpdateModel(part, Prefix, null, null))
            {
                _notifier.Information(T("Menu styling edited successfully"));
            }
            else
            {
                _notifier.Error(T("Error during item counter update!"));
            }
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(MenuStylingPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("BackColor", part.BackColor);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ForeColor", part.ForeColor);
            context.Element(part.PartDefinition.Name).SetAttributeValue("SelectedBackColor", part.SelectedBackColor);
            context.Element(part.PartDefinition.Name).SetAttributeValue("SelectedForeColor", part.SelectedForeColor);
            context.Element(part.PartDefinition.Name).SetAttributeValue("HoverBackColor", part.HoverBackColor);
            context.Element(part.PartDefinition.Name).SetAttributeValue("HoverForeColor", part.HoverForeColor);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Style", part.Style);
        }

        protected override void Importing(MenuStylingPart part, ImportContentContext context) {
            string partName = part.PartDefinition.Name;

            part.BackColor = DriverImportUtility.GetAttribute<string>(context, partName, "BackColor");
            part.ForeColor = DriverImportUtility.GetAttribute<string>(context, partName, "ForeColor");
            part.SelectedBackColor = DriverImportUtility.GetAttribute<string>(context, partName, "SelectedBackColor");
            part.SelectedForeColor = DriverImportUtility.GetAttribute<string>(context, partName, "SelectedForeColor");
            part.HoverBackColor = DriverImportUtility.GetAttribute<string>(context, partName, "HoverBackColor");
            part.HoverForeColor = DriverImportUtility.GetAttribute<string>(context, partName, "HoverForeColor");
            part.Style = DriverImportUtility.GetEnumAttribute<MenuStyles>(context, partName, "Style");
        }
    }
}