using System;
using System.IO;
using System.Web;
using Orchard;
using Orchard.Logging;
using Orchard.ContentManagement;
using oforms.Models;

namespace oforms.Services
{
    public class SerialService : ISerialService
    {
        private readonly IOrchardServices orchardServices;

        static readonly string serialKeyFromFile = ReadSerialFromFile();

        public SerialService(IOrchardServices orchardServices) {
            this.orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool IsSerialValid()
        {
            string serialKey = serialKeyFromFile;
            var oformSettings = orchardServices.WorkContext.CurrentSite.As<OFormSettingsPart>();
            if (oformSettings != null && !string.IsNullOrEmpty(oformSettings.SerialKey))
            {
                serialKey = oformSettings.SerialKey.Trim();
            }
             
            if (IsNumeric(DecodeSerial(serialKey)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string ReadSerialFromFile() {
            string text;
            try {
                text = File.ReadAllText(GetSerialFilePath());
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return text.Trim();
        }

        private string DecodeSerial(string str)
        {
            try
            {
                // ignore 3st 3 letters
                str = str.Substring(3, str.Length - 3);
                byte[] decbuff = Convert.FromBase64String(str);
                return System.Text.Encoding.UTF8.GetString(decbuff);
            }
            catch (Exception ex)
            {
            	Logger.Information(ex, "Can not decode serial");
                return "";
            }

        }

        private static bool IsNumeric(string s)
        {
            double Result;
            return double.TryParse(s, out Result);  // TryParse routines were added in Framework version 2.0.
        } 

        private static string GetSerialFilePath()
        {
            var oformsDataDir = HttpContext.Current.Server.MapPath("~/App_Data/oforms/");
            return Path.Combine(oformsDataDir, "sn.dat");
        }
    }
}