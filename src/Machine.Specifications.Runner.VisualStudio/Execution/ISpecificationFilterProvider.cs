using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.VSTestAdapter.Execution
{
    public interface ISpecificationFilterProvider
    {
        IEnumerable<TestCase> FilteredTests(IEnumerable<TestCase> testCases, IRunContext runContext, IFrameworkHandle frameworkHandle);
    }
}
