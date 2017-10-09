using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace Srushti.Updates
{
    [System.Xml.Serialization.XmlRoot("Files")]
    public class UpdateFiles : BaseXmlSerializer<UpdateFiles>
    {
        [System.Xml.Serialization.XmlElement("UpdateFile")]
        public List<UpdateFile> Files { get; set; }

        [System.Xml.Serialization.XmlElement("PreUpdateAssemblyPath")]
        public string PreUpdateAssemblyPath { get; set; }

        [System.Xml.Serialization.XmlElement("PostUpdateAssemblyPath")]
        public string PostUpdateAssemblyPath { get; set; }
    }
    
    public class UpdateFile 
    {
        [System.Xml.Serialization.XmlAttribute("SourceURI")]
        public string SourceURI { get; set; }

        [System.Xml.Serialization.XmlAttribute("DestinationFolder")]
        public string DestinationFolder { get; set; }
        
        [System.Xml.Serialization.XmlAttribute("FileName")]
        public string FileName { get; set; }
        
        /// <summary>
        /// An optional property to rename update file to 
        /// original extension.
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("RenameFileTo")]
        public string RenameFileTo { get; set; }
        
        [System.Xml.Serialization.XmlAttribute("MoreInfoURL")]
        public string MoreInfoURL { get; set; }
        
        [System.Xml.Serialization.XmlAttribute("ShortDescription")]
        public string ShortDescription { get; set; }
        
    }
}
