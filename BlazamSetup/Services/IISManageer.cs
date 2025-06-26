using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.Web.Administration;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

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
                            Path.GetFullPath(InstallationConfiguraion.InstallDirPath));
                    }

                    Log.Information("IIS Site {@Site}", site);

                    serverManager.CommitChanges();

                    SecurityIdentifier iisIUSRSsid;
                    try
                    {
                        iisIUSRSsid = (SecurityIdentifier)new NTAccount("IIS_IUSRS").Translate(typeof(SecurityIdentifier));
                    }
                    catch (IdentityNotMappedException ex)
                    {
                        Log.Error("Failed to translate IIS_IUSRS to SID. IIS may not be installed or configured correctly. {@Error}", ex);
                        return false;
                    }

                    if (!FileSystemService.AddPermission(
                        Path.GetFullPath(InstallationConfiguraion.InstallDirPath),
                        iisIUSRSsid,
                        FileSystemRights.ReadAndExecute
                        ))
                    {
                        Log.Error("Failed to set ReadAndExecute permission for IIS_IUSRS on install directory.");
                        // Depending on desired behavior, we might want to return false here
                        // For now, matching previous behavior of AddPermission potentially failing silently at this level of caller
                    }
                    return true;
                }
            }
            catch ( Exception ex ) // This is a general catch block, ensure specific IdentityNotMappedException for SID translation is handled above.
            {
                Log.Error("Error while creating IIS website. {@Error}", ex);
            }
            return false;
        }

        internal static bool RemoveApplication()
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
                        return true;
                    }

                    Log.Information("Deleting IIS Site {@Site}", site);
                    serverManager.Sites.Remove(site);
                    serverManager.CommitChanges();

                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while removing IIS website. {@Error}", ex);
            }
            return false;
        }

        internal static bool Start()
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
                        Log.Error("Tried to start site when it doesn't exist");

                        return false;
                    }

                    Log.Information("IIS Site {@Site}", site);

                    site.Start();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while starting IIS website. {@Error}", ex);
            }
            return false;
        }
    }
}
