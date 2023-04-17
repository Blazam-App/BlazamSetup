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
using static System.Net.Mime.MediaTypeNames;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for IISPrerequisiteCheck.xaml
    /// </summary>
    public partial class IISPrerequisiteCheck : UserControl, IInstallationStep
    {
        public bool FrameworkInstalled { get; set; }
        public IISPrerequisiteCheck()
        {
            InitializeComponent();
            DataContext = this;
            CheckForAspCore();
        }

        void CheckForAspCore()
        {
            try
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Updates\\.NET\\");
                if (key != null)
                {
                    var possibleAspKeys = key.GetSubKeyNames();
                    if (possibleAspKeys.Length > 0)
                    {
                        foreach (var possibleKey in possibleAspKeys)
                        {
                            if (possibleKey.Contains("Microsoft .NET 6"))
                            {
                                FrameworkInstalled = true;
                            }
                        }
                    }
                }
            }
            catch { }
            FrameworkInstalled = false;

        }

        IInstallationStep IInstallationStep.NextStep()
        {
            return new ConfigureIIS();
        }
    }
}
