using BlazamSetup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for PostInstallation.xaml
    /// </summary>
    public partial class PostInstallation : UserControl, IInstallationStep
    {
        public PostInstallation()
        {
            InitializeComponent();
            MainWindow.DisableBack();

        }

        public int Order => 11;

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (oldParent == null)
            {
                MainWindow.EnableNext();
                MainWindow.DisableBack();
                MainWindow.SetNextText("Finish");
            }


        }

        IInstallationStep IInstallationStep.NextStep()
        {
            if (startCheckBox.IsChecked == true)
            {
                if (InstallationConfiguraion.InstallationType == InstallType.IIS)
                {
                    IISManager.Start();
                }
                else
                {
                    ServiceManager.Start();
                }
                var hostname = "localhost";
                if (InstallationConfiguraion.WebHostConfiguration.ListeningAddress != "*")
                {
                    hostname = InstallationConfiguraion.WebHostConfiguration.ListeningAddress;
                }
                Process browser = new Process();
                browser.StartInfo.CreateNoWindow = true;
                browser.StartInfo.FileName = "cmd.exe";
                if (InstallationConfiguraion.WebHostConfiguration.SSLCert != null)
                {
                    browser.StartInfo.Arguments = "/c start https://"+hostname+"/";

                }
                else
                {
                    browser.StartInfo.Arguments = "/c start http://"+hostname+"/";

                }
                browser.Start();
            }
           return new ExitStep();
        }


    }
}
