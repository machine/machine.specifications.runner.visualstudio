using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Machine.Specifications.Runner.VisualStudio.Discovery.BuiltIn
{
    public class SourceCodeLocationFinder : IDisposable
    {
        private readonly Lazy<AssemblyDefinition> assemblyDefinition;

        public SourceCodeLocationFinder(string assemblyFilePath)
        {
            assemblyDefinition = new Lazy<AssemblyDefinition>(() => LoadAssembly(assemblyFilePath));
        }

        public SourceCodeLocationInfo GetFieldLocation(string fullTypeName, string fieldName)
        {
            var type = Assembly.MainModule.GetType(HandleNestedTypeName(fullTypeName));

            var field = type?.Fields.FirstOrDefault(f => f.Name == fieldName);

            if (field == null)
                return null;

            return GetFieldLocationCore(type, fieldName);
        }

        private AssemblyDefinition LoadAssembly(string assemblyFilePath)
        {
            return AssemblyDefinition.ReadAssembly(assemblyFilePath, new ReaderParameters
            {
                ReadSymbols = true,
            });
        }

        public void Dispose()
        {
            Assembly?.Dispose();
        }

        private AssemblyDefinition Assembly => assemblyDefinition.Value;

        private string HandleNestedTypeName(string type)
        {
            return type.Replace("+", "/");
        }

        /// <summary>
        /// Field assignments get converted to assignments in the .ctor, so if we find that - we get the line info from there.
        /// </summary>
        private SourceCodeLocationInfo GetFieldLocationCore(TypeDefinition type, string fieldFullName)
        {
            if (!type.HasMethods)
                return null;

            var constructorDefinition = type.Methods
                .SingleOrDefault(x => x.IsConstructor && !x.Parameters.Any() && x.Name.EndsWith(".ctor", StringComparison.Ordinal));

            if (!constructorDefinition.HasBody)
                return null;

            if (constructorDefinition.DebugInformation == null)
                return null;

            var instruction = constructorDefinition.Body.Instructions
                .SingleOrDefault(x => x.Operand != null &&
                            x.Operand.GetType().IsAssignableFrom(typeof(FieldDefinition)) &&
                            ((MemberReference)x.Operand).Name == fieldFullName);

            while (instruction != null)
            {
                var sequencePoint = constructorDefinition.DebugInformation?.GetSequencePoint(instruction);

                if (sequencePoint != null && !IsHidden(sequencePoint))
                {
                    return new SourceCodeLocationInfo()
                    {
                        CodeFilePath = sequencePoint.Document.Url,
                        LineNumber = sequencePoint.StartLine
                    };
                }

                instruction = instruction.Previous;
            }

            return null;
        }

        private bool IsHidden(SequencePoint sequencePoint)
        {
            return sequencePoint.IsHidden;
        }
    }
}
