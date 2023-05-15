using BlazamSetup.Services;
using Org.BouncyCastle.Asn1.X509;
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

namespace BlazamSetup.Steps.Uninstall
{
    /// <summary>
    /// Interaction logic for Uninstall.xaml
    /// </summary>
    public partial class Uninstall : UserControl, IInstallationStep
    {
        public Uninstall()
        {
            InitializeComponent();
            CurrentDispatcher = this.Dispatcher;
            RunUninstall();
        }

        private void RunUninstall()
        {
            Task.Run(() =>
            {
                var installPath = InstallationConfiguraion.ProductInformation.InstallLocation;
                RemoveApplicationFiles(installPath);
                RegistryService.DeleteUninstallKey();
                ServiceManager.Uninstall();
                MainWindow.EnableNext();
                return true;
            });
        }

        private void RemoveApplicationFiles(string installPath)
        {
            var totalFiles = FileSystemService.GetFileCount(installPath);
            var fileIndex = 0;
            foreach (var subFolder in Directory.GetDirectories(installPath))
            {
                fileIndex=DeleteFolder(subFolder, fileIndex);
                Directory.Delete(subFolder);
                StepProgress = fileIndex / totalFiles * 100;

            }
            foreach (var file in Directory.GetFiles(installPath))
            {
                try
                {
                    File.Delete(file);
                    fileIndex++;
                    StepProgress = fileIndex / totalFiles * 100;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private int DeleteFolder(string subFolder,int fileIndex=0)
        {
            foreach(var file in Directory.GetFiles(subFolder))
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
            foreach(var folder in Directory.GetDirectories(subFolder))
            {
                DeleteFolder(folder, fileIndex);
                Directory.Delete(folder);
            }
            return fileIndex;
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
            throw new NotImplementedException();
        }
    }
}
