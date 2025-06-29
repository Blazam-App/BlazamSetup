using BlazamSetup.Services;
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

namespace BlazamSetup
{
    /// <summary>
    /// Interaction logic for InstallStepDisplay.xaml
    /// </summary>
    public partial class InstallStepDisplay : UserControl
    {
        public InstallStepDisplay()
        {
            InitializeComponent();
            if (RegistryService.InstallationExists)
            {
                Stack.Visibility = Visibility.Collapsed;
            }
                NavigationManager.OnPageChanged += PageChanged;
            PageChanged(new Welcome());
        }

        private void PageChanged(IInstallationStep value)
        {
      
           
            foreach(var step in Stack.Children)
            {
                if (step is InstallStepControl stepDisplay) 
                {
                    stepDisplay.CurrentStep = value.Order;
                    
                }
            }
         
        }
    }
}
