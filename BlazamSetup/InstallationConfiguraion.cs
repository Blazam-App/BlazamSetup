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
    public enum DBType { Sqlite, SQL, MySQL }
    internal static class InstallationConfiguraion
    {
        public static WebHostConfiguration WebHostConfiguration = new WebHostConfiguration();

        public static DBType? DatabaseType { get; set; } = null;
        public static InstallType? InstallationType { get; set; } = null;
        public static string InstallDirPath { get;  set; }
        public static DatabaseConfiguration DatabaseConfiguration { get;  set; } = new DatabaseConfiguration();
    }

    public class WebHostConfiguration
    {
        public string ListeningAddress { get; set; }
        public int HttpPort { get; set; }
        public int HttpsPort { get; set; }
        public X509Certificate2 SSLCert { get;  set; }

    }

    public class DatabaseConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ValidationMessage
        {
            get
            {

                if (Server.IsNullOrEmpty())
                    return "Server must not be empty.";

                if (Port == 0)
                    return "Port must not be 0.";

                if (Database.IsNullOrEmpty())
                    return "Database must not be empty.";
                return "";
            }
        }
        public bool IsValid
        {
            get
            {
                if (!Server.IsNullOrEmpty()
                    && Port != 0
                    && !Database.IsNullOrEmpty())
                {
                    return true;
                }
                return false;
            }
        }

    }
}
