using System;
using Orchard.ContentManagement.Handlers;

namespace Szmyd.Orchard.Modules.Menu.Utilities {
    public static class DriverImportUtility {
        
        public static T GetAttribute<T>(ImportContentContext context, string partName, string elementName) {
            string value = context.Attribute(partName, elementName);
            if (value != null) {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return default(T);
        }

        public static T GetEnumAttribute<T>(ImportContentContext context, string partName, string elementName) {
            string value = context.Attribute(partName, elementName);
            if (value != null) {
                return (T)(Enum.Parse(typeof(T), value));
            }
            return default(T);
        }
         
    }
}