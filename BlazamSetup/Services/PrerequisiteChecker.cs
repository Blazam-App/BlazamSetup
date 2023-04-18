using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                        if (dir.Contains("6."))
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
                            if (possibleKey.Contains("Microsoft .NET 6") && possibleKey.Contains("Hosting"))
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
    }
}
