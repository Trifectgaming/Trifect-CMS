using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Facebook.Like.Models;

namespace Facebook.Like.Drivers
{
    public class LikeDriver : ContentPartDriver<LikePart>
    {
        protected override DriverResult Display(LikePart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_Like",
                () => shapeHelper.Parts_Like(Show_Faces: part.Show_Faces,
                                                Width: part.Width,
                                                Font: part.Font,
                                                Colorsheme: part.Colorsheme,
                                                Verb: part.Verb,
                                                LayoutStyle: part.LayoutStyle));
        }

        protected override DriverResult Editor(LikePart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Like_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Like",
                    Model: part,
                    Prefix: Prefix));
        }

        protected override DriverResult Editor(LikePart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}