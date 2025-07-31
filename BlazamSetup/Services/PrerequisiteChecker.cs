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
        private static bool IsFeatureEnabled(string featureName)
        {
            ManagementClass objMC = new ManagementClass("Win32_OptionalFeature");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                string name = (string)objMO.Properties["Name"].Value;
                if (name.Equals(featureName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var installState = (uint)objMO.Properties["InstallState"].Value;
                    return installState.Equals(1);
                }
            }
            return false;
        }

        internal static bool CheckForWebSockets()
        {
            return IsFeatureEnabled("IIS-WebSockets");
        }
        internal static bool CheckForApplicationInitializationModule()
        {
            return IsFeatureEnabled("IIS-ApplicationInit");
        }
    }
}
