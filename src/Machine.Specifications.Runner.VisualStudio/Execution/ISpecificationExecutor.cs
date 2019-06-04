using System;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Execution
{
    public interface ISpecificationExecutor
    {
        void RunAssembly(string assemblyPath, Settings settings, Uri adaptorUri, IFrameworkHandle frameworkHandle);

        void RunAssemblySpecifications(
            string assemblyPath,
            VisualStudioTestIdentifier[] specifications,
            Settings settings,
            Uri adaptorUri,
            IFrameworkHandle frameworkHandle);
    }
}
