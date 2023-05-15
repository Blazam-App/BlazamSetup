using BlazamSetup.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Diagnostics;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace BlazamSetup
{
    /// <summary>
    /// A generic app event that passes no data
    /// </summary>
    public delegate void AppEvent();
    /// <summary>
    /// A generic app event that passes variable data
    /// </summary>
    public delegate void AppEvent<T>(T value);

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public bool DidAppCrash { get; private set; }
        public ErrorReport LastCrashReport { get; private set; }
        public event AppEvent OnLastRunCrashed;
        public static StartupEventArgs StartupArgs { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            StartupArgs = e;
           
            if (!Debugger.IsAttached)
            {
                //Otherwise, make sure we are admin
                if (!IsRunningAsAdministrator())
                {
                    RestartAsAdmin();
                }

                //Start Microsoft AppCenter Analytics
                SetupAppCenter();
            }

            

        }

        private void SetupAppCenter()
        {
            try
            {
                //Maximum log levels
                AppCenter.LogLevel = LogLevel.Verbose;

                AppCenter.Start("13368723-fe6f-4e92-bdb6-89bad5181f97",
                      typeof(Analytics), typeof(Crashes));

                Task.Run(async () =>
                {
                    DidAppCrash = await Crashes.HasCrashedInLastSessionAsync();
                    LastCrashReport = await Crashes.GetLastSessionCrashReportAsync();
                    if (DidAppCrash || Debugger.IsAttached)
                    {
                        OnLastRunCrashed?.Invoke();
                    }
                    //Crashes.GenerateTestCrash();
                });
            }
            catch
            {
                //Don't crash program due to AppCenter error,ignore it
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (!Debugger.IsAttached)
                DownloadService.CleanDownload();
            base.OnExit(e);
        }


        private static void RestartAsAdmin()
        {
            try
            {
                // Setting up start info of the new process of the same application
                ProcessStartInfo processStartInfo = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName);

                // Using operating shell and setting the ProcessStartInfo.Verb to “runas” will let it run as admin
                processStartInfo.UseShellExecute = true;
                processStartInfo.Verb = "runas";

                // Start the application as new process
                Process.Start(processStartInfo);
            }
            catch { }
            // Shut down the current (old) process
            Quit();
        }

        internal static void Quit()
        {
            Application.Current.Shutdown();

        }

        /// <summary>
        /// Function that check's if current user is in Aministrator role
        /// </summary>
        /// <returns></returns>
        public static bool IsRunningAsAdministrator()
        {
            // Get current Windows user
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();

            // Get current Windows user principal
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(windowsIdentity);

            // Return TRUE if user is in role "Administrator"
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
