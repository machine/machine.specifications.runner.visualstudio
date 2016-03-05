using System;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Discovery.BuiltIn;
using Machine.VSTestAdapter.Execution;

namespace Machine.VSTestAdapter
{
    public class MSpecTestAdapterFactory
    {
        private Func<ISpecificationDiscoverer> discovererCreator;
        private Func<ISpecificationExecutor> executorCreator;

        public MSpecTestAdapterFactory(Func<ISpecificationDiscoverer> discovererCreator, Func<ISpecificationExecutor> executorCreator)
        {
            if (discovererCreator == null)
                throw new ArgumentNullException(nameof(discovererCreator));

            if (executorCreator == null)
                throw new ArgumentNullException(nameof(executorCreator));

            this.discovererCreator = discovererCreator;
            this.executorCreator = executorCreator;
        }

        public MSpecTestAdapterFactory()
        {
            this.discovererCreator = () => { return new BuiltInSpecificationDiscoverer(); };
            //this.discovererCreator = () => { return new CecilSpecificationDiscoverer(); };
            
            this.executorCreator = () => { return new SpecificationExecutor(); };
        }

        public ISpecificationDiscoverer CreateDiscover()
        {
            return this.discovererCreator();
        }

        public ISpecificationExecutor CreateExecutor()
        {
            return this.executorCreator();
        }
    }
}