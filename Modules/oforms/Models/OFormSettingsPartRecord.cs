using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace oforms.Models
{
    public class OFormSettingsPartRecord : ContentPartRecord
    {
        public virtual string SerialKey { get; set; }
    }
}