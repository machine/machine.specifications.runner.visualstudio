using System;

namespace Machine.VSTestAdapter
{
    public class MSpecTestAdapterFactory
    {
        private Func<ISpecificationDiscoverer> discovererCreator;
        private Func<ISpecificationExecutor> executorCreator;

        public MSpecTestAdapterFactory(Func<ISpecificationDiscoverer> discovererCreator, Func<ISpecificationExecutor> executorCreator)
        {
            if (discovererCreator == null)
            {
                throw new ArgumentNullException("discovererCreator");
            }

            if (executorCreator == null)
            {
                throw new ArgumentNullException("executorCreator");
            }

            this.discovererCreator = discovererCreator;
            this.executorCreator = executorCreator;
        }

        public MSpecTestAdapterFactory()
        {
            this.discovererCreator = () => { return new SpecificationDiscoverer(); };
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