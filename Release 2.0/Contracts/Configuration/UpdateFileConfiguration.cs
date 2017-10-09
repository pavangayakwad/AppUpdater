using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace Srushti.Updates
{
    [System.Xml.Serialization.XmlRoot("Files")]
    public class UpdateFiles 
    {
        [System.Xml.Serialization.XmlElement("UpdateFile")]
        public List<UpdateFile> Files { get; set; }

        private CustomUpdate _preUpdate = new CustomUpdate();
        [System.Xml.Serialization.XmlElement("PreUpdate")]
        public CustomUpdate PreUpdate { get { return _preUpdate; } set { _preUpdate = value; } }

        private CustomUpdate _postUpdate = new CustomUpdate();
        [System.Xml.Serialization.XmlElement("PostUpdate")]
        public CustomUpdate PostUpdate { get { return _postUpdate; } set { _postUpdate = value; } }
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

    public class CustomUpdate
    {
        [System.Xml.Serialization.XmlAttribute("AssemblyPath")]
        public string AssemblyPath { get; set; }

        [System.Xml.Serialization.XmlAttribute("FullyQualifiedName")]
        public string FullyQualifiedName { get; set; }
    }
}
