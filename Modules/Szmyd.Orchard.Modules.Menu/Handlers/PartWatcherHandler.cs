using Orchard.Autoroute.Models;
using Orchard.ContentManagement.Handlers;
using Szmyd.Orchard.Modules.Menu.Services;

namespace Szmyd.Orchard.Modules.Menu.Handlers
{
    public class PartWatcherHandler : ContentHandler
    {
        public PartWatcherHandler(IPartWatcher watcher)
        {
            OnGetDisplayShape<AutoroutePart>((ctx, part) =>
            {
                if (ctx.DisplayType != "Detail") return;
                watcher.Watch(part);
            });
        }
    }
}