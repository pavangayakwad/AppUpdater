using System;
using System.Collections.Generic;
using System.Text;

namespace Srushti.Updates
{
    [System.Xml.Serialization.XmlRoot("Product")]
    public class Product 
    {
        [System.Xml.Serialization.XmlAttribute("AppVersion")]
        public string AppVersion { get; set; }
        [System.Xml.Serialization.XmlAttribute("ProductURL")]
        public string ProductURL { get; set; }
        [System.Xml.Serialization.XmlAttribute("UpdateVersion")]
        public string UpdateVersion { get; set; }
        
        /// <summary>
        /// Online path (http://www.srushtisoft.com/updates/1_1_0/updates1_1_0.xml)
        /// to get update information for a particular version of software update.
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("UpdateFilesXMLPath")]
        public string UpdateFilesXMLPath { get; set; }

    }
}
