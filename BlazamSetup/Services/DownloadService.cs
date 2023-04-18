﻿using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlazamSetup.Services
{
    internal static class DownloadService
    {

        public  static string SourceDirectory = Path.GetTempPath()+"BlazamSetup\\";
        public static string UpdateFile = SourceDirectory + "blazam.zip";
        private static ReleaseAsset latestRelease;

        public static int ExpectedSize { get; private set; }
        public static CancellationTokenSource cancellationTokenSource { get; private set; } = new CancellationTokenSource();
        public static int CompletedBytes { get; private set; }
        public static InstallEvent<int> DownloadPercentageChanged { get; set; }

        public static async Task<bool> Download()
        {
            var githubclient = new GitHubClient(new ProductHeaderValue("BLAZAM-APP"));




            //Get the releases from the repo
            var releases = await githubclient.Repository.Release.GetAll("Blazam-App", "Blazam");
            //Filter the releases to the selected branch
            var branchReleases = releases.Where(r => r.TagName.ToLower().Contains("stable"));
            //Get the first release,which should be the most recent
            latestRelease = branchReleases.FirstOrDefault()?.Assets.FirstOrDefault();
            //Get the release filename to prepare a version object
            var filename = Path.GetFileNameWithoutExtension(latestRelease.Name);
            //Create that version object
            if (filename == null) throw new ApplicationUpdateException("Filename could not be retrieved from GitHub");





            if (latestRelease != null)
            {
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(latestRelease.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            //Loggers.UpdateLogger?.Debug("Unable to connect to download url: " + response.StatusCode + " : " + response.ReasonPhrase);

                            return false;
                        }
                        if (File.Exists(UpdateFile))File.Delete(UpdateFile);
                        using (var streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        {
                            Directory.CreateDirectory(SourceDirectory);
                            File.Create(UpdateFile).Close();
                            using (var streamToWriteTo = File.OpenWrite(UpdateFile))
                            {
                                ExpectedSize = (int)latestRelease.Size;
                                var buffer = new byte[262144];
                                //var buffer = new byte[4096];
                                int bytesRead;
                                int totalBytesRead = 0;

                                while ((bytesRead = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    if (!cancellationTokenSource.IsCancellationRequested)
                                    {
                                        await streamToWriteTo.WriteAsync(buffer, 0, bytesRead);
                                        totalBytesRead += bytesRead;
                                        CompletedBytes = totalBytesRead;
                                        var percentage = (CompletedBytes / (double)ExpectedSize * 100);
                                        DownloadPercentageChanged?.Invoke((int)percentage);
                                    }
                                    else
                                    {
                                        streamToReadFrom.Close();
                                        DownloadPercentageChanged?.Invoke(0);

                                        return false;
                                    }
                                }

                                return true;


                            }
                        }
                    }
                }

                return true;
            }
                return false;
        }
        public  static void CleanDownload()
        {
            
                cancellationTokenSource.Cancel();
                Task.Run(() => {
                    int retries =5 ;
                    while (retries-->0)
                    {
                        try
                        {
                            File.Delete(UpdateFile);
                        Directory.Delete(SourceDirectory, true);
                        }
                        catch
                        {
                            Task.Delay(50).Wait();
                        }
                    }
                });
               
          
        }
    }
}