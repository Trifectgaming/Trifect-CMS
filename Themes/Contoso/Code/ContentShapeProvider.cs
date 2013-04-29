using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;

namespace Contoso.ShapeProviders {
    public class ContentShapeProvider : IShapeTableProvider {
        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Content")
                .OnDisplaying(displaying => {
                                  if (displaying.ShapeMetadata.DisplayType == "Detail") {
                                      var autoRoute = ((IContent)displaying.Shape.ContentItem).As<AutoroutePart>();
                                      if (autoRoute != null && autoRoute.Path == "") {
                                          displaying.ShapeMetadata.Alternates.Add("Content__HomePage");
                                      }
                                  }
                              });
        }
    }
}