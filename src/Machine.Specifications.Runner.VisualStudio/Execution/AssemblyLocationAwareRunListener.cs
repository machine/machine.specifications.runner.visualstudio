using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Machine.Specifications.Runner.VisualStudio.Execution
{
    public class AssemblyLocationAwareRunListener : ISpecificationRunListener
    {
        private readonly Assembly[] assemblies;

        public AssemblyLocationAwareRunListener(Assembly[] assemblies)
        {
            this.assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            var loadedAssembly = assemblies.FirstOrDefault(x => x.GetName().Name.Equals(assembly.Name, StringComparison.OrdinalIgnoreCase));

            Directory.SetCurrentDirectory(Path.GetDirectoryName(loadedAssembly.Location));
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
        }

        public void OnRunStart()
        {
        }

        public void OnRunEnd()
        {
        }

        public void OnContextStart(ContextInfo context)
        {
        }

        public void OnContextEnd(ContextInfo context)
        {
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
        }

        public void OnFatalError(ExceptionResult exception)
        {
        }
    }
}
