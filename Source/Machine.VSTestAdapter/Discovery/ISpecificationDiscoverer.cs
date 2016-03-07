using System.Collections.Generic;

namespace Machine.VSTestAdapter.Discovery
{
    public interface ISpecificationDiscoverer
    {
        IEnumerable<MSpecTestCase> EnumerateSpecs(string assemblyPath);

        bool SourceDirectoryContainsMSpec(string assemblyFileName);

        bool AssemblyContainsMSpecReference(string assemblyFileName);
    }
}