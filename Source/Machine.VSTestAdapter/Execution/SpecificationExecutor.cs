using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Machine.VSTestAdapter.Execution
{
    public class SpecificationExecutor : MarshalByRefObject, ISpecificationExecutor
    {
        private MSpecVSRunnerManager runManager;

        public SpecificationExecutor()
        {
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void RunAssembly(string source, Uri uri, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            source = Path.GetFullPath(source);
            if (!File.Exists(source))
            {
                throw new ArgumentException("Could not find file: " + source);
            }

            string assemblyFilename = source;
            string defaultConfigFile = SpecificationExecutor.GetDefaultConfigFile(source);
            runManager = new MSpecVSRunnerManager();
            runManager.RunAllTestsInAssembly(assemblyFilename, defaultConfigFile, frameworkHandle, uri);
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
            {
                return Path.GetFullPath(path);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}