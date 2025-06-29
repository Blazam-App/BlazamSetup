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

namespace BlazamSetup.Steps.Update
{
    /// <summary>
    /// Interaction logic for Repair.xaml
    /// </summary>
    public partial class Update : UserControl, IInstallationStep
    {
        private double stepProgress;
        private string currentStep;

        public Update()
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
            RunUpdate();
        }

        private async void RunUpdate()
        {
            try
            {

                _ =await  InstallationService.StartUpdateAsync();
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

        public int Order => 2;
    }
}
