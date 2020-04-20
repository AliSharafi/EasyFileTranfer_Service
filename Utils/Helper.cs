using EasyFileTransfer.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EFTService.Utils
{
    public class Helper
    {
        public static string GetSavePath(string ConfigPath)
        {
            JavaScriptSerializer _serializer = new JavaScriptSerializer();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            AppConfigs conf = new AppConfigs();

            fileMap.ExeConfigFilename = ConfigPath;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap,ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;
            if (settings["AppConfigs"] != null && settings["AppConfigs"].Value != "")
            {
               conf = (AppConfigs)_serializer.Deserialize(settings["AppConfigs"].Value, typeof(AppConfigs));
            }

            string currUser = GetCurrentUsername();
            return conf.Employees.Where(e => e.Username == currUser).FirstOrDefault().SavePath;
        }

        static string GetCurrentUsername()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
            ManagementObjectCollection collection = searcher.Get();
            return (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
        }
    }
}
