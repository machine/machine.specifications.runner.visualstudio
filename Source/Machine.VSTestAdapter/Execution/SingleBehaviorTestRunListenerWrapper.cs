using System;
using System.Diagnostics.Contracts;
using Machine.Specifications;
using Machine.Specifications.Runner;
using Machine.VSTestAdapter.Helpers;

namespace Machine.VSTestAdapter.Execution
{
    /// <summary>
    /// The purpose of this class is to ignore everything, but a single specification's notifications.
    /// Also because [Behavior] It's get reported as belonging to the Behavior class rather than test class
    /// we need to map from one to the other for Visual Studio to capture the results.
    /// </summary>
    public class SingleBehaviorTestRunListenerWrapper : ISpecificationRunListener
    {
        private readonly ISpecificationRunListener runListener;
        private readonly VisualStudioTestIdentifier listenFor;
        private ContextInfo currentContext;

        public SingleBehaviorTestRunListenerWrapper(ISpecificationRunListener runListener, VisualStudioTestIdentifier listenFor)
        {
            if (listenFor == null)
                throw new ArgumentNullException(nameof(listenFor));
            if (runListener == null)
                throw new ArgumentNullException(nameof(runListener));

            this.runListener = runListener;
            this.listenFor = listenFor;
        }


        public void OnContextEnd(ContextInfo context)
        {
            currentContext = null;
            runListener.OnContextEnd(context);
        }

        public void OnContextStart(ContextInfo context)
        {
            currentContext = context;
            runListener.OnContextStart(context);
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            if (listenFor != null && !listenFor.Equals(specification.ToVisualStudioTestIdentifier(currentContext)))
                return;

            runListener.OnSpecificationEnd(specification, result);
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            if (listenFor != null && !listenFor.Equals(specification.ToVisualStudioTestIdentifier(currentContext)))
                return;

            runListener.OnSpecificationStart(specification);
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            runListener.OnAssemblyEnd(assembly);
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            runListener.OnAssemblyStart(assembly);
        }

        public void OnFatalError(ExceptionResult exception)
        {
            runListener.OnFatalError(exception);
        }

        public void OnRunEnd()
        {
            runListener.OnRunEnd();
        }

        public void OnRunStart()
        {
            runListener.OnRunStart();
        }
    }
}