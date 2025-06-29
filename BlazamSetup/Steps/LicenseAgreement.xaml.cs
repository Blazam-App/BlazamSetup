using BlazamSetup.Properties;
using BlazamSetup.Services;
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
            MainWindow.DisableNext();

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                var license = Properties.Resources.license;
                MemoryStream ms = new MemoryStream();
                foreach (var bite in license)
                {
                    ms.WriteByte((byte)bite);
                }

                LicenseTextBox.Selection.Load(ms, DataFormats.Rtf);
                LicenseTextBox.LayoutUpdated += (s, e) =>
                {
                    if (NavigationManager.CurrentPage == this)
                    {
                        var tes = ScrollBox.VerticalOffset;
                        if (ScrollBox.ContentVerticalOffset >= ScrollBox.ActualHeight - 20)
                        {
                            if (!MainWindow.NextStepButton.IsEnabled)
                            {
                                MainWindow.EnableNext();
                            }
                        }
                    }
                };
            }
            catch
            {

            }

        }

        public int Order => 1;

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (oldParent == null)
            {
                MainWindow.SetNextText("I Agree");
                MainWindow.SetActionLabel("Scroll to bottom of license to agree");
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
