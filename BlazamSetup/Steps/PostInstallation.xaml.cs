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
    public partial class PostInstallation : UserControl,IInstallationStep
    {
        public PostInstallation()
        {
            InitializeComponent();
            MainWindow.DisableBack();

        }
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

                Process browser = new Process();
                browser.StartInfo.CreateNoWindow = true;
                browser.StartInfo.FileName = "cmd.exe";
                browser.StartInfo.Arguments = "/c start http://localhost/";
                browser.Start();
            }
           return new ExitStep();
        }


    }
}
