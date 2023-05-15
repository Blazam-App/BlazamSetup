using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlazamSetup.Services
{
    internal static class InstallationService
    {
        internal static AppEvent<int> OnProgress { get; set; }
        internal static AppEvent<string> OnStepTitleChanged { get; set; }
        internal static AppEvent OnInstallationFinished { get; set; }
        internal static CancellationTokenSource CancellationTokenSource { get; set; }= new CancellationTokenSource();

        internal static async Task StartInstallationAsync() {

            await Task.Run(() =>
            {
                PreInstallation();

                CopySourceFiles(InstallationConfiguraion.InstallDirPath + "\\Blazam\\");
                CreateProgramDataDirectory();
                if (InstallationConfiguraion.InstallationType == InstallType.Service && !ServiceManager.IsInstalled)
                {
                    OnStepTitleChanged?.Invoke("Install Services");
                    OnProgress?.Invoke(0);
                    ServiceManager.Install();
                    OnProgress?.Invoke(100);
                }
                else
                {
                    OnStepTitleChanged?.Invoke("Configuring IIS");
                    OnProgress?.Invoke(0);

                    IISManager.CreateApplication();
                    OnProgress?.Invoke(100);

                }
                OnStepTitleChanged?.Invoke("Finishing Installation");
                OnProgress?.Invoke(0);
                //Post install steps
                AppSettingsService.Copy();
                AppSettingsService.Configure();
                RegistryService.CreateUninstallKey();
                RegistryService.SetProductInformation(InstallationConfiguraion.ProductInformation);
                OnProgress?.Invoke(100);
                OnStepTitleChanged?.Invoke("Installation Finished");
                MainWindow.DisableBack();
                MainWindow.EnableNext();

            });
        }

        private static void CreateProgramDataDirectory()
        {
            string identity = "IIS_IUSRS";
            if (InstallationConfiguraion.InstallationType == InstallType.Service)
                identity = "NT Authority/NetworkService";

            Directory.CreateDirectory(InstallationConfiguraion.ProgramDataDir);
            FileSystemService.AddPermission(
                InstallationConfiguraion.ProgramDataDir,
                identity,
                System.Security.AccessControl.FileSystemRights.Write | System.Security.AccessControl.FileSystemRights.Modify | System.Security.AccessControl.FileSystemRights.ReadAndExecute
                );
        }


        /// <summary>
        /// Copies the entire directory tree to another directory
        /// </summary>
        /// <param name="targetDirectory"></param>
        /// <returns></returns>
        public static bool CopySourceFiles(string targetDirectory)
        {
            try
            {
                OnStepTitleChanged?.Invoke("Copy Files");
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
                        Directory.CreateDirectory(dirPath.Replace(DownloadService.SourceDirectory, targetDirectory));
                    }
                    var files = Directory.GetFiles(DownloadService.SourceDirectory, "*.*", SearchOption.AllDirectories).AsEnumerable();

                    if (copyingDownTree)
                        files = files.Where(f => !f.Contains(targetDirectory));
                    //Copy all the files & Replaces any files with the same name
                    foreach (string newPath in files)
                    {
                        File.Copy(newPath, newPath.Replace(DownloadService.SourceDirectory, targetDirectory), true);
                        fileIndex++;
                        OnProgress?.Invoke((fileIndex / totalFiles) * 100);
                    }
                    CopySetup(targetDirectory);
                    return true;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private static void CopySetup(string targetDirectory)
        {
            var setupPath = Assembly.GetExecutingAssembly().Location;
            var destPath = targetDirectory + "setup.exe";
            File.Copy(setupPath, destPath, true);
        }


        private static void PreInstallation()
        {
            OnStepTitleChanged?.Invoke("Extract Files");

            DownloadService.UnpackDownload();
            return;
        }


        internal static void Cancel()
        {
            if (!CancellationTokenSource.IsCancellationRequested)
            {
                CancellationTokenSource.Cancel();
            }
        }
    }
}
