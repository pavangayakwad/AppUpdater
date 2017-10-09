using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Srushti.Updates
{
    public class ObjectHostHelper
    {
        AppDomain domain;
        private string AssemblyPath = @"C:\temp\DemoModule\DemoModule\bin\Release\DemoModule.dll";
        public object HostAssembly(string asmPath, string typeName)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Environment.CurrentDirectory;
            setup.ApplicationName = "UpdateDLLHoster";
            domain = AppDomain.CreateDomain("UpdateDLLHoster",null,setup);
            
            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            object o = domain.CreateInstanceFrom(asmPath,typeName).Unwrap();
            return o;
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly a = Assembly.LoadFile(AssemblyPath);
            return a;
        }

        public void RunIt()
        {
            try
            {
                //Assembly a = Assembly.LoadFile(@"C:\temp\DemoModule\DemoModule\bin\Release\DemoModule.dll");
                //Type t = a.GetType("Srushti.Updates.Interfaces.DemoClass");

                object o = HostAssembly(AssemblyPath, "Srushti.Updates.Interfaces.CPostUpdates");
                Srushti.Updates.Interfaces.CPostUpdates post = o;
                post.PerformUpdates();
                post.Rollback();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }
    }
}
