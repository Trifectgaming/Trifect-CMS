using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.UI.Notify;
using Szmyd.Orchard.Modules.Menu.Models;
using Szmyd.Orchard.Modules.Menu.Services;

namespace Szmyd.Orchard.Modules.Menu.Drivers
{
    using global::Orchard.Data;

    public class AdvancedMenuPartDriver : ContentPartDriver<AdvancedMenuPart>
    {
        private readonly INotifier _notifier;
        private readonly IAdvancedMenuService _service;
        private readonly ITransactionManager _transaction;

        private const string TemplateName = "Parts/Menu.AdvancedMenu";

        public Localizer T { get; set; }

        public AdvancedMenuPartDriver(INotifier notifier, IAdvancedMenuService service, ITransactionManager transaction)
        {
            _notifier = notifier;
            _service = service;
            _transaction = transaction;
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Editor(AdvancedMenuPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Menu_AdvancedMenu",
                    () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(AdvancedMenuPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if (updater.TryUpdateModel(part, Prefix, null, null))
            {
                if (part.Name.Contains(" "))
                {
                    updater.AddModelError("AdvancedMenuPart.Name", T("Menu name cannot contain spaces"));
                }
                else
                {
                    _notifier.Information(this.T("Menu updated successfully")); 
                }  
            }
            else
            {
                _notifier.Error(T("Error during menu update!"));
            }
            _service.TriggerSignal();
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(AdvancedMenuPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Name", part.Name);
        }

        protected override void Importing(AdvancedMenuPart part, ImportContentContext context) {
            part.Name = context.Attribute(part.PartDefinition.Name, "Name");
        }
    }
}