using BlazamSetup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public Install()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;
            MainWindow.DisableNext();
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
        private double stepProgress;

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
            App.Quit();
            return new Welcome();
        }

        private async void RunInstallation()
        {
            await Task.Run(() =>
            {
                PreInstallation();

                CopySourceFiles(InstallationConfiguraion.InstallDirPath + "\\Blazam\\");
                if (InstallationConfiguraion.InstallationType == InstallType.Service && !ServiceManager.IsInstalled)
                {
                    CurrentStep = "Install Services";
                    StepProgress = 0;
                    ServiceManager.Install();
                    StepProgress = 100;
                }
                CurrentStep = "Finishing Installation";
                StepProgress = 0;
                //Post install steps
                CopyExampleAppSettings();
                RegistryService.CreateUninstallKey();
                RegistryService.SetProductInformation(InstallationConfiguraion.ProductInformation);
                StepProgress = 100;
                CurrentStep = "Installation Finished";
                MainWindow.DisableBack();
                MainWindow.EnableNext();

            });
        }

        private void CopyExampleAppSettings()
        {
            string exampleFilePath = InstallationConfiguraion.InstallDirPath + "\\Blazam\\appsettings.json.example";
            string filePath = InstallationConfiguraion.InstallDirPath + "\\Blazam\\appsettings.json";
            if (!File.Exists(filePath))
            {
                if (File.Exists(exampleFilePath))
                    File.Copy(exampleFilePath, filePath);
                else
                    MessageBox.Show("Example appsettings.json configuration file was not found in the installed files!");
            }

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
                        StepProgress = (fileIndex / totalFiles) * 100;
                    }
                    var setupPath = Assembly.GetExecutingAssembly().Location;
                    var destPath = targetDirectory + "setup.exe";
                    File.Copy(setupPath, destPath, true);
                    return true;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

      

        private void PreInstallation()
        {
            CurrentStep = "Extract Files";

            DownloadService.UnpackDownload();
            return;
        }
    }
}
