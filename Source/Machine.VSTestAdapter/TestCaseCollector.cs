using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;
using System.Linq;

namespace Machine.VSTestAdapter
{
    public class TestCaseCollector : ITestCaseDiscoverySink
    {
        private List<TestCase> testCases = new List<TestCase>();

        public IEnumerable<TestCase> TestCases { get { return this.testCases.Select(x => x); } }

        public void SendTestCase(TestCase discoveredTest)
        {
            this.testCases.Add(discoveredTest);
        }
    }
}