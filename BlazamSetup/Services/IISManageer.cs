using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.Web.Administration;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace BlazamSetup.Services
{
    internal static class IISManager
    {
        public static bool CreateApplication()
        {
            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Log.Information("IIS Connected");
                    string httpBinding = InstallationConfiguraion.WebHostConfiguration.ListeningAddress + ":" + InstallationConfiguraion.WebHostConfiguration.HttpPort + ":";
                    Site site = serverManager.Sites.FirstOrDefault(s => s.Name == "Blazam");
                    if (site is null)
                    {
                        site = serverManager.Sites.Add("Blazam",
                            "http",
                            httpBinding,
                            InstallationConfiguraion.InstallDirPath + @"Blazam\\");
                    }

                    Log.Information("IIS Site {@Site}", site);

                    serverManager.CommitChanges();

                    FileSystemService.AddPermission(
                        InstallationConfiguraion.InstallDirPath + @"Blazam\\",
                        "IIS_IUSRS",
                        FileSystemRights.ReadAndExecute
                        );
                    return true;
                }
            }
            catch ( Exception ex )
            {
                Log.Error("Error while creating IIS website. {@Error}", ex);
            }
            return false;
        }
       
    }
}
