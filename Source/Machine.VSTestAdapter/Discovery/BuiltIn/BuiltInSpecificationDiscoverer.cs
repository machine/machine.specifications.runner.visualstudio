using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Machine.VSTestAdapter.Discovery.BuiltIn
{
    public class BuiltInSpecificationDiscoverer : ISpecificationDiscoverer
    {
        public IEnumerable<MSpecTestCase> EnumerateSpecs(string assemblyFilePath)
        {
            using (IsolatedAppDomainExecutionScope<TestDiscoverer> scope = new IsolatedAppDomainExecutionScope<TestDiscoverer>(assemblyFilePath)) {
                TestDiscoverer discoverer = scope.CreateInstance();

                return discoverer.DiscoverTests(assemblyFilePath)
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