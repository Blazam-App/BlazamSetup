using System;
using System.Security;
using System.Windows;
using Microsoft.Win32;

namespace BlazamSetup.Services
{
    internal static class RegistryService
    {
        private static RegistryKey Hive => Registry.LocalMachine;
        private static string UninstallKey => @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";
        private static string ProductUninstallKey => UninstallKey + '{' + InstallationConfiguraion.ProductGuid + @"}\";


        internal static ProductInformation GetProductInformation()
        {
            ProductInformation productInformation = new ProductInformation();
            var key = OpenKey();
            foreach (var property in typeof(ProductInformation).GetProperties())
            {
                var value = key.GetValue(property.Name);
                if (value != null)
                    switch (property.PropertyType.Name)
                    {
                        case "Int64":
                            property.SetValue(productInformation, Convert.ToInt64(value));

                            break;

                        case "Int32":
                            property.SetValue(productInformation, (int)value);

                            break;
                        case "String":
                            property.SetValue(productInformation, value);

                            break;
                    }
            }
            return productInformation;
        }

        internal static bool SetProductInformation(ProductInformation productInformation)
        {
            try
            {
                RegistryService.CreateUninstallKey();

                var key = OpenKey(true);
                foreach (var property in typeof(ProductInformation).GetProperties())
                {
                    var value = property.GetValue(productInformation);
                    if (value != null)
                        key.SetValue(property.Name, value);
                }
                return true;
            }
            catch (SecurityException)
            {
                MessageBox.Show("Registry access not authorized");
                return false;
            }
        }
        public static bool InstallationExists
        {
            get
            {
                try
                {
                    var key = OpenKey();
                    if (key == null) return false;
                    var installDate = key.GetValue("InstallDate");
                    if (installDate is string && !((string)installDate).IsNullOrEmpty())
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public static string InstallLocation
        {
            get
            {
                try
                {
                    var key = OpenKey();
                    if (key == null) return null;
                    var installLocation = key.GetValue("InstallLocation");
                    if (installLocation is string installLocationString && !installLocationString.IsNullOrEmpty())
                    {
                        return installLocationString;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public static bool CreateUninstallKey()
        {
            try
            {
                Hive.CreateSubKey(ProductUninstallKey);
                return true;
            }
            catch (Exception) { return false; }
        }

        public static bool DeleteUninstallKey()
        {
            try
            {
                Hive.DeleteSubKey(ProductUninstallKey);
                return true;
            }
            catch (Exception) { return false; }
        }
        private static RegistryKey OpenKey(bool write = false)
        {
            RegistryKey key;
            key = Hive.OpenSubKey(ProductUninstallKey, write);

            return key;
        }
    }
}
