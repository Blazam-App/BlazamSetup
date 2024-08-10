using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BlazamSetup.Services
{
    internal static  class PrerequisiteChecker
    {
        internal static bool CheckForAspCore()
        {
            try
            {
                var dirs = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\dotnet\\shared\\Microsoft.NETCore.App");
                if (dirs != null && dirs.Length > 0)
                {

                    foreach (var dir in dirs)
                    {
                        if (dir.Contains("8."))
                        {
                            

                            return true;
                        }
                    }

                }
            }
            catch { }
            return false;
        }
      internal static bool CheckForAspCoreHosting()
        {
            try
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Updates\\.NET\\");
                if (key != null)
                {
                    var possibleAspKeys = key.GetSubKeyNames();
                    if (possibleAspKeys.Length > 0)
                    {
                        foreach (var possibleKey in possibleAspKeys)
                        {
                            if (possibleKey.Contains("Microsoft .NET 8") && possibleKey.Contains("Hosting"))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch { }
            return false;

        }
        internal static bool CheckForWebSockets()
        {
            ManagementClass objMC = new ManagementClass("Win32_OptionalFeature");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                string featureName = (string)objMO.Properties["Name"].Value;
                if (featureName.Equals("IIS-WebSockets", StringComparison.InvariantCultureIgnoreCase))
                {
                    var path = (uint)objMO.Properties["InstallState"].Value;
                    return path.Equals(1);
                }
                
            }
            return false;
        }
    }
}
