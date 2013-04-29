namespace Szmyd.Orchard.Modules.Menu.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Orchard.UI.Navigation;

    public static class PositionUtility {
        public static string GetNext(IEnumerable<MenuItem> items) {
            var retVal = "1";
            try {
                var maxPosition = PositionComparer.Max(items.Select(x => x.Position).Where(x => x != null));
                retVal = PositionComparer.After(maxPosition);
            }
            catch(NullReferenceException) {}

            return retVal;
        }
    }
}