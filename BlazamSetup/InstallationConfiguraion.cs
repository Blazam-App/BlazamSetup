using BlazamSetup.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BlazamSetup
{
    public enum InstallType { IIS, Service }
    internal static class InstallationConfiguraion
    {
        public static InstallType? InstallationType { get; set; } = null;
        public static X509Certificate2 SSLCert { get; internal set; }
    }
}
