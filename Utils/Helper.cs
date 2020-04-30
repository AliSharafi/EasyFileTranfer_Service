using EasyFileTransfer.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EFTService.Utils
{
    public class Helper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConfigPath"></param>
        /// <param name="fileName"> Contains username eg : test.txt■mabna\sharafi</param>
        /// <returns></returns>
        public static string GetSavePath(string ConfigPath,string fileNameWithUsername)
        {
            try
            {
#if DEBUG
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "EFTService";
                    eventLog.WriteEntry("GetSavePath  fileNameWithUsername : " + fileNameWithUsername + " ConfigPath : " + ConfigPath, EventLogEntryType.Information, 101, 1);
                }
#endif
                JavaScriptSerializer _serializer = new JavaScriptSerializer();
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                AppConfigs conf = new AppConfigs();

                fileMap.ExeConfigFilename = ConfigPath;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                var settings = config.AppSettings.Settings;

                if (settings["AppConfigs"] != null && settings["AppConfigs"].Value != "")
                {
                    conf = (AppConfigs)_serializer.Deserialize(settings["AppConfigs"].Value, typeof(AppConfigs));
                }

                string currUser = fileNameWithUsername.Split('■')[1];
                string filename = fileNameWithUsername.Split('■')[0];

                return conf.Employees.Where(e => e.Username.Trim().ToLower() == currUser.Trim().ToLower()).FirstOrDefault().SavePath + "\\" + filename;
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "EFTService";
                    eventLog.WriteEntry("GetSavePath ex : " + ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error, 101, 1);
                }
                return "";
            }
        }

        //static string GetCurrentUsername()
        //{
        //    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
        //    ManagementObjectCollection collection = searcher.Get();
        //    return (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
        //}
    }
}
