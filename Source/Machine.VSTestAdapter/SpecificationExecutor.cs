using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Machine.VSTestAdapter
{
    public class SpecificationExecutor : MarshalByRefObject, ISpecificationExecutor, ICancelTarget
    {
        private const int PdbHiddenLine = 16707566;
        private MSpecVSRunnerManager runManager;

        public SpecificationExecutor()
        {
        }

        public override object InitializeLifetimeService()
        {
            return (object)null;
        }

        public void RunAssembly(string source, Uri uri, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            source = Path.GetFullPath(source);
            if (!File.Exists(source))
                throw new ArgumentException("Could not find file: " + source);
            string assemblyFilename = source;
            string defaultConfigFile = SpecificationExecutor.GetDefaultConfigFile(source);

            //SpecificationRunListener specificationRunListener = new SpecificationRunListener(frameworkHandle, source, uri);
            //using (Machine.VSTestAdapter.MSpecVSRunnerManager appDomainManager = new Machine.VSTestAdapter.MSpecVSRunnerManager(str3, defaultConfigFile, true))
            //{
            //    appDomainManager.CreateAppDomainExecutor().RunAllTestsInAssembly(str3, (ISpecificationRunListener)specificationRunListener);
            //}
        }

        public void RunAssemblySpecifications(string source, Uri uri, IRunContext runContext, IFrameworkHandle frameworkHandle, IEnumerable<TestCase> specifications)
        {
            source = Path.GetFullPath(source);
            if (!File.Exists(source))
            {
                throw new ArgumentException("Could not find file: " + source);
            }

            string assemblyFilename = source;
            string defaultConfigFile = SpecificationExecutor.GetDefaultConfigFile(source);
            IEnumerable<string> specsToRun = specifications.Select(x => x.FullyQualifiedName).ToList();
            runManager = new MSpecVSRunnerManager();
            runManager.RunTestsInAssembly(assemblyFilename, defaultConfigFile, frameworkHandle, specsToRun, uri);
        }

        private static string GetDefaultConfigFile(string assemblyFile)
        {
            string path = assemblyFile + ".config";
            if (File.Exists(path))
                return Path.GetFullPath(path);
            else
                return string.Empty;
        }

        public void Cancel()
        {
            if (this.runManager != null)
            {
                this.runManager.Cancel();
            }
        }
    }
}