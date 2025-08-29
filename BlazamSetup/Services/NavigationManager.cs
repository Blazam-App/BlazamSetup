using System;
using BlazamSetup.Steps;

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
            catch (InvalidOperationException)
            {
                // No more pages in back navigation history
            }
        }

    }
}
