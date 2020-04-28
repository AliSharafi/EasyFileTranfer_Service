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
#if DEBUG
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "EFTService";
                    eventLog.WriteEntry("GetSavePath : 1", EventLogEntryType.Information, 101, 1);
                }
#endif
                fileMap.ExeConfigFilename = ConfigPath;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                var settings = config.AppSettings.Settings;
#if DEBUG
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "EFTService";
                    eventLog.WriteEntry("GetSavePath : 2" + settings.Count.ToString(), EventLogEntryType.Information, 101, 1);
                }
#endif
                if (settings["AppConfigs"] != null && settings["AppConfigs"].Value != "")
                {
#if DEBUG
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "EFTService";
                        eventLog.WriteEntry("GetSavePath Json: " + settings["AppConfigs"].Value, EventLogEntryType.Information, 101, 1);
                    }
#endif
                    conf = (AppConfigs)_serializer.Deserialize(settings["AppConfigs"].Value, typeof(AppConfigs));
                }
#if DEBUG
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "EFTService";
                    eventLog.WriteEntry("GetSavePath : 3", EventLogEntryType.Information, 101, 1);
                }
#endif
                string currUser = fileNameWithUsername.Split('■')[1];
                string filename = fileNameWithUsername.Split('■')[0];

#if DEBUG
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "EFTService";
                    eventLog.WriteEntry("GetSavePath : 4" + conf == null ? "conf is null" : conf.ToString(), EventLogEntryType.Information, 101, 1);
                    eventLog.WriteEntry("GetSavePath : 5" + conf.Employees.Count.ToString(), EventLogEntryType.Information, 101, 1); ;
                    eventLog.WriteEntry("GetSavePath : 6" + conf.Employees.FirstOrDefault().Username, EventLogEntryType.Information, 101, 1); ;
                    eventLog.WriteEntry("GetSavePath  result : " + conf.Employees.Where(e => e.Username.Trim().ToLower() == currUser.Trim().ToLower()).FirstOrDefault().SavePath + "\\" + filename, EventLogEntryType.Information, 101, 1);
                }
#endif

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
