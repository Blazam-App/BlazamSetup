using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
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

        public static bool AddPermission(string path, string identity, FileSystemRights fileSystemRights)
        {

            DirectoryInfo dInfo = new DirectoryInfo(path);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(identity, fileSystemRights, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
            return true;
        }

    }
}
