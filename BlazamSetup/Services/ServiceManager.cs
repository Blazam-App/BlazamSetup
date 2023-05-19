using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BlazamSetup.Services
{
    internal static class ServiceManager
    {
        internal static string ServiceName = "Blazam";

        private static string tempNSSMPath = Path.GetFullPath(InstallationConfiguraion.SetupTempDirectory + Path.DirectorySeparatorChar + "nssm.exe");

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
                CreateTempNSSM();
                var path = InstallationConfiguraion.InstallDirPath + "\\Blazam\\nssm.exe";

                var arguments = "install " + ServiceName + " \"" + InstallationConfiguraion.InstallDirPath + "\\Blazam\\blazam.exe\"";
                Process.Start(path, arguments).WaitForExit();
                arguments = "set " + ServiceName + " ObjectName \"NT AUTHORITY\\Network Service\" \"\"";
                Process.Start(path, arguments).WaitForExit();


            }
            File.Delete(tempNSSMPath);

            return true;
        }

        internal static bool Uninstall()
        {
          

            if (IsInstalled)
            {
                CreateTempNSSM();
                Stop();
                Process.Start(
                    tempNSSMPath,
                    "remove " + ServiceName + " confirm").WaitForExit();

            }
            File.Delete(tempNSSMPath);

            return true;
        }

        private static void CreateTempNSSM()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var nssm = Properties.Resources.nssm;
            if (File.Exists(tempNSSMPath))
                File.Delete(tempNSSMPath);


            var fileWriteStream = File.Create(tempNSSMPath);
            //MemoryStream ms = new MemoryStream();
            foreach (var bite in nssm)
            {
                fileWriteStream.WriteByte((byte)bite);
            }
            fileWriteStream.Close();
        }

        internal static bool Start()
        {
            var service = BlazamServiceController;
            if (service.Status != ServiceControllerStatus.StartPending && service.Status != ServiceControllerStatus.Running)

                service.Start();

            return true;
        }
        internal static bool Stop()
        {
            var service = BlazamServiceController;
            if (service.Status != ServiceControllerStatus.Stopped && service.Status != ServiceControllerStatus.StopPending)
                service.Stop();
            return true;
        }
    }
}
