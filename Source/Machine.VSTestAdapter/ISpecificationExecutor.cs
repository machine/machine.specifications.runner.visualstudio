using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;

namespace Machine.VSTestAdapter
{
    public interface ISpecificationExecutor
    {
        void RunAssembly(string source, Uri uri, IRunContext runContext, IFrameworkHandle frameworkHandle);

        void RunAssemblySpecifications(string source, Uri uri, IRunContext runContext, IFrameworkHandle frameworkHandle, IEnumerable<TestCase> specifications);
    }
}