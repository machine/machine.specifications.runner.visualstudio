﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.Runner.VisualStudio.Helpers;

namespace Machine.Specifications.Runner.VisualStudio.Execution
{
    public class TestExecutor
#if !NETSTANDARD
        : MarshalByRefObject
#endif
    {
        public void RunAllTestsInAssembly(string pathToAssembly, ISpecificationRunListener specificationRunListener)
        {
            var assemblyToRun = AssemblyHelper.Load(pathToAssembly);

            var mspecRunner = CreateRunner(assemblyToRun, specificationRunListener);
            mspecRunner.RunAssembly(assemblyToRun);
        }

        private DefaultRunner CreateRunner(Assembly assembly,ISpecificationRunListener specificationRunListener)
        {
            var listener = new AggregateRunListener(new[]
            {
                specificationRunListener,
                new AssemblyLocationAwareRunListener(new[] {assembly})
            });

            return new DefaultRunner(listener, RunOptions.Default);
        }

        public void RunTestsInAssembly(string pathToAssembly, IEnumerable<VisualStudioTestIdentifier> specsToRun, ISpecificationRunListener specificationRunListener)
        {
            DefaultRunner mspecRunner = null;
            Assembly assemblyToRun = null;
       
            try
            {
                assemblyToRun = AssemblyHelper.Load(pathToAssembly);
                mspecRunner = CreateRunner(assemblyToRun, specificationRunListener);

                var specsByContext = specsToRun.GroupBy(x => x.ContainerTypeFullName);

                mspecRunner.StartRun(assemblyToRun);

                foreach (var specs in specsByContext)
                {
                    var fields = specs.Select(x => x.FieldName);

                    mspecRunner.RunType(assemblyToRun, assemblyToRun.GetType(specs.Key), fields.ToArray());
                }
            }
            catch (Exception e)
            {
                specificationRunListener.OnFatalError(new ExceptionResult(e));
            }
            finally
            {
                if (mspecRunner != null && assemblyToRun != null)
                    mspecRunner.EndRun(assemblyToRun);
            }
        }

#if !NETSTANDARD
        public override object InitializeLifetimeService()
        {
            return null;
        }
#endif
    }
}
