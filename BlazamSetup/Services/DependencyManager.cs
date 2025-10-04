using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

namespace BlazamSetup.Services
{
    internal static class DependencyManager
    {
        private static readonly HttpClient client = new HttpClient();
        private const string AspNetCoreRuntimeUrl = "https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/8.0.20/aspnetcore-runtime-8.0.20-win-x64.exe";
        private const string AspNetCoreHostingBundleUrl = "https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/8.0.20/dotnet-hosting-8.0.20-win.exe";

        public static async Task DownloadAndInstallAspNetCoreRuntime()
        {
            await DownloadAndInstall(AspNetCoreRuntimeUrl, "aspnetcore-runtime-8.0.20-win-x64.exe");
        }

        public static async Task DownloadAndInstallHostingBundle()
        {
            await DownloadAndInstall(AspNetCoreHostingBundleUrl, "dotnet-hosting-8.0.20-win.exe");
        }

        private static async Task DownloadAndInstall(string url, string fileName)
        {
            try
            {
                var tempPath = Path.Combine(Path.GetTempPath(), fileName);
                Log.Information($"Downloading {fileName} from {url} to {tempPath}");

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using (var fs = new FileStream(tempPath, System.IO.FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fs);
                }

                Log.Information($"Download complete. Starting silent installation of {fileName}");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = tempPath,
                        Arguments = "/install /quiet /norestart",
                        UseShellExecute = true
                    }
                };

                process.Start();
                process.WaitForExit();

                Log.Information($"{fileName} installation completed.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to download or install {fileName}");
                throw;
            }
        }
    }
}