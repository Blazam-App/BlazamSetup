using System;
using System.Collections.Generic;
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
    public partial class Install : UserControl,IInstallationStep
    {
        public Install()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;
            MainWindow.NextStepButton.Visibility = Visibility.Collapsed;
            StartInstallation();
        }

        public Dispatcher CurrentDispatcher { get; }

        IInstallationStep IInstallationStep.NextStep()
        {
            throw new NotImplementedException();
        }

        private async void StartInstallation()
        {
            await Task.Run(() => {
                
            
            });
        }
    }
}
