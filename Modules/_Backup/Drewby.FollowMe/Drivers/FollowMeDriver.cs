using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Drewby.FollowMe.Models;
using Drewby.FollowMe.ViewModels;

namespace Drewby.FollowMe.Drivers
{
    public class FollowMeDriver : ContentPartDriver<FollowMePart>
    {
        protected override DriverResult Display(FollowMePart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_FollowMe", () =>
                shapeHelper.Parts_FollowMe(FollowMe : new FollowMeViewModel(part)
                                            ));
        }

        //GET
        protected override DriverResult Editor(FollowMePart part, dynamic shapeHelper)
        {

            return ContentShape("Parts_FollowMe_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/FollowMe",
                    Model: part,
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(
            FollowMePart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

    }
}