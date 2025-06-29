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

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for Exit.xaml
    /// </summary>
    public partial class ExitStep : UserControl, IInstallationStep
    {
        public ExitStep()
        {
            InitializeComponent();
            App.Quit();
        }

        public int Order => 12;

        IInstallationStep IInstallationStep.NextStep()
        {
            throw new NotImplementedException();
        }
    }
}
