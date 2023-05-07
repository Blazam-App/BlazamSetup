namespace BlazamSetup
{
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

        public string SqliteDirectory { get; internal set; } = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    }
}
