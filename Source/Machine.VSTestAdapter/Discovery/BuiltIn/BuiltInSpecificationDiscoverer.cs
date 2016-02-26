using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;

namespace Machine.VSTestAdapter.Discovery.BuiltIn
{
    public class BuiltInSpecificationDiscoverer : ISpecificationDiscoverer
    {
        public IEnumerable<MSpecTestCase> EnumerateSpecs(string assemblyFilePath)
        {
            using (var scope = new AssemblyTestDiscoveryIsolatedScope(assemblyFilePath)) {
                return scope.DiscoverTests();
            }
        }


        public bool SourceDirectoryContainsMSpec(string assemblyFileName)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(assemblyFileName), "Machine.Specifications.dll"));
        }

        public bool AssemblyContainsMSpecReference(string assemblyFileName)
        {
            AssemblyDefinition asmDef = AssemblyDefinition.ReadAssembly(assemblyFileName);
            foreach (AssemblyNameReference anrRef in asmDef.MainModule.AssemblyReferences)
            {
                if (anrRef.FullName.StartsWith("Machine.Specifications", StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}