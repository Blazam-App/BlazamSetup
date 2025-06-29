using BlazamSetup.Steps;
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

namespace BlazamSetup
{
    public enum OrderEnum
    {
        Before,
        Current,
        After
    }
    /// <summary>
    /// Interaction logic for InstallStepControl.xaml
    /// </summary>
    public partial class InstallStepControl : UserControl
    {
        private string _label;
        public string Label 
        {
            get => _label;
            set {
                _label = value;
                CurrentDispatcher?.Invoke(()=> { StepLabel.Content = _label;});
            } 
        }
        private int _currentStep;
        public int CurrentStep
        {
            get => _currentStep;
            set {
                _currentStep = value;
                CurrentDispatcher?.Invoke(()=> {
                    if (_currentStep > Order)
                    {
                        StepLabel.IsEnabled = false;
                        Radio.IsChecked = true;
                        StepLabel.Foreground = Brushes.White;
                        StepLabel.FontWeight = FontWeights.Regular;
                    }
                    else if(_currentStep == Order)
                    {
                        StepLabel.IsEnabled = true;
                        Radio.IsChecked = false;
                        StepLabel.Foreground = Brushes.White;

                        StepLabel.FontWeight = FontWeights.Bold;

                    }
                    else if (_currentStep < Order)
                    {
                        StepLabel.IsEnabled = false;
                        Radio.IsChecked = false;
                        StepLabel.Foreground = Brushes.Gray;

                        StepLabel.FontWeight = FontWeights.Regular;

                    }
                });
            } 
        }
        public int Order
        {
            get;set;
        }
        private Dispatcher CurrentDispatcher;
        public InstallStepControl()
        {
            InitializeComponent();
            CurrentDispatcher = this.Dispatcher;
            StepLabel.Content = Label;
            Radio.IsEnabled = false;
        }
    }
}
