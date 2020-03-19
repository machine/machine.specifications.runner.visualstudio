using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using Machine.VSTestAdapter.Helpers;
using Machine.VSTestAdapter.Configuration;

namespace Machine.VSTestAdapter.Execution
{
    public interface ISpecificationExecutor
    {
        void RunAssemblySpecifications(string assemblyPath, VisualStudioTestIdentifier[] specifications, Settings settings, Uri adaptorUri, IFrameworkHandle frameworkHandle);
    }
}
