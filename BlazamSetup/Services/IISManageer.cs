using Microsoft.Web.Administration;
using Org.BouncyCastle.Bcpg.OpenPgp;
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
                FileSystemService.AddPermission(
                    InstallationConfiguraion.InstallDirPath + @"Blazam\\",
                    "IIS_IUSRS",
                    FileSystemRights.ReadAndExecute
                    );
                return true;
            }
            return false;
        }
       
    }
}
