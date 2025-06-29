using BlazamSetup.Services;
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

namespace BlazamSetup.Steps.Repair
{
    /// <summary>
    /// Interaction logic for Repair.xaml
    /// </summary>
    public partial class Repair : UserControl, IInstallationStep
    {
        private double stepProgress;
        private string currentStep;

        public Repair()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;
            MainWindow.DisableNext();
            InstallationService.OnProgress += (value) => StepProgress = value;
            InstallationService.OnStepTitleChanged += (value) => CurrentStep = value;
            InstallationService.OnInstallationFinished += () => {
                MainWindow.EnableNext();
                CurrentStepLabel.Content = "Finished";
                };
            RunRepair();
        }

        private async void RunRepair()
        {
            try
            {

                _ =await  InstallationService.StartReparAsync();
            }catch (Exception ex)
            {
                CurrentStepLabel.Content = "Failed";
                errorLabel.Content = ex.Message;
            }
        }

        IInstallationStep IInstallationStep.NextStep()
        {
            return new ExitStep();

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
                CurrentDispatcher.Invoke(() => { RepairProgressBar.Value = value; });

            }
        }
        public Dispatcher CurrentDispatcher { get; }

        public int Order => 1;
    }
}
