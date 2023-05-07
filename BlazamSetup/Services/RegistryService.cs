using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazamSetup.Services
{
    internal static class RegistryService
    {
        private static RegistryKey Hive => Registry.LocalMachine;
        private static string UninstallKey => @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";
        private static string ProductUninstallKey => UninstallKey +'{'+ InstallationConfiguraion.ProductGuid + @"}\";


        internal static ProductInformation GetProductInformation()
        {
            ProductInformation productInformation = new ProductInformation();
            var key = OpenKey();
            foreach (var property in typeof(ProductInformation).GetProperties())
            {
                var value = key.GetValue(property.Name);
                if (value != null)
                    property.SetValue(productInformation, value);
            }
            return productInformation;
        }

        internal static bool SetProductInformation(ProductInformation productInformation)
        {
            var key = OpenKey(true);
            foreach (var property in typeof(ProductInformation).GetProperties())
            {
                var value = property.GetValue(productInformation);
                if (value != null)
                    key.SetValue(property.Name, value);
            }
            return true;
        }
        public static bool InstallationExists
        {
            get
            {
                try
                {
                    var key = OpenKey();
                    if(key==null) return false;
                    var installDate = key.GetValue("InstallDate");
                    if(installDate is int && (int)installDate != 0)
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    return false;
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
            catch (Exception e) { return false; }
        }
        private static RegistryKey OpenKey(bool write = false)
        {
            RegistryKey key;
            key = Hive.OpenSubKey(ProductUninstallKey, write);

            return key;
        }
    }
}
