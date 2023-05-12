using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Web.Administration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlazamSetup.Services
{
    internal static class AppSettingsService
    {
        public static bool Configure()
        {

            string path = InstallationConfiguraion.InstallDirPath + @"Blazam\\";
            

            string jsonString = System.IO.File.ReadAllText(path+"appsettings.json");


            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new ExpandoObjectConverter());
            jsonSettings.Converters.Add(new StringEnumConverter());

            dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(jsonString, jsonSettings);

            config.EncryptionKey = RandomString();
            config.HTTPPort = InstallationConfiguraion.WebHostConfiguration.HttpPort.ToString();
            config.HTTPSPort = InstallationConfiguraion.WebHostConfiguration.HttpsPort.ToString();
            config.ListeningAddress = InstallationConfiguraion.WebHostConfiguration.ListeningAddress;
            config.DatabaseType = InstallationConfiguraion.DatabaseType.ToString();
            config.ConnectionStrings.DBConnectionString = InstallationConfiguraion.DatabaseConfiguration.ToAppSettingsString();
            var newJson = JsonConvert.SerializeObject(config, Formatting.Indented, jsonSettings);
            File.WriteAllText(Path.Combine(path, "appsettings.json"), newJson.ToString());


            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(path)
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            //var configuration = builder.Build();
            //configuration["EncryptionKey"] = RandomString();
            //configuration["HTTPPort"] = InstallationConfiguraion.WebHostConfiguration.HttpPort.ToString();
            //configuration["HTTPPort"] = InstallationConfiguraion.WebHostConfiguration.HttpsPort.ToString();
            //configuration["ListeningAddress"] = InstallationConfiguraion.WebHostConfiguration.ListeningAddress;
            //configuration["DatabaseType"] = InstallationConfiguraion.DatabaseType.ToString();
            //configuration["ConnectionStrings:DBConnectionString"] = InstallationConfiguraion.DatabaseConfiguration.ToAppSettingsString();
            ////File.WriteAllText(Path.Combine(path, "appsettings.json"), configuration.ToString());
            //// Save changes to appsettings.json
            //using (var stream = new FileStream("appsettings.json", FileMode.OpenOrCreate))
            //{
            //    var json = JsonConvert.SerializeObject(configuration.AsEnumerable().ToList(), Formatting.Indented);
            //    var str = configuration.ToString();
            //    //configuration.Save(stream);
            //}
            return true;
        }

        internal static void Copy()
        {
            string exampleFilePath = InstallationConfiguraion.InstallDirPath + "\\Blazam\\appsettings.example.json";
            string filePath = InstallationConfiguraion.InstallDirPath + "\\Blazam\\appsettings.json";
         
                if (File.Exists(exampleFilePath))
                    File.Copy(exampleFilePath, filePath,true);
                else
                    MessageBox.Show("Example appsettings.json configuration file was not found in the installed files!");
            
        }

        private static string RandomString()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var randomString = new string(Enumerable.Repeat(chars, 32)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }
    }
}
