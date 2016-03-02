using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Machine.VSTestAdapter.Discovery.BuiltIn
{
    public class AssemblyTestDiscoveryIsolatedScope : IDisposable
    {
        private Lazy<TestDiscoverer> testDiscoverer = new Lazy<TestDiscoverer>();
        private AppDomain appDomain;
        private readonly string assemblyPath;
        private string appName = typeof(AssemblyTestDiscoveryIsolatedScope).Assembly.GetName().Name;

        public AssemblyTestDiscoveryIsolatedScope(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentException($"{nameof(assemblyPath)} is null or empty.", nameof(assemblyPath));

            this.assemblyPath = assemblyPath;
        }


        private TestDiscoverer CreateTestDiscovererInstance(string assemblyPath)
        {
            appDomain = CreateAppDomain(assemblyPath, this.appName);

            return (TestDiscoverer)appDomain.CreateInstanceAndUnwrap(typeof(TestDiscoverer).Assembly.FullName, typeof(TestDiscoverer).FullName);
        }


        public IEnumerable<MSpecTestCase> DiscoverTests()
        {
            TestDiscoverer discoverer = CreateTestDiscovererInstance(assemblyPath);

            return discoverer.DiscoverTests(assemblyPath)
                .Select(test => new MSpecTestCase() {
                    CodeFilePath = test.CodeFilePath,
                    ContextFullType = test.ContextFullType,
                    ContextType = test.ContextType,
                    LineNumber = test.LineNumber,
                    SpecificationName = test.SpecificationName,
                    SubjectName = test.SubjectName,
                    Tags = test.Tags
                })
                .ToList();
        }

        private static AppDomain CreateAppDomain(string assemblyPath, string appName)
        {
            CopyRequiredRuntimeDependencies(new[] {
                typeof(TestDiscoverer).Assembly,
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
