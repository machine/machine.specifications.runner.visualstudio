using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public void RunTestsInAssembly(string pathToAssembly, IEnumerable<string> specsToRun, ISpecificationRunListener specificationRunListener)
        {
            DefaultRunner mspecRunner = null;
            dynamic dynMSpeccRunner = null;
            Assembly assemblyToRun = null;
            bool canIndicateStartAndEnd = false;

            // determine the mspec version, if its greater or equal to 0.5.12 we can call the start and endrun methods in mspec
            string pathToMSpec = Path.Combine(Path.GetDirectoryName(pathToAssembly), "Machine.Specifications.dll");
            if (File.Exists(pathToMSpec))
            {
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(pathToMSpec);
                if (fileInfo.FileMinorPart > 5)
                {
                    canIndicateStartAndEnd = true;
                }
                else
                    if (fileInfo.FileMinorPart == 5 && fileInfo.FileBuildPart >= 12)
                    {
                        canIndicateStartAndEnd = true;
                    }
            }
            try
            {
                assemblyToRun = Assembly.LoadFrom(pathToAssembly);
                mspecRunner = new DefaultRunner(specificationRunListener, RunOptions.Default);
                dynMSpeccRunner = mspecRunner;
                if (canIndicateStartAndEnd)
                {
                    dynMSpeccRunner.StartRun(assemblyToRun);
                }
                foreach (string spec in specsToRun)
                {
                    // get the spec type
                    string[] splits = spec.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                    string specClassName = splits[0];
                    string specFieldName = splits[1];

                    Type specType = assemblyToRun.GetType(specClassName);
                    if (specType != null)
                    {
                        // get the method info from the type
                        MemberInfo specField = specType.GetMembers(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public).Where(x => x.Name == specFieldName).SingleOrDefault();
                        if (specField != null)
                            mspecRunner.RunMember(assemblyToRun, specField);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (mspecRunner != null && assemblyToRun != null)
                {
                    if (canIndicateStartAndEnd)
                    {
                        dynMSpeccRunner.EndRun(assemblyToRun);
                    }
                    mspecRunner = null;
                    dynMSpeccRunner = null;
                    assemblyToRun = null;
                }
            }
        }
    }
}