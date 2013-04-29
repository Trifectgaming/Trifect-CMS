using System;
using oforms.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;

namespace oforms.Drivers {
    public class OFormSettingsPartDriver : ContentPartDriver<OFormSettingsPart> {
        public OFormSettingsPartDriver()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override string Prefix { get { return "OFormSettings"; } }

        protected override DriverResult Editor(OFormSettingsPart part, dynamic shapeHelper)
        {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(OFormSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {

            return ContentShape("Parts_OFormSettings_Edit", () => {
                    if (updater != null) {
                        updater.TryUpdateModel(part.Record, Prefix, null, null);
                    }
                    return shapeHelper.EditorTemplate(TemplateName: "Parts/OFormSettings", Model: part, Prefix: Prefix); 
                })
                .OnGroup("oforms");
        }
    }
}