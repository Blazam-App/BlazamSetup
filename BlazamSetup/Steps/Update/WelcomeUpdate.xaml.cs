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

namespace BlazamSetup.Steps.Update
{
    /// <summary>
    /// Interaction logic for WelcomeRepair.xaml
    /// </summary>
    public partial class WelcomeUpdate : UserControl, IInstallationStep
    {
        public WelcomeUpdate()
        {
            InitializeComponent();
        }

        public int Order => 1;

        IInstallationStep IInstallationStep.NextStep()
        {
            return new Update();
        }
    }
}
