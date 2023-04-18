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




        public static void Next()
        {
            MainWindow.NextStepButton.Content = "Next";

            CurrentPage = CurrentPage.NextStep();
            MainWindow.InstallerFrame.Navigate(CurrentPage);

        }

        public static void Back()
        {
            try
            {
                MainWindow.NextStepButton.Content = "Next";
                MainWindow.EnableNext();
                MainWindow.NextStepButton.Visibility = System.Windows.Visibility.Visible;
                MainWindow.InstallerFrame.GoBack();
            }catch(InvalidOperationException ex)
            {

            }
        }

        public static void Quit()
        {

        }

    }
}
