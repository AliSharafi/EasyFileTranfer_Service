using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.Script.Serialization;

namespace EasyFileTransfer.Utils
{
    public class AppConfigs
    {
        public AppConfigs()
        {
            Employees = new List<Employee>();
            ValidExtensions = new List<FileExtensions>();
        }

        private int _maxSize;
        public int MaxSize
        {
            get
            {
                return _maxSize;
            }
            set
            {
                _maxSize = value;
            }
        }

        List<Employee> _employees;
        public List<Employee> Employees
        {
            get
            {
                return _employees;
            }
            set
            {
                _employees = value;
            }

        }

        List<FileExtensions> _validExtensions;
        public List<FileExtensions> ValidExtensions
        {
            get
            {
                return _validExtensions;
            }
            set
            {
                _validExtensions = value;
            }
        }

        public static AppConfigs Load()
        {
            JavaScriptSerializer _serializer = new JavaScriptSerializer();

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if(settings["AppConfigs"] != null && settings["AppConfigs"].Value != "")
            {
                return (AppConfigs) _serializer.Deserialize(settings["AppConfigs"].Value,typeof(AppConfigs));
            }
            return new AppConfigs();
        }

        public static void Save(AppConfigs conf)
        {
            JavaScriptSerializer _serializer = new JavaScriptSerializer();
            AddOrUpdateAppSettings("AppConfigs",_serializer.Serialize(conf));
        }
        static void AddOrUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    
        

    }

    public class Employee
    {
        #region Private Variables
        private string _userName;
        private string _savePath;
        private string _ipAddres;
        #endregion

        #region Public Properties
        public string Username
        {
            get { return _userName; }
            set { _userName = value; }
        }
        public string SavePath
        {
            get { return _savePath; }
            set { _savePath = value; }
        }
        public string IPAddress
        {
            get
            {
                return _ipAddres;
            }
            set
            {
                _ipAddres = value;
            }
        }
        #endregion
    }

    public class FileExtensions
    {
        private string _extension;

        public string Extension
        {
            get
            {
                return _extension;
            }
            set
            {
                _extension = value;

            }
        }

    }

}
