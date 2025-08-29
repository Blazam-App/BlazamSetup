using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Web.Administration;
using Serilog;

namespace BlazamSetup.Services
{
    public static class IISManager
    {
        public static bool EnablePrerequisites()
        {
            try
            {
                EnableWebSockets();
                EnableApplicationInit();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error installing IIS prerequisites");
            }

            return false;
        }

        private static void EnableWebSockets()
        {

            try
            {
                if (!PrerequisiteChecker.CheckForWebSockets())
                {

                    var dism = Process.Start("Dism", "/online /Enable-Feature /FeatureName:IIS-WebSockets /All");
                    dism.WaitForExit();

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error enabling Web Sockets");
            }
        }

        private static void EnableApplicationInit()
        {

            try
            {
                if (!PrerequisiteChecker.CheckForApplicationInitializationModule())
                {
                    var dism = Process.Start("Dism", "/online /Enable-Feature /FeatureName:IIS-ApplicationInit /All");

                    dism.WaitForExit();
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error enabling Application Initialization");
            }

        }


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
                        site = AddSite(serverManager, httpBinding);
                    }


                    Log.Information("IIS Site {@Site}", site);


                    if (InstallationConfiguraion.WebHostConfiguration.SSLCert != null && InstallationConfiguraion.WebHostConfiguration.HttpsPort != 0)
                    {
                        AddSSL(site);
                    }




                    serverManager.CommitChanges();
                }

                using (ServerManager serverManager = new ServerManager())
                {
                    if (PrerequisiteChecker.CheckForApplicationInitializationModule())
                    {
                        var site = serverManager.Sites.FirstOrDefault(s => s.Name == "Blazam");
                        ConfigureApplicationInitialization(serverManager, site);
                        serverManager.CommitChanges();
                    }

                }
                SecurityIdentifier iisIUSRSsid;
                try
                {
                    iisIUSRSsid = (SecurityIdentifier)new NTAccount("IIS_IUSRS").Translate(typeof(SecurityIdentifier));
                }
                catch (IdentityNotMappedException ex)
                {
                    Log.Error(ex, "Failed to translate IIS_IUSRS to SID. IIS may not be installed or configured correctly.");
                    return false;
                }

                if (!FileSystemService.AddPermission(
                    Path.GetFullPath(InstallationConfiguraion.InstallDirPath),
                    iisIUSRSsid,
                    FileSystemRights.ReadAndExecute
                    ))
                {
                    Log.Error("Failed to set ReadAndExecute permission for IIS_IUSRS on install directory.");
                }
                return true;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while creating IIS website. {@Error}");
            }
            return false;
        }

        public static void ConfigureApplicationInitialization(ServerManager serverManager, Site site)
        {
            Log.Information("Application Initialization Module found, setting preload and always running");

            site.Applications[0].SetAttributeValue("preloadEnabled", true);

            ApplicationPool appPool = serverManager.ApplicationPools["Blazam"];
            appPool.StartMode = StartMode.AlwaysRunning;
        }

        public static void UnconfigureApplicationInitialization(ServerManager serverManager, Site site)
        {
            Log.Information("Removing preload and always running");

            site.Applications[0].SetAttributeValue("preloadEnabled", false);

            ApplicationPool appPool = serverManager.ApplicationPools["Blazam"];
            appPool.StartMode = StartMode.OnDemand;
        }

        public static void AddSSL(Site site)
        {
            string httpsBindingInfo = InstallationConfiguraion.WebHostConfiguration.ListeningAddress + ":" + InstallationConfiguraion.WebHostConfiguration.HttpsPort + ":";

            var existingBinding = site.Bindings.FirstOrDefault(b => b.Protocol == "https" &&
                                           b.EndPoint.Port == InstallationConfiguraion.WebHostConfiguration.HttpsPort);

            if (existingBinding == null)
            {
                var cert = InstallationConfiguraion.WebHostConfiguration.SSLCert;
                // Get the certificate hash (thumbprint) and specify the store ("My" is the Personal store).
                byte[] certHash = cert.GetCertHash();
                string certStoreName = "My";

                // Add the new HTTPS binding.
                site.Bindings.Add(httpsBindingInfo, certHash, certStoreName);

                //newBinding.SetAttributeValue("sslFlags", 1); // 1 = SslFlags.Sni

                Log.Information("HTTPS binding added for port {Port}", InstallationConfiguraion.WebHostConfiguration.HttpsPort);
            }
            else
            {
                Log.Information("An HTTPS binding for port {Port} already exists.", InstallationConfiguraion.WebHostConfiguration.HttpsPort);
            }
        }

        public static Site AddSite(ServerManager serverManager, string httpBinding)
        {
            if (!serverManager.ApplicationPools.Any(p => p.Name == "Blazam"))
            {
                serverManager.ApplicationPools.Add("Blazam");
            }
            var site = serverManager.Sites.Add("Blazam",
                "http",
                httpBinding,
                Path.GetFullPath(InstallationConfiguraion.InstallDirPath));

            site.ApplicationDefaults.ApplicationPoolName = "Blazam";
            return site;
        }

        public static bool RemoveApplication()
        {
            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Log.Information("IIS Connected");
                    Site site = serverManager.Sites.FirstOrDefault(s => s.Name == "Blazam");
                    if (site is null)
                    {
                        return true;
                    }

                    Log.Information("Deleting IIS Site {@Site}", site);
                    serverManager.Sites.Remove(site);
                    var pool = serverManager.ApplicationPools.FirstOrDefault(p => p.Name == "Blazam");
                    if (pool != null)
                    {
                        serverManager.ApplicationPools.Remove(pool);
                    }

                    serverManager.CommitChanges();


                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while removing IIS website. {@Error}");
            }
            return false;
        }

        public static bool Start()
        {
            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Log.Information("IIS Connected");
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
                Log.Error(ex, "Error while starting IIS website. {@Error}");
            }
            return false;
        }


        public static void RemoveSSL(Site site)
        {
            var httpsBindings = site.Bindings
                .Where(b => b.Protocol == "https" &&
                            b.EndPoint.Port == InstallationConfiguraion.WebHostConfiguration.HttpsPort)
                .ToList();

            if (httpsBindings.Count == 0)
            {
                Log.Information("No HTTPS bindings found for port {Port} to remove.", InstallationConfiguraion.WebHostConfiguration.HttpsPort);
                return;
            }

            foreach (var binding in httpsBindings)
            {
                site.Bindings.Remove(binding);
                Log.Information("Removed HTTPS binding for port {Port}.", InstallationConfiguraion.WebHostConfiguration.HttpsPort);
            }
        }
    }
}
