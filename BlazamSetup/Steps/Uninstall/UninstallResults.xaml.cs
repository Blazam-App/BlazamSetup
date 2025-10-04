using System.Diagnostics;
using System.Windows.Controls;

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

        public int Order => 3;

        IInstallationStep IInstallationStep.NextStep()
        {
            // This will cause the application to quit when ExitStep is instantiated.
            // Get the path to the installation directory
            string installDir = InstallationConfiguraion.InstallDirPath;

            // Build the command to recursively delete the installation directory after a short delay
            string cmdArgs = $"/C ping 127.0.0.1 -n 2 > nul & rmdir /S /Q \"{installDir}\"";

            // Start the command process (hidden window)
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = cmdArgs,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
            return new ExitStep();
        }
    }
}
