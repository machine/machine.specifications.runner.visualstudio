using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Machine.Specifications.VSRunner
{
    public class AppDomainExecutor : MarshalByRefObject, IAppDomainExecutor
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

        public void RunTestsInAssembly(string pathToAssembly, IEnumerable<string> specsToRun, Uri adapterUri, Func<bool> checkHasBeenCancelled, Action<string> sendErrorMessage,
            Action<string, string> recordStart,
            Action<string, string, int> recordEnd,
            Action<string, string, DateTime, DateTime, string, string, int> recordResult)
        {
            try
            {
                Assembly assemblyToRun = Assembly.LoadFrom(pathToAssembly);

                DefaultRunner mspecRunner = new DefaultRunner(new SpecificationRunListener(pathToAssembly, checkHasBeenCancelled, sendErrorMessage, recordStart, recordEnd, recordResult), RunOptions.Default);
                foreach (string spec in specsToRun)
                {
                    // get the spec type
                    string[] splits = spec.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                    string specClassName = splits[0];
                    string specFieldName = splits[1];
                    Type specType = assemblyToRun.GetType(specClassName);

                    // get the method info from the type
                    MemberInfo specField = specType.GetMembers(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public).Where(x => x.Name == specFieldName).SingleOrDefault();
                    mspecRunner.RunMember(assemblyToRun, specField);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private object CreateObject(string assemblyName, string typeName, params object[] args)
        {
            return AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName, typeName, false, 0, null, args, null, null);
        }
    }
}