using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Specifications;
using Machine.VSTestAdapter.Helpers;

namespace Machine.VSTestAdapter.Execution
{
    public class AppDomainExecutor : MarshalByRefObject
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public AppDomainExecutor()
        {
        }

        public void RunAllTestsInAssembly(string pathToAssembly, ISpecificationRunListener specificationRunListener)
        {
            Assembly assemblyToRun = Assembly.LoadFrom(pathToAssembly);

            DefaultRunner mspecRunner = new DefaultRunner(specificationRunListener, RunOptions.Default);

            mspecRunner.RunAssembly(assemblyToRun);
        }

        public void RunTestsInAssembly(string pathToAssembly, IEnumerable<VisualStudioTestIdentifier> specsToRun, ISpecificationRunListener specificationRunListener)
        {
            DefaultRunner mspecRunner = null;
            Assembly assemblyToRun = null;
       
            try
            {
                assemblyToRun = Assembly.LoadFrom(pathToAssembly);
                mspecRunner = new DefaultRunner(specificationRunListener, RunOptions.Default);

                IEnumerable<Context> specificationContexts = new AssemblyExplorer().FindContextsIn(assemblyToRun) ?? Enumerable.Empty<Context>();
                Dictionary<string, Context> contextMap = specificationContexts.ToDictionary(c => c.Type.FullName, StringComparer.Ordinal);

                // We use explicit assembly start and end to wrap the RunMember loop
                mspecRunner.StartRun(assemblyToRun);

                foreach (VisualStudioTestIdentifier test in specsToRun)
                {
                    Context context = contextMap[test.ContainerTypeFullName];
                    if (context == null)
                        continue;

                    Specification specification = context.Specifications.SingleOrDefault(spec => spec.FieldInfo.Name.Equals(test.FieldName, StringComparison.Ordinal));
                    
                    if (specification is BehaviorSpecification)
                    {
                        // MSpec doesn't expose any way to run an an "It" coming from a "[Behavior]", so we have to do some trickery
                        VisualStudioTestIdentifier listenFor = specification.ToVisualStudioTestIdentifier(context);
                        DefaultRunner behaviorRunner = new DefaultRunner(new SingleBehaviorTestRunListenerWrapper(specificationRunListener, listenFor), RunOptions.Default);
                        behaviorRunner.RunMember(assemblyToRun, context.Type);
                    } 
                    else 
                    {
                        mspecRunner.RunMember(assemblyToRun, specification.FieldInfo);
                    }

                }
            } catch (Exception e) {
                specificationRunListener.OnFatalError(new ExceptionResult(e));
            }
            finally
            {
                if (mspecRunner != null && assemblyToRun != null)
                    mspecRunner.EndRun(assemblyToRun);
            }
        }
    }
}