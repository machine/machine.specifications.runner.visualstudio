using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Machine.Specifications;
using Machine.VSTestAdapter.Discovery;

namespace Machine.VSTestAdapter.Specs.Discovery
{

    public abstract class With_DiscoverySetup<TDiscoverer> where TDiscoverer : ISpecificationDiscoverer, new()
    {
        protected static string AssemblyPath;
        protected static IEnumerable<MSpecTestCase> Results;
        protected static ISpecificationDiscoverer Discoverer;

        Establish context = () => {
            Discoverer = new TDiscoverer();
            AssemblyPath = Path.Combine(Helper.GetTestDirectory(), "SampleSpecs.dll");
        };

        Because of = () => {
            Results = Discoverer.DiscoverSpecs(AssemblyPath);
        };
    }
}