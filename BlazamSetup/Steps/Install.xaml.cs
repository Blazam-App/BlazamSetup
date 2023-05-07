using BlazamSetup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for Install.xaml
    /// </summary>
    public partial class Install : UserControl, IInstallationStep
    {
        private string currentStep;
        private double stepProgress;

        public Install()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;
            MainWindow.NextStepButton.Visibility = Visibility.Collapsed;
            RunInstallation();
        }
        public string CurrentStep
        {
            get => currentStep; set
            {
                currentStep = value;
                CurrentDispatcher.Invoke(() => { CurrentStepLabel.Content = value; });
            }
        }

        public double StepProgress
        {
            get => stepProgress; set
            {
                stepProgress = value;
                CurrentDispatcher.Invoke(() => { InstallProgressBar.Value = value; });

            }
        }
        public Dispatcher CurrentDispatcher { get; }

        IInstallationStep IInstallationStep.NextStep()
        {
            throw new NotImplementedException();
        }

        private async void RunInstallation()
        {
            await Task.Run(() =>
            {
                PreInstallation();

                CopySourceFiles(InstallationConfiguraion.InstallDirPath + "Blazam\\");
                if (!ServiceManager.IsInstalled)
                {
                    CurrentStep = "Install Services";
                    StepProgress = 0;
                    ServiceManager.Install();
                    StepProgress = 100;
                }
                RegistryService.CreateUninstallKey();
                RegistryService.SetProductInformation(InstallationConfiguraion.ProductInformation);
            });
        }
        /// <summary>
        /// Copies the entire directory tree to another directory
        /// </summary>
        /// <param name="targetDirectory"></param>
        /// <returns></returns>
        public bool CopySourceFiles(string targetDirectory)
        {
            try
            {
                CurrentStep = "Copy Files";
                bool copyingDownTree = false;
                if (targetDirectory.Contains(DownloadService.SourceDirectory))
                {
                    copyingDownTree = true;
                }
                var totalFiles = GetFileCount(DownloadService.SourceDirectory);
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
                        StepProgress = (fileIndex / totalFiles) * 100;
                    }
                    return true;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private int GetFileCount(string sourceDirectory, int count = 0)
        {
            count += Directory.GetFiles(sourceDirectory).Count();
            foreach (var subDir in Directory.GetDirectories(sourceDirectory))
            {
                count = GetFileCount(subDir, count);
            }
            return count;

        }

        private void PreInstallation()
        {
            CurrentStep = "Extract Files";

            DownloadService.UnpackDownload();
            return;
        }
    }
}
