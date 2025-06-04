using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows;

namespace SFS_Tool_Management.Repositories
{
    static class Config
    {
        public static void Save(string propertyName, string propertyValue)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var appSettings = configFile.AppSettings.Settings;

                if (appSettings[propertyName] == null)
                {
                    appSettings.Add(propertyName, propertyValue);
                }
                else
                {
                    appSettings.Remove(propertyName);
                    appSettings.Add(propertyName, propertyValue);
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException e)
            {
                MessageBox.Show($"Configuration error:\r\n{e}\r\nProperty: {propertyName}={propertyValue}",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string Load(string propertyName)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[propertyName] ?? string.Empty;
            }
            catch (ConfigurationErrorsException e)
            {
                MessageBox.Show($"Configuration error:\r\n{e}\r\nProperty: {propertyName}",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }
    }
}