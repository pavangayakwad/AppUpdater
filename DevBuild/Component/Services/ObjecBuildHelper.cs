using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;
using Srushti.Updates.Contracts;

namespace Srushti.Updates
{
    internal static class ObjectBuildHelper
    {
        /// <summary>
        /// Build object from a seriable XML configuration file.
        /// </summary>
        /// <typeparam name="T">Template of object to work on</typeparam>
        /// <param name="configFilePath">Type of object to build</param>
        /// <returns>Serialized XML file path</returns>
        public static T LoadConfiguration<T>(string configFilePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFilePath);
            T config = ConfigurationHelper<T>.Load(xmlDoc.InnerXml);
            return config;
        }

        /// <summary>
        /// Save serialized object to disk
        /// </summary>
        /// <typeparam name="T">Template of object to work on</typeparam>
        /// <param name="obj">Object to save</param>
        /// <param name="filePath">Disk location</param>
        public static void SaveConfiguration<T>(T obj, string filePath)
        {
            ConfigurationHelper<T>.Save(obj, filePath);
        }

        /// <summary>
        /// Create instance of type T from an assembly.
        /// </summary>
        /// <typeparam name="T">Instance type expected</typeparam>
        /// <param name="assemblyFullPath">Assembly file from where T is expected to create</param>
        /// <returns>Instance of type T</returns>
        public static T CreateInstance<T>(string assemblyFullPath)
        {
            Assembly a = Assembly.LoadFile(assemblyFullPath);
            return (T)a.CreateInstance(typeof(T).ToString());
        }

        /// <summary>
        /// Run post updates in a seperate application domain
        /// </summary>
        /// <param name="assembly">Post update assembly full path</param>
        /// <returns>Ture if everything goes well.</returns>
        public static void RunPostUpdates(string assemblyFullPath, string className)
        {
            AppDomain domain = AppDomain.CreateDomain("PostUpdatesDomain");
            PostUpdatesBase postUpdates = null;
            try
            {
                object o = domain.CreateInstanceFromAndUnwrap(assemblyFullPath, className);
                postUpdates = o as PostUpdatesBase;
                postUpdates.PerformPostUpdateActivities();
            }
            catch(Exception outerEx) 
            {
                try
                {
                    if (postUpdates != null)
                        postUpdates.Rollback();
                }
                catch (Exception InnerEx) { throw InnerEx; }
                throw outerEx;
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

        /// <summary>
        /// Runs pre updates in a seperate application domain
        /// </summary>
        /// <param name="assembly">Pre updates assembly full path</param>
        /// <returns>True if everything goes well</returns>
        public static void RunPreUpdates(string assemblyFullPath, string className)
        {
            AppDomain domain = AppDomain.CreateDomain("PreUpdatesDomain");
            PreUpdatesBase preUpdates = null;
            try
            {
                object o = domain.CreateInstanceFromAndUnwrap(assemblyFullPath, className);
                preUpdates = o as PreUpdatesBase;
                preUpdates.PerformPreUpdateActivities();
            }
            catch(Exception outEx)
            {
                try
                {
                    if (preUpdates != null)
                        preUpdates.Rollback();
                }
                catch (Exception ex) { throw ex; }
                throw outEx;
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

    }
}
