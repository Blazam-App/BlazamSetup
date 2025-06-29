using BlazamSetup.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BlazamSetup.Services
{
    public static class NavigationManager
    {
        internal static IInstallationStep CurrentPage { get; set; } = new Welcome();

        internal static AppEvent<IInstallationStep> OnPageChanged { get; set; }


        public static void Next()
        {
            MainWindow.SetNextText("Next");

            CurrentPage = CurrentPage.NextStep();
            MainWindow.InstallerFrame.Navigate(CurrentPage);
            OnPageChanged?.Invoke(CurrentPage);

        }

        public static void Back()
        {
            try
            {
                MainWindow.SetNextText("Next");
                MainWindow.EnableNext();
                MainWindow.NextStepButton.Visibility = System.Windows.Visibility.Visible;
                MainWindow.InstallerFrame.GoBack();
                OnPageChanged?.Invoke(CurrentPage);

            }
            catch (InvalidOperationException ex)
            {

            }
        }

        public static void Quit()
        {

        }

    }
}
