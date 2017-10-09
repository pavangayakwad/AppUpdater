using System;
using System.Collections.Generic;
using System.Text;

namespace Srushti.Updates
{
    /// <summary>
    /// Base class for serializable classes. 
    /// Any class wants to persist on xml file and restore
    /// back to object form can inherit this 'Generic' class.
    /// This class offers inbuilt functionality to 
    /// serialize/deserialize xml files.
    /// </summary>
    /// <typeparam name="T">Inheriting class</typeparam>
    internal class ConfigurationHelper<T>
    {
        /// <summary>
        /// Accepts xml file as string and cretes object from it.
        /// </summary>
        /// <param name="xmlConfigAsString">Pass complete XML string</param>
        /// <returns>Deserialized object</returns>
        public static T Load(string xmlConfigAsString)
        {
            try
            {
                System.IO.TextReader txt = new System.IO.StringReader(xmlConfigAsString);
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(T));
                T uc;
                uc = (T) x.Deserialize(txt);
                txt.Close();
                return uc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Accepts target object and creates xml of it.
        /// </summary>
        /// <param name="configObject">Serializable object</param>
        /// <param name="SaveTo">Full path with filename to save serialized object</param>
        public static void Save(T configObject, string SaveTo)
        {
            try
            {
                System.IO.StreamWriter str = new System.IO.StreamWriter(SaveTo);
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(T));
                x.Serialize(str, configObject);
                str.Close();
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
