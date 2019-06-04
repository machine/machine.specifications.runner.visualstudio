using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Machine.Specifications.Runner.VisualStudio.Helpers
{
#if !NETSTANDARD
    public class IsolatedAppDomainExecutionScope<T> : IDisposable
        where T : MarshalByRefObject, new()
    {
        private AppDomain appDomain;
        private string appName = typeof(IsolatedAppDomainExecutionScope<>).Assembly.GetName().Name;
        private readonly string assemblyPath;

        public IsolatedAppDomainExecutionScope(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentException($"{nameof(assemblyPath)} is null or empty.", nameof(assemblyPath));

            this.assemblyPath = assemblyPath;
        }

        public T CreateInstance()
        {
            if (appDomain == null)
            {
                // Because we need to copy files around - we create a global cross-process mutex here to avoid multi-process race conditions
                // in the case where both of those are true:
                //  1. VSTest is told to run tests in parallel, so it spawns multiple processes
                //  2. There are multiple test assemblies in the same directory
                using (var mutex = new Mutex(false, $"{appName}_{Path.GetDirectoryName(assemblyPath).Replace(Path.DirectorySeparatorChar, '_')}"))
                {
                    try
                    {
                        mutex.WaitOne(TimeSpan.FromMinutes(1));
                    }
                    catch (AbandonedMutexException)
                    {
                    }

                    try
                    {
                        appDomain = CreateAppDomain(assemblyPath, appName);
                    }
                    finally
                    {
                        try
                        {
                            mutex.ReleaseMutex();
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return (T) appDomain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
        }


        private static AppDomain CreateAppDomain(string assemblyPath, string appName)
        {
            // This is needed in the following two scenarios, so that the target test dll and its dependencies are loaded correctly:
            //
            // 1. pre-.NET Standard (old) .csproj and Visual Studio IDE Test Explorer run
            // 2. vstest.console.exe run against .dll which is not in the build output folder (e.g. packaged build artifact)
            // 
            CopyRequiredRuntimeDependencies(new[] {
                typeof(IsolatedAppDomainExecutionScope<>).Assembly,
                typeof(Mono.Cecil.MemberReference).Assembly,
                typeof(Mono.Cecil.Pdb.PdbReaderProvider).Assembly,
                typeof(Mono.Cecil.Mdb.MdbReaderProvider).Assembly,
                typeof(Mono.Cecil.Rocks.IILVisitor).Assembly,
            }, Path.GetDirectoryName(assemblyPath));

            var setup = new AppDomainSetup
            {
                ApplicationName = appName,
                ShadowCopyFiles = "true",
                ApplicationBase = Path.GetDirectoryName(assemblyPath),
                PrivateBinPath = Path.GetDirectoryName(assemblyPath),
                CachePath = Path.Combine(Path.GetTempPath(), appName, Guid.NewGuid().ToString()),
                ConfigurationFile = Path.Combine(Path.GetDirectoryName(assemblyPath), Path.GetFileName(assemblyPath) + ".config")
            };

            return AppDomain.CreateDomain($"{appName}.dll", null, setup);
        }

        private static void CopyRequiredRuntimeDependencies(IEnumerable<Assembly> assemblies, string destination)
        {
            foreach (var assembly in assemblies)
            {
                var sourceAssemblyFile = assembly.Location;
                var destinationAssemblyFile = Path.Combine(destination, Path.GetFileName(sourceAssemblyFile));

                // file doesn't exist or is older
                if (!File.Exists(destinationAssemblyFile) || File.GetLastWriteTimeUtc(sourceAssemblyFile) > File.GetLastWriteTimeUtc(destinationAssemblyFile))
                    CopyWithoutLockingSourceFile(sourceAssemblyFile, destinationAssemblyFile);
            }
        }

        private static void CopyWithoutLockingSourceFile(string sourceFile, string destinationFile)
        {
            const int bufferSize = 10 * 1024;

            using (var inputFile = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize))
            using (var outputFile = new FileStream(destinationFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize))
            {
                var buffer = new byte[bufferSize];
                int bytes;

                while ((bytes = inputFile.Read(buffer, 0, buffer.Length)) > 0)
                    outputFile.Write(buffer, 0, bytes);
            }
        }

        public void Dispose()
        {
            if (appDomain != null)
            {
                try
                {
                    var cacheDirectory = appDomain.SetupInformation.CachePath;

                    AppDomain.Unload(appDomain);
                    appDomain = null;

                    if (Directory.Exists(cacheDirectory))
                        Directory.Delete(cacheDirectory, true);
                }
                catch
                {
                }
            }
        }
    }
#endif
}
