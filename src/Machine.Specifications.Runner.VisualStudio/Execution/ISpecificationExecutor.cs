using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using Machine.VSTestAdapter.Helpers;
using Machine.VSTestAdapter.Configuration;

namespace Machine.VSTestAdapter.Execution
{
    public interface ISpecificationExecutor
    {
        void RunAssemblySpecifications(string assemblyPath, IEnumerable<VisualStudioTestIdentifier> specifications, Settings settings, Uri adapterUri, IFrameworkHandle frameworkHandle);
    }
}
