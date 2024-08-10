using System;
using System.IO;

namespace BlazamSetup
{
    public class DatabaseConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password {
            get; 
            set; 
        }
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


        /// <summary>
        /// Returns a full connection string propert for the appsettings.json
        /// </summary>
        /// <returns></returns>
        public string ToAppSettingsString()
        {
            string connectionString = "Data Source=";
            if (InstallationConfiguraion.DatabaseType == DBType.Sqlite)
            {
                connectionString += SqliteDirectory + Path.DirectorySeparatorChar+ "database.db;";
            }
            else
            {
                connectionString += Server + ',' + Port + ";";
                connectionString = InsertValue(connectionString, "Database", Database);
                connectionString = InsertValue(connectionString,
                    InstallationConfiguraion.DatabaseType==DBType.MySQL?"User": "User Id",
                    Database);
                connectionString = InsertValue(connectionString, "Password", Password);

            }
            return connectionString;
        }

        private static string InsertValue(string connectionString, string name, string value)
        {
            if (!value.IsNullOrEmpty())
            {
                connectionString += name + "=" + value + ";";
            }
            return connectionString;
        }

        public string SqliteDirectory { get; internal set; } = InstallationConfiguraion.ProgramDataDir;
    }
}
