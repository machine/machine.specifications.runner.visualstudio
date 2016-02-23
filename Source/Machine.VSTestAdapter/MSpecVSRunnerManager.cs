using Machine.Specifications.Runner;
using Machine.Specifications.VSRunner;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

namespace Machine.VSTestAdapter
{
    public class MSpecVSRunnerManager : MarshalByRefObject
    {
        private const string runnerName = "Machine.Specifications.VSRunner";
        private const string runnerDllName = "Machine.Specifications.VSRunner.dll";
        private const string adapterName = "Machine.TestAdapter";
        private const string runnerClassName = "Machine.Specifications.VSRunner.AppDomainExecutor";
        private IFrameworkHandle frameworkHandle;
        private Uri uri;
        private string sourcePath;

        public MSpecVSRunnerManager()
        {
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void RunAllTestsInAssembly(string pathToAssembly, string pathToConfigFile, IFrameworkHandle frameworkHandle, Uri executorUri)
        {
            AppDomain appDomain = null;
            this.frameworkHandle = frameworkHandle;
            this.uri = executorUri;
            this.sourcePath = pathToAssembly;
            try
            {
                appDomain = this.CreateAppDomain(pathToAssembly, pathToConfigFile, true);
                IAppDomainExecutor executor = CreateAppDomainExecutor(appDomain);
                ISpecificationRunListener listener = new SpecificationRunListener(this.sourcePath, (Action<string>)SendErrorMessage,
                    (Action<string, string>)RecordStart,
                    (Action<string, string, int>)RecordEnd,
                    (Action<string, string, DateTime, DateTime, string, string, int>)RecordResult);

                executor.RunAllTestsInAssembly(pathToAssembly, listener);

                SpecificationRunListener listenerConcrete = (SpecificationRunListener)listener;
            }
            catch (Exception)
            {
            }
            finally
            {
                if (appDomain != null)
                {
                    string baseDirectory = appDomain.BaseDirectory;
                    string cachePath = appDomain.SetupInformation.CachePath;

                    AppDomain.Unload(appDomain);

                    if (Directory.Exists(cachePath))
                    {
                        Directory.Delete(cachePath, true);
                    }

                    string path = Path.Combine(baseDirectory, runnerDllName);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }
        }

        public void RunTestsInAssembly(string pathToAssembly, string pathToConfigFile, IFrameworkHandle frameworkHandle, IEnumerable<string> specsToRun, Uri executorUri)
        {
            AppDomain appDomain = null;
            this.frameworkHandle = frameworkHandle;
            this.uri = executorUri;
            this.sourcePath = pathToAssembly;
            try
            {
                appDomain = this.CreateAppDomain(pathToAssembly, pathToConfigFile, true);
                IAppDomainExecutor executor = CreateAppDomainExecutor(appDomain);
                ISpecificationRunListener listener = new SpecificationRunListener(this.sourcePath, (Action<string>)SendErrorMessage,
                    (Action<string, string>)RecordStart,
                    (Action<string, string, int>)RecordEnd,
                    (Action<string, string, DateTime, DateTime, string, string, int>)RecordResult);

                executor.RunTestsInAssembly(pathToAssembly, specsToRun, listener);

                SpecificationRunListener listenerConcrete = (SpecificationRunListener)listener;
            }
            catch (Exception)
            {
            }
            finally
            {
                if (appDomain != null)
                {
                    string baseDirectory = appDomain.BaseDirectory;
                    string cachePath = appDomain.SetupInformation.CachePath;

                    AppDomain.Unload(appDomain);

                    if (Directory.Exists(cachePath))
                    {
                        Directory.Delete(cachePath, true);
                    }

                    string path = Path.Combine(baseDirectory, runnerDllName);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }
        }

        private AppDomain CreateAppDomain(string assemblyFilename, string configFilename, bool shadowCopy)
        {
            AppDomainSetup info = new AppDomainSetup();
            info.ApplicationBase = Path.GetDirectoryName(assemblyFilename);
            info.ApplicationName = Guid.NewGuid().ToString();
            if (shadowCopy)
            {
                info.ShadowCopyFiles = "true";
                info.ShadowCopyDirectories = info.ApplicationBase;
                info.CachePath = Path.Combine(Path.GetTempPath(), info.ApplicationName);
            }
            info.ConfigurationFile = configFilename;
            return AppDomain.CreateDomain(info.ApplicationName, null, info);
        }

        private dynamic CreateAppDomainExecutor(AppDomain VSRunnerDomain)
        {
            // until the vsrunner is included with mspec we will need to ship it with the adapter and copy it into the target mspec directory
            // get the location of the adpter so we know where to copy the vsrunner from
            Assembly mspecVsTestAdapterAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith(adapterName)).SingleOrDefault();
            if (mspecVsTestAdapterAssembly == null)
            {
                throw new Exception();
            }

            // we will copy it to the vsrunner app domains base directory
            string vsRunnerDestinationFileName = Path.Combine(VSRunnerDomain.BaseDirectory, runnerDllName);

            // from the current mstestadapter domain location
            string vsRunnerSourceFileName = Path.Combine(Path.GetDirectoryName(mspecVsTestAdapterAssembly.Location), runnerDllName);
            if (File.Exists(vsRunnerDestinationFileName))
            {
                File.Delete(vsRunnerDestinationFileName);
            }
            File.Copy(vsRunnerSourceFileName, vsRunnerDestinationFileName);

            // load the vs runner assembly into the appdomain
            VSRunnerDomain.Load(runnerName);

            return VSRunnerDomain.CreateInstanceAndUnwrap(runnerName, runnerClassName, false, BindingFlags.Default, (Binder)null, new object[0], (CultureInfo)null, (object[])null);
        }

        private void SendErrorMessage(string errorMessage)
        {
            this.frameworkHandle.SendMessage(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestMessageLevel.Error, errorMessage);
        }

        private void RecordStart(string testFullyQualifiedName, string testDisplayName)
        {
            this.frameworkHandle.RecordStart(new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase(testFullyQualifiedName, uri, sourcePath) { DisplayName = testDisplayName });
        }

        private void RecordEnd(string testFullyQualifiedName, string testDisplayName, int outCome)
        {
            this.frameworkHandle.RecordEnd(new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase(testFullyQualifiedName, uri, sourcePath) { DisplayName = testDisplayName }, (TestOutcome)outCome);
        }

        private void RecordResult(string testFullyQualifiedName, string testDisplayName, DateTime startTime, DateTime endTime, string errorMessage, string errorStackTrace, int outCome)
        {
            TestCase testCase = new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase(testFullyQualifiedName, uri, sourcePath) { DisplayName = testDisplayName };
            this.frameworkHandle.RecordResult(new TestResult(testCase)
                        {
                            ComputerName = Environment.MachineName,
                            Duration = endTime - startTime,
                            EndTime = endTime,
                            ErrorMessage = errorMessage,
                            ErrorStackTrace = errorStackTrace,
                            Outcome = (TestOutcome)outCome,
                            StartTime = startTime
                        });
        }
    }
}