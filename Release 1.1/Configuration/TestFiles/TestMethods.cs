using System;
using System.Collections.Generic;
using System.Text;

namespace Srushti.Updates
{
    public class TestMethods
    {
        public void Test()
        {
            UpdateFiles ufc = new UpdateFiles();
            ufc.Files = new List<UpdateFile>();
            ufc.Files.Add(new UpdateFile { FileName = "qqupdate.xml", DestinationFolder = @"c:\log", RenameFileTo = "rename1.txt", SourceURI = "http://srushtisoft.com/updates/qq/gen/", MoreInfoURL = "http://srushtisoft.com/updates/qq/gen/qqupdate.xml", ShortDescription = "quickinfo" });
            ufc.Files.Add(new UpdateFile { FileName = "qqupdate.xml", DestinationFolder = @"c:\log", RenameFileTo = "rename2.txt", SourceURI = "http://srushtisoft.com/updates/qq/gen/", MoreInfoURL = "http://srushtisoft.com/updates/qq/gen/qqupdate.xml", ShortDescription = "quickinfo" });
            ufc.Files.Add(new UpdateFile { FileName = "qqupdate.xml", DestinationFolder = @"c:\log", RenameFileTo = "rename3.txt", SourceURI = "http://srushtisoft.com/updates/qq/gen/", MoreInfoURL = "http://srushtisoft.com/updates/qq/gen/qqupdate.xml", ShortDescription = "quickinfo" });
            ufc.Files.Add(new UpdateFile { FileName = "qqupdate.xml", DestinationFolder = @"c:\log", RenameFileTo = "rename4.txt", SourceURI = "http://srushtisoft.com/updates/qq/gen/", MoreInfoURL = "http://srushtisoft.com/updates/qq/gen/qqupdate.xml", ShortDescription = "quickinfo" });
            ufc.Files.Add(new UpdateFile { FileName = "qqupdate.xml", DestinationFolder = @"c:\log", RenameFileTo = "rename5.txt", SourceURI = "http://srushtisoft.com/updates/qq/gen/", MoreInfoURL = "http://srushtisoft.com/updates/qq/gen/qqupdate.xml", ShortDescription = "quickinfo" });
            ufc.PreUpdateAssemblyPath = "preupdat.dll";
            ufc.PostUpdateAssemblyPath = "postupdate.dll";
            UpdateFiles.Save(ufc, "c:\\log\\conf.xml");

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load("c:\\log\\conf.xml");

            UpdateFiles temp = UpdateFiles.Load(doc.InnerXml) as UpdateFiles;

            Product p = new Product();
            p.AppVersion = "1.0.0";
            p.ProductURL = "https://kk.com";
            p.UpdateFilesXMLPath = "http://kk.com/kk.xml";
            p.UpdateVersion = "1.1.0";
            Product.Save(p, "c:\\log\\prd.xml");

            doc = new System.Xml.XmlDocument();
            doc.Load("c:\\log\\prd.xml");
            p = Product.Load(doc.InnerXml) as Product;
        }
    }
}
