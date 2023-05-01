using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BlazamSetup.Services
{
    internal static class ServiceManager
    {
        internal static string ServiceName = "Blazam";

        public static ServiceController BlazamServiceController => new ServiceController(ServiceName, "localhost");

        internal static bool IsInstalled => ServiceStatus != null;

        internal static bool IsStopped => ServiceStatus == ServiceControllerStatus.Stopped;

        private static ServiceControllerStatus? ServiceStatus
        {
            get
            {
                ServiceController controller = BlazamServiceController;

                try
                {
                    var status = controller.Status;
                    return status;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
                finally
                {
                    if (controller != null)
                        controller.Dispose();
                }
            }
        }
        internal static bool Install()
        {
            if (!IsInstalled)
            {
                var path = InstallationConfiguraion.InstallDirPath + "\\Blazam\\nssm.exe";
                var arguments = "install " + ServiceName + " \"" + InstallationConfiguraion.InstallDirPath + "\\Blazam\\blazam.exe\"";
                Process.Start(path,arguments);
            }
            return true;
        }

        internal static bool Uninstall()
        {
            if (IsInstalled)
            {
                Process.Start(
                    InstallationConfiguraion.InstallDirPath + "\\Blazam\nssm.exe",
                    "remove " + ServiceName + " confirm");
            }
            return true;
        }

        internal static bool Start()
        {
            var service = BlazamServiceController;
            service.Start();

            return true;
        }
        internal static bool Stop()
        {
            var service = BlazamServiceController;
            service.Stop();
            return true;
        }
    }
}
