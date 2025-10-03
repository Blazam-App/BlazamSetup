using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazamSetup.Services
{
    internal static class DependencyManager
    {
        private static readonly HttpClient client = new HttpClient();
        private const string AspNetCoreRuntimeUrl = "https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.20-windows-x64-installer";
        private const string AspNetCoreHostingBundleUrl = "https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.20-windows-hosting-bundle-installer";

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
                await process.WaitForExitAsync();

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