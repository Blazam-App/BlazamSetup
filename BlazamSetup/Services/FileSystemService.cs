using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BlazamSetup.Services
{
    internal static class FileSystemService
    {
        public static int GetFileCount(string sourceDirectory, int count = 0)
        {
            count += Directory.GetFiles(sourceDirectory).Count();
            foreach (var subDir in Directory.GetDirectories(sourceDirectory))
            {
                count = GetFileCount(subDir, count);
            }
            return count;

        }

        public static bool AddPermission(string path, SecurityIdentifier sid, FileSystemRights fileSystemRights)
        {
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(path);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();

                dSecurity.AddAccessRule(new FileSystemAccessRule(sid, fileSystemRights, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
                Log.Information($"Successfully set permission for SID '{sid}' on path '{path}'.");
                return true;
            }
            catch (Exception ex)
            {
                // Catching IdentityNotMappedException is no longer needed here as SID translation happens at the caller.
                // Callers should handle IdentityNotMappedException if they are performing translations.
                Log.Error($"Error setting permission for SID '{sid}' on path '{path}': {ex.Message}", ex);
                return false;
            }
        }

        public static bool CopyDirectory(string source, string destination, ref AppEvent<double> progressEvent)
        {

            try
            {
                Log.Information("File copy started");

                bool copyingDownTree = false;
                if (destination.Contains(source))
                {
                    copyingDownTree = true;
                }
                var totalFiles = FileSystemService.GetFileCount(source);
                var fileIndex = 0;

                if (Directory.Exists(source))
                {
                    var directories = Directory.GetDirectories(source, "*", SearchOption.AllDirectories).AsEnumerable();

                    if (copyingDownTree)
                        directories = directories.Where(d => !d.Contains(destination));

                    //Now Create all of the directories
                    foreach (string dirPath in directories)
                    {
                        Log.Information("Creating directory: " + dirPath);

                        Directory.CreateDirectory(dirPath.Replace(source, destination));
                    }
                    var files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories).AsEnumerable();

                    if (copyingDownTree)
                        files = files.Where(f => !f.Contains(destination));
                    //Copy all the files & Replaces any files with the same name
                    foreach (string path in files)
                    {
                        var newPath = path.Replace(source, destination);
                        Log.Information("Copying file: " + newPath);

                        File.Copy(path, newPath, true);
                        fileIndex++;
                        progressEvent?.Invoke((fileIndex / totalFiles) * 100);
                    }
                    return true;

                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Copying files {@Error}", ex);

                Console.WriteLine(ex.Message);
            }
            return false;
        }

        internal static string GetFileVersion(string installLocation)
        {
            try
            {
                if (File.Exists(installLocation))
                {
                    var fvi = FileVersionInfo.GetVersionInfo(installLocation);
                    string version = fvi.FileVersion+"."+fvi.ProductVersion;
                    return version; 

                }

            }
            catch (Exception ex)
            {
                Log.Error("Error getting file version {@Exception}", ex);
            }
            return "";
        }

        internal static long GetDirectorySize(string installLocation, long size =0)
        {
            var info = new DirectoryInfo(installLocation);
            size += info.EnumerateFiles().Sum(file => file.Length);
            foreach(var dir in info.EnumerateDirectories())
            {
                size += GetDirectorySize(dir.FullName);
            }

            return size;
        }
    }
}
