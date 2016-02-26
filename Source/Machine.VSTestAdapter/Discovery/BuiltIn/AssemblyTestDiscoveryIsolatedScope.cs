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

        public AssemblyTestDiscoveryIsolatedScope(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentException($"{nameof(assemblyPath)} is null or empty.", nameof(assemblyPath));

            this.assemblyPath = assemblyPath;
        }


        private TestDiscoverer CreateTestDiscovererInstance(string assemblyPath)
        {
            appDomain = CreateAppDomain(assemblyPath);

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

        private static AppDomain CreateAppDomain(string assemblyPath)
        {
            CopyRequiredRuntimeDependencies(new[] {
                typeof(TestDiscoverer).Assembly,
                typeof(Mono.Cecil.MemberReference).Assembly,
                typeof(Mono.Cecil.Pdb.PdbReader).Assembly,
                typeof(Mono.Cecil.Mdb.MdbReader).Assembly,
                typeof(Mono.Cecil.Rocks.ILParser).Assembly,
            }, Path.GetDirectoryName(assemblyPath));

            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = "Machine.TestAdapter" + "-" + Path.GetFileNameWithoutExtension(assemblyPath);
            setup.ShadowCopyFiles = "true";
            setup.ApplicationBase = setup.PrivateBinPath = Path.GetDirectoryName(assemblyPath);

            return AppDomain.CreateDomain("discovery.dll", null, setup);
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
                AppDomain.Unload(appDomain);
                appDomain = null;
            }
        }
    }
}
