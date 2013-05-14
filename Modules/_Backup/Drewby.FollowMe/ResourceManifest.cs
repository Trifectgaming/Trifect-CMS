using Orchard.UI.Resources;

namespace Drewby.FollowMe
{
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            builder.Add().DefineStyle("FollowMe").SetUrl("followme.css");
        }
    }
}
