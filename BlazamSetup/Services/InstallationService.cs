﻿using Serilog;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlazamSetup.Services
{
    internal static class InstallationService
    {
        internal static AppEvent<double> OnProgress { get; set; }
        internal static AppEvent<string> OnStepTitleChanged { get; set; }
        internal static AppEvent OnInstallationFinished { get; set; }
        internal static CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

        internal static async Task StartInstallationAsync()
        {
            Log.Information("Installation Started {@InstallationType} {@InstallDirPath} {@DatabaseConfiguration}", InstallationConfiguraion.InstallationType, InstallationConfiguraion.InstallDirPath, InstallationConfiguraion.DatabaseConfiguration);

            if (!await PreInstallation()) Rollback();
            if (CancellationTokenSource.IsCancellationRequested) return;
            if (!await ApplyProgramFilesAsync(InstallationConfiguraion.InstallDirPath)) Rollback();
            if (CancellationTokenSource.IsCancellationRequested) return;

            await Task.Run(() =>
            {

                if (CancellationTokenSource.IsCancellationRequested) return;

                if (!CreateProgramDataDirectory()) Rollback();
                if (CancellationTokenSource.IsCancellationRequested) return;


                if (InstallationConfiguraion.InstallationType == InstallType.Service)
                {
                    OnStepTitleChanged?.Invoke("Install Services");
                    OnProgress?.Invoke(0);
                    if (!ServiceManager.Install()) Rollback();
                    if (CancellationTokenSource.IsCancellationRequested) return;


                    OnProgress?.Invoke(100);
                }
                else
                {
                    OnStepTitleChanged?.Invoke("Configuring IIS");
                    OnProgress?.Invoke(0);

                    if (!IISManager.CreateApplication()) Rollback();
                    if (CancellationTokenSource.IsCancellationRequested) return;

                    OnProgress?.Invoke(100);

                }
                OnStepTitleChanged?.Invoke("Finishing Installation");
                OnProgress?.Invoke(0);
                if (CancellationTokenSource.IsCancellationRequested) return;

                //Post install steps
                AppSettingsService.Copy();
                if (InstallationConfiguraion.DatabaseType == DBType.Sqlite)
                {
                    SecurityIdentifier sid;
                    if (InstallationConfiguraion.InstallationType == InstallType.Service)
                    {
                        sid = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);
                    }
                    else
                    {
                        try
                        {
                            sid = (SecurityIdentifier)new NTAccount("IIS_IUSRS").Translate(typeof(SecurityIdentifier));
                        }
                        catch (IdentityNotMappedException ex)
                        {
                            Log.Error("Failed to translate IIS_IUSRS to SID for SqliteDirectory permissions. IIS may not be installed or configured correctly. {@Error}", ex);
                            // This might lead to a failure later or be caught by Rollback() if AddPermission fails.
                            // Consider if a more direct Rollback() or cancellation is needed here.
                            // For now, proceeding to allow AddPermission to potentially fail.
                            return; // Or handle more gracefully, e.g., Rollback(); CancellationTokenSource.Cancel(); return;
                        }
                    }

                    Directory.CreateDirectory(InstallationConfiguraion.DatabaseConfiguration.SqliteDirectory);
                    if (sid != null) // Ensure sid was successfully created before attempting to use it
                    {
                        if (!FileSystemService.AddPermission(InstallationConfiguraion.DatabaseConfiguration.SqliteDirectory,
                            sid,
                            FileSystemRights.Write | FileSystemRights.Modify | FileSystemRights.ReadAndExecute
                            ))
                        {
                            Log.Warning($"Failed to set permissions for SID {sid} on SqliteDirectory. This might cause issues.");
                            // Not calling Rollback() here directly, as AddPermission itself doesn't throw to stop the flow.
                            // The installation might proceed with incorrect permissions.
                        }
                    }
                }
                AppSettingsService.Configure();
                InstallationConfiguraion.ProductInformation.EstimatedSize = (int)(FileSystemService.GetDirectorySize(InstallationConfiguraion.ProductInformation.InstallLocation) / 1024);
                RegistryService.SetProductInformation(InstallationConfiguraion.ProductInformation);
                OnProgress?.Invoke(100);
                OnStepTitleChanged?.Invoke("Installation Finished");
                Log.Information("Installation Finished Successfully");

                MainWindow.DisableBack();
                OnInstallationFinished?.Invoke();
            });
        }

        private static void Rollback()
        {
            Cancel();
        }

        private static bool CreateProgramDataDirectory()
        {
            try
            {
                SecurityIdentifier sid;
                if (InstallationConfiguraion.InstallationType == InstallType.Service)
                {
                    sid = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);
                }
                else
                {
                    try
                    {
                        sid = (SecurityIdentifier)new NTAccount("IIS_IUSRS").Translate(typeof(SecurityIdentifier));
                    }
                    catch (IdentityNotMappedException ex)
                    {
                        Log.Error("Failed to translate IIS_IUSRS to SID for ProgramDataDir permissions. IIS may not be installed or configured correctly. {@Error}", ex);
                        return false;
                    }
                }

                Directory.CreateDirectory(InstallationConfiguraion.ProgramDataDir);
                if (!FileSystemService.AddPermission( // Check return value
                    InstallationConfiguraion.ProgramDataDir,
                    sid,
                    FileSystemRights.Write | FileSystemRights.Modify | FileSystemRights.ReadAndExecute
                    ))
                {
                    Log.Error($"Failed to set permissions for SID {sid} on ProgramDataDir.");
                    return false; // Explicitly return false if AddPermission fails
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error creating program data directory: {@Error}", ex);

            }
            return false;
        }


        /// <summary>
        /// Copies the entire directory tree to another directory
        /// </summary>
        /// <param name="targetDirectory"></param>
        /// <returns></returns>
        private static async Task<bool> ApplyProgramFilesAsync(string targetDirectory)
        {
            return await Task.Run(() =>
            {
                return ApplyProgramFiles(targetDirectory);
            });

        }

        private static bool ApplyProgramFiles(string targetDirectory)
        {
            try
            {
                OnStepTitleChanged?.Invoke("Copy Files");
                Log.Information("File copy started");

                bool copyingDownTree = false;
                if (targetDirectory.Contains(DownloadService.SourceDirectory))
                {
                    copyingDownTree = true;
                }
                var totalFiles = FileSystemService.GetFileCount(DownloadService.SourceDirectory);
                var fileIndex = 0;

                if (Directory.Exists(DownloadService.SetupTempDirectory))
                {
                    var directories = Directory.GetDirectories(DownloadService.SourceDirectory, "*", SearchOption.AllDirectories).AsEnumerable();

                    if (copyingDownTree)
                        directories = directories.Where(d => !d.Contains(targetDirectory));

                    //Now Create all of the directories
                    foreach (string dirPath in directories)
                    {
                        Log.Information("Creating directory: " + dirPath);

                        Directory.CreateDirectory(dirPath.Replace(DownloadService.SourceDirectory, targetDirectory));
                    }
                    var files = Directory.GetFiles(DownloadService.SourceDirectory, "*.*", SearchOption.AllDirectories).AsEnumerable();

                    if (copyingDownTree)
                        files = files.Where(f => !f.Contains(targetDirectory));
                    //Copy all the files & Replaces any files with the same name
                    foreach (string path in files)
                    {
                        var newPath = path.Replace(DownloadService.SourceDirectory, targetDirectory);
                        Log.Information("Copying file: " + newPath);

                        File.Copy(path, newPath, true);
                        fileIndex++;
                        var progress = (double)fileIndex / totalFiles * 100.0;
                        OnProgress?.Invoke(progress);
                    }

                    CopySetup(targetDirectory);

                    return true;

                }

            }
            catch (Exception ex)
            {
                Log.Error("Error Copying files {@Error}", ex);

                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private static void CopySetup(string targetDirectory)
        {
            var setupPath = Assembly.GetExecutingAssembly().Location;
            var destPath = targetDirectory + "setup.exe";
            Log.Information("Copying file: " + destPath);

            File.Copy(setupPath, destPath, true);
        }


        private static async Task<bool> PreInstallation()
        {
            OnStepTitleChanged?.Invoke("Extract Files");

            return await DownloadService.UnpackDownload();

        }


        internal static void Cancel()
        {
            if (!CancellationTokenSource.IsCancellationRequested)
            {
                Log.Information("Cancelling");

                CancellationTokenSource.Cancel();
            }
        }
        internal static async Task<bool> StartUpdateAsync()
        {



            var targetDirectory = InstallationConfiguraion.InstallDirPath;
            OnStepTitleChanged("Downloading latest version...");
            DownloadService.DownloadPercentageChanged = ((val) => { OnProgress(val); });
            if (!await DownloadService.Download()) return false;
            OnProgress(0);
            if (DownloadService.LatestRelease.Name.Contains(InstallationConfiguraion.InstalledVersion))
                throw new ApplicationUpdateException("Latest version is already installed.");
            OnStepTitleChanged("Preparing update...");

            if (!await PreInstallation()) return false;
            if (CancellationTokenSource.IsCancellationRequested) return false;
            OnStepTitleChanged("Applying update...");
            //InstallationService.OnProgress= ((val) => { OnProgress(val); });
            // InstallationService.OnInstallationFinished= (() => { OnInstallationFinished(); });
            if (!await ApplyProgramFilesAsync(InstallationConfiguraion.InstallDirPath)) return false;
            if (CancellationTokenSource.IsCancellationRequested) return false;

            OnInstallationFinished?.Invoke();
            return true;
        }
        internal static async Task<bool> StartReparAsync()
        {



            var targetDirectory = InstallationConfiguraion.InstallDirPath;
            OnStepTitleChanged("Downloading current version...");
            DownloadService.DownloadPercentageChanged = ((val) => { OnProgress(val); });
            if (!await DownloadService.Download(InstallationConfiguraion.InstalledVersion)) return false;
            if (!await PreInstallation()) return false;
            if (CancellationTokenSource.IsCancellationRequested) return false;
            if (!await ApplyProgramFilesAsync(InstallationConfiguraion.InstallDirPath)) return false;
            if (CancellationTokenSource.IsCancellationRequested) return false;
            OnInstallationFinished?.Invoke();

            return true;
        }

        private static bool RemoveProgramFiles(string installPath)
        {
            try
            {
                var totalFiles = FileSystemService.GetFileCount(installPath);
                var fileIndex = 0;
                foreach (var subFolder in Directory.GetDirectories(installPath))
                {
                    fileIndex = DeleteFolder(subFolder, fileIndex);
                    Directory.Delete(subFolder);

                }
                foreach (var file in Directory.GetFiles(installPath))
                {
                    try
                    {
                        File.Delete(file);
                        fileIndex++;
                        var progress = (double)fileIndex / totalFiles * 100.0;

                        OnProgress?.Invoke(progress);

                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error removing application file {@File} {@Exception}", file, ex);

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error removing application files {@Exception}", ex);
            }
            return false;
        }

        private static int DeleteFolder(string subFolder, int fileIndex = 0)
        {
            foreach (var file in Directory.GetFiles(subFolder))
            {
                try
                {
                    File.Delete(file);
                    fileIndex++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            foreach (var folder in Directory.GetDirectories(subFolder))
            {
                DeleteFolder(folder, fileIndex);
                Directory.Delete(folder);
            }
            return fileIndex;
        }

        internal static Task StartUninstallAsync()
        {
            return Task.Run(() =>
            {
                var installPath = InstallationConfiguraion.ProductInformation.InstallLocation;
                RemoveProgramFiles(installPath);
                IISManager.RemoveApplication();
                ServiceManager.Uninstall();
                RegistryService.DeleteUninstallKey();

                MainWindow.EnableNext();
                return true;
            });
        }
    }
}
