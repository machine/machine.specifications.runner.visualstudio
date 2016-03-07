using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Machine.VSTestAdapter.Helpers
{

    public class IsolatedAppDomainExecutionScope<T> : IDisposable where T : MarshalByRefObject, new()
    {
        private AppDomain appDomain;
        private string appName = typeof(IsolatedAppDomainExecutionScope<>).Assembly.GetName().Name;
        private readonly string assemblyPath;

        public IsolatedAppDomainExecutionScope(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentException($"{nameof(assemblyPath)} is null or empty.", nameof(assemblyPath));

            this.assemblyPath = assemblyPath;
        }

        public T CreateInstance()
        {
            if (appDomain == null)
                appDomain = CreateAppDomain(assemblyPath, this.appName);

            return (T)appDomain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
        }

 
        private static AppDomain CreateAppDomain(string assemblyPath, string appName)
        {
            CopyRequiredRuntimeDependencies(new[] {
                typeof(IsolatedAppDomainExecutionScope<>).Assembly,
                typeof(Mono.Cecil.MemberReference).Assembly,
                typeof(Mono.Cecil.Pdb.PdbReader).Assembly,
                typeof(Mono.Cecil.Mdb.MdbReader).Assembly,
                typeof(Mono.Cecil.Rocks.ILParser).Assembly,
            }, Path.GetDirectoryName(assemblyPath));

            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = appName;
            setup.ShadowCopyFiles = "true";
            setup.ApplicationBase = setup.PrivateBinPath = Path.GetDirectoryName(assemblyPath);
            setup.CachePath = Path.Combine(Path.GetTempPath(), appName, Guid.NewGuid().ToString());
            setup.ConfigurationFile = Path.Combine(Path.GetDirectoryName(assemblyPath), (Path.GetFileName(assemblyPath) + ".config"));

            return AppDomain.CreateDomain($"{appName}.dll", null, setup);
        }

        private static void CopyRequiredRuntimeDependencies(IEnumerable<Assembly> assemblies, string destination)
        {
            foreach (Assembly assembly in assemblies)
            {
                string assemblyLocation = assembly.Location;
                string assemblyName = Path.GetFileName(assemblyLocation);
                string assemblyFileDestination = Path.Combine(destination, assemblyName);
                File.Copy(assemblyLocation, assemblyFileDestination, true);
            }
        }

        public void Dispose()
        {
            if (appDomain != null)
            {
                string cacheDirectory = appDomain.SetupInformation.CachePath;

                AppDomain.Unload(appDomain);
                appDomain = null;

                if (Directory.Exists(cacheDirectory))
                    Directory.Delete(cacheDirectory, true);
            }
        }
    }
}
