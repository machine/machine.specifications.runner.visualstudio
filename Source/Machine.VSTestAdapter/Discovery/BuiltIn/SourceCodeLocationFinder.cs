using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Machine.VSTestAdapter.Discovery.BuiltIn
{
    public class SourceCodeLocationFinder
    {
        private readonly Lazy<AssemblyDefinition> _assemblyDefinition;

        public SourceCodeLocationFinder(string assemblyFilePath)
        {
            _assemblyDefinition = new Lazy<AssemblyDefinition>(() => { return LoadAssembly(assemblyFilePath); });
        }

        private AssemblyDefinition LoadAssembly(string assemblyFilePath)
        {
            return AssemblyDefinition.ReadAssembly(assemblyFilePath, new ReaderParameters() {
                ReadSymbols = true,
            });
        }

        private AssemblyDefinition Assembly {
            get { return _assemblyDefinition.Value; }
        }

        public SourceCodeLocationInfo GetFieldLocation(string fullTypeName, string fieldName)
        {
            TypeDefinition type = Assembly.MainModule.GetType(HandleNestedTypeName(fullTypeName));
            if (type == null)
                return null;

            FieldDefinition field = type.Fields.FirstOrDefault(f => f.Name == fieldName);
            if (field == null)
                return null;

            return GetFieldLocationCore(type, fieldName);
        }

        private string HandleNestedTypeName(string type)
        {
            return type.Replace("+", "/");
        }

        /// <summary>
        /// Field assignments get converted to assignments in the .ctor, so if we find that - we get the line info from there.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldFullName"></param>
        /// <returns></returns>
        private SourceCodeLocationInfo GetFieldLocationCore(TypeDefinition type, string fieldFullName)
        {
            if (!type.HasMethods)
                return null;


            MethodDefinition constructorDefinition = type.Methods
                .SingleOrDefault(x => x.IsConstructor && !x.Parameters.Any() && x.Name.EndsWith(".ctor", StringComparison.Ordinal));

            if (!constructorDefinition.HasBody)
                return null;

            Instruction instruction = constructorDefinition.Body.Instructions
                .Where(x => x.Operand != null && 
                            x.Operand.GetType().IsAssignableFrom(typeof(FieldDefinition)) &&
                            ((MemberReference)x.Operand).Name == fieldFullName).SingleOrDefault();

            while (instruction != null)
            {
                const int PdbHiddenLine = 0xFEEFEE;

                if (instruction.SequencePoint != null && instruction.SequencePoint.StartLine != PdbHiddenLine)
                {
                    return new SourceCodeLocationInfo() {
                        CodeFilePath = instruction.SequencePoint.Document.Url,
                        LineNumber = instruction.SequencePoint.StartLine
                    };
                }

                instruction = instruction.Previous;
            }

            return null;
        }
    }

    public class SourceCodeLocationInfo
    {
        public string CodeFilePath { get; set; }
        public int LineNumber { get; set; }
    }
}
