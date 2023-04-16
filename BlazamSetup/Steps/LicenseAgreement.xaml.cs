using BlazamSetup.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for LicenseAgreement.xaml
    /// </summary>
    public partial class LicenseAgreement : UserControl, IInstallationStep
    {
        private DownloadInstallation _downloadInstaller;

        public LicenseAgreement()
        {
            InitializeComponent();
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("license.rtf"));
                LicenseTextBox.Selection.Load(reader.BaseStream, DataFormats.Rtf);
            }
            catch
            {

            }

        }

        IInstallationStep IInstallationStep.NextStep()
        {
            if (_downloadInstaller == null)
                _downloadInstaller = new DownloadInstallation();

            return _downloadInstaller;

        }
    }
}
