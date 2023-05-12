using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace BlazamSetup.Services
{
    internal static class IISManager
    {
        public static bool CreateApplication()
        {
            using (ServerManager serverManager = new ServerManager())
            {
                if (!serverManager.Sites.Any(s => s.Name == "Blazam"))
                {
                    Site site = serverManager.Sites.Add("Blazam",
                        "http",
                        InstallationConfiguraion.WebHostConfiguration.ListeningAddress + ":" + InstallationConfiguraion.WebHostConfiguration.HttpPort + ":",
                        InstallationConfiguraion.InstallDirPath + @"Blazam\\");
                    serverManager.CommitChanges();
                }
                SetFolderPermissions();
                return true;
            }
            return false;
        }
        public static bool SetFolderPermissions()
        {
            var path = InstallationConfiguraion.InstallDirPath + @"Blazam\\";
            DirectoryInfo dInfo = new DirectoryInfo(path);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule("IIS_IUSRS", FileSystemRights.ReadAndExecute, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
            return true;
        }
    }
}
