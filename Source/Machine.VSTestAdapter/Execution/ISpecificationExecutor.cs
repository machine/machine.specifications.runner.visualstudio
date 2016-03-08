using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using Machine.VSTestAdapter.Helpers;

namespace Machine.VSTestAdapter.Execution
{
    public interface ISpecificationExecutor
    {
        void RunAssembly(string assemblyPath, Uri adaptorUri, IRunContext runContext, IFrameworkHandle frameworkHandle);

        void RunAssemblySpecifications(string assemblyPath, IEnumerable<VisualStudioTestIdentifier> specifications, Uri adaptorUri, IRunContext runContext, IFrameworkHandle frameworkHandle);
    }
}