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
using System.Windows.Threading;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for ServicePrerequisite.xaml
    /// </summary>
    public partial class ServicePrerequisite : UserControl, IInstallationStep
    {
        public ServicePrerequisite()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;
            CheckForAspCoreRuntime();
        }

        public Dispatcher CurrentDispatcher { get; }

        public int Order => 4;

        void CheckForAspCoreRuntime()
        {

            if (PrerequisiteChecker.CheckForAspCore())
            {
                CurrentDispatcher.Invoke(() => {
                    frameworkCheckbox.IsChecked = true;
                });
            }
            else
            {
                CurrentDispatcher.Invoke(() => {
                    frameworkCheckbox.IsChecked = false;
                });
            }

        }


        IInstallationStep IInstallationStep.NextStep()
        {
            return new ConfigureService();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            Process.Start("https://aka.ms/dotnet-download");

        }   
    }
}
