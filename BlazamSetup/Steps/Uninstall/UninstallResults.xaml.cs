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

namespace BlazamSetup.Steps.Uninstall
{
    /// <summary>
    /// Interaction logic for UninstallResults.xaml
    /// </summary>
    public partial class UninstallResults : UserControl, IInstallationStep
    {
        public UninstallResults()
        {
            InitializeComponent();
            MainWindow.EnableNext();
            MainWindow.SetNextText("Finish");
            MainWindow.DisableBack(); // Prevent going back to the uninstall progress screen
        }

        IInstallationStep IInstallationStep.NextStep()
        {
            // This will cause the application to quit when ExitStep is instantiated.
            return new ExitStep();
        }
    }
}
