using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace oforms.Models
{
    public class OFormSettingsPart : ContentPart<OFormSettingsPartRecord>
    {
        [Required]
        public string SerialKey 
        {
            get { return Record.SerialKey; }
            set { Record.SerialKey = value; }
        }
    }
}