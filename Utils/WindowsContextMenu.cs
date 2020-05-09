using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTService.Utils
{
    public class WindowsContextMenu
    {
        public static void Add(string Text)
        {
            string _currentPath = System.Reflection.Assembly.GetEntryAssembly().Location;

            RegistryKey _key = Registry.ClassesRoot.OpenSubKey("*\\Shell", true);
            RegistryKey newkey = _key.CreateSubKey(Text);
            newkey.SetValue("Icon", string.Concat(Path.GetDirectoryName(_currentPath) ,"\\eft.ico"));

            RegistryKey subNewkey = newkey.CreateSubKey("Command");
            subNewkey.SetValue("", string.Concat(_currentPath," " , "\"%1\"") );
            subNewkey.Close();
            newkey.Close();
            _key.Close();
        }    

        public static void Remove(string Text)
        {
            try
            {
                RegistryKey _key = Registry.ClassesRoot.OpenSubKey("*\\Shell", true);
                _key.DeleteSubKeyTree(Text);
                _key.Close();
            }
            catch { }
        }
    }
}
