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
using static System.Net.Mime.MediaTypeNames;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for IISPrerequisiteCheck.xaml
    /// </summary>
    public partial class IISPrerequisiteCheck : UserControl, IInstallationStep
    {
        public bool FrameworkInstalled { get; set; }
        public Dispatcher CurrentDispatcher { get; }

        public IISPrerequisiteCheck()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;
            DataContext = this;
            CheckForAspCoreHosting();
        }
       
        void CheckForAspCoreHosting()
        {
            
                if (PrerequisiteChecker.CheckForAspCoreHosting())
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
            return new ConfigureIIS();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
                Process.Start("https://aka.ms/dotnet-download");
        }
    }
}
