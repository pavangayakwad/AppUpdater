using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Configuration;

namespace Srushti.Updates
{
    public static class ConfigurationsHelper
    {
        /// <summary>
        /// Get list of update files from update server.
        /// </summary>
        /// <returns>True if update files configuration is to 
        /// read from update server.</returns>
        private static T LoadConfiguration<T>(string configFilePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFilePath);
            T config = BaseXmlSerializer<T>.Load(xmlDoc.InnerXml);
            return config;
        }

        /// <summary>
        /// Load latest configuration from update server. 
        /// </summary>
        public static Product LoadNewProductConfiguration()
        {
            if (!File.Exists(ConfigurationManager.AppSettings["LatestProductConfigFilePath"]))
                throw new Exception("Update configuration file missing on update server.");

            return LoadConfiguration<Product>(ConfigurationManager.AppSettings["LatestProductConfigFilePath"]);
        }

        /// <summary>
        /// Loads local configuration from user computer. 
        /// </summary>
        public static Product LoadLocalProductConfiguration()
        {
            if (!File.Exists(ConfigurationManager.AppSettings["LocalProductConfigCacheFilePath"]))
                throw new Exception("Local update configuration file missing");

            return LoadConfiguration<Product>(ConfigurationManager.AppSettings["LocalProductConfigCacheFilePath"]);
        }

        /// <summary>
        /// Loads update files list from update server. 
        /// </summary>
        public static UpdateFiles LoadUpdatesConfiguration(string fullPath)
        {
            UpdateFiles config =
                LoadConfiguration<UpdateFiles>(fullPath);
            return config;
        }

       


    }
}
