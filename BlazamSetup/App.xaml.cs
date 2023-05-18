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
using Serilog;
using Serilog.Events;
using System.Windows.Threading;

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

        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);
            Log.Logger = new LoggerConfiguration()
                  .Enrich.FromLogContext()
                 .Enrich.WithMachineName()
                 .Enrich.WithEnvironmentName()
                 .Enrich.WithEnvironmentUserName()
                 .Enrich.WithProperty("Application Name","Blazam Setup")

                  .WriteTo.File(InstallationConfiguraion.SetupTempDirectory + @"setuplog.txt",
                  rollingInterval: RollingInterval.Infinite,
                  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}",
                  retainedFileTimeLimit: TimeSpan.FromDays(30))
                  .WriteTo.Logger(lc =>
                  {
                      //lc.WriteTo.Console();
                      lc.Filter.ByExcluding(e => e.Level == LogEventLevel.Information).WriteTo.Console();
                  })
                  .WriteTo.Seq("http://logs.blazam.org:5341", apiKey: "S3JdoIIfIKcX4L3howh1", restrictedToMinimumLevel: LogEventLevel.Information)
                  .CreateLogger();
            SetupUnhandledExceptionHandling();
            StartupArgs = args;
           
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

        private void SetupUnhandledExceptionHandling()
        {
            // Catch exceptions from all threads in the AppDomain.
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                ShowUnhandledException(args.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException", false);

            // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
            TaskScheduler.UnobservedTaskException += (sender, args) =>
                ShowUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException", false);

            // Catch exceptions from a single specific UI dispatcher thread.
            Dispatcher.UnhandledException += (sender, args) =>
            {
                // If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
                if (!Debugger.IsAttached)
                {
                    args.Handled = true;
                    ShowUnhandledException(args.Exception, "Dispatcher.UnhandledException", true);
                }
            };

            // Catch exceptions from the main UI dispatcher thread.
            // Typically we only need to catch this OR the Dispatcher.UnhandledException.
            // Handling both can result in the exception getting handled twice.
            //Application.Current.DispatcherUnhandledException += (sender, args) =>
            //{
            //	// If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
            //	if (!Debugger.IsAttached)
            //	{
            //		args.Handled = true;
            //		ShowUnhandledException(args.Exception, "Application.Current.DispatcherUnhandledException", true);
            //	}
            //};
        }

        void ShowUnhandledException(Exception e, string unhandledExceptionType, bool promptUserForShutdown)
        {
            Log.Error("Uncaught Exception: {@Error}", e);

            var messageBoxTitle = $"Fatal Error!";
            var messageBoxMessage = $"We apologize for the error. A report has been sent to the developers.";
            var messageBoxButtons = MessageBoxButton.OK;

            if (promptUserForShutdown)
            {
                messageBoxMessage += "\n\nNormally the installer would close now. Should we close it?";
                messageBoxButtons = MessageBoxButton.YesNo;
            }

            // Let the user decide if the app should die or not (if applicable).
            if (MessageBox.Show(messageBoxMessage, messageBoxTitle, messageBoxButtons) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
