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
        private readonly VisualStudioTestIdentifier mapTo;

        public SingleBehaviorTestRunListenerWrapper(ISpecificationRunListener runListener, VisualStudioTestIdentifier listenFor, VisualStudioTestIdentifier mapTo)
        {
            Contract.Requires(mapTo != null);
            if (listenFor == null)
                throw new ArgumentNullException(nameof(listenFor));
            if (runListener == null)
                throw new ArgumentNullException(nameof(runListener));

            this.runListener = runListener;
            this.listenFor = listenFor;
            this.mapTo = mapTo;
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            if (listenFor != null && !listenFor.Equals(specification.ToVisualStudioTestIdentifier()))
                return;

            runListener.OnSpecificationEnd(new SpecificationInfo(specification.Leader, specification.Name, mapTo.ContainerTypeFullName, mapTo.FieldName) {
                CapturedOutput = specification.CapturedOutput,
            }, result);
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            if (listenFor != null && !listenFor.Equals(specification.ToVisualStudioTestIdentifier()))
                return;

            runListener.OnSpecificationStart(new SpecificationInfo(specification.Leader, specification.Name, mapTo.ContainerTypeFullName, mapTo.FieldName) {
                CapturedOutput = specification.CapturedOutput,
            });
        }


        #region Stubs
             public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            runListener.OnAssemblyEnd(assembly);
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            runListener.OnAssemblyStart(assembly);
        }

        public void OnContextEnd(ContextInfo context)
        {
            runListener.OnContextEnd(context);
        }

        public void OnContextStart(ContextInfo context)
        {
            runListener.OnContextStart(context);
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
        #endregion
    }
}