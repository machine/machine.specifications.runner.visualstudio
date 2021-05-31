using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Machine.VSTestAdapter.Reflection
{
    public class MethodData
    {
        private readonly PEReader reader;

        private readonly MetadataReader metadata;

        private readonly object sync = new object();

        private ReadOnlyCollection<InstructionData> instructions;

        private List<SequencePointData> sequencePoints;

        public MethodData(string assembly, string typeName, string name, PEReader reader, MetadataReader metadata, MethodDefinitionHandle handle)
        {
            this.reader = reader;
            this.metadata = metadata;

            Assembly = assembly;
            TypeName = typeName;
            Name = name;
            Handle = handle;
        }

        public string Assembly { get; }

        public string TypeName { get; }

        public string Name { get; }

        public MethodDefinition Definition { get; }

        public MethodDefinitionHandle Handle { get; }

        public ReadOnlyCollection<InstructionData> Instructions
        {
            get
            {
                if (instructions != null)
                {
                    return instructions;
                }

                lock (sync)
                {
                    instructions = GetInstructions().AsReadOnly();
                }

                return instructions;
            }
        }

        public SequencePointData GetSequencePoint(InstructionData instruction)
        {
            if (sequencePoints == null)
            {
                lock (sync)
                {
                    sequencePoints = GetSequencePoints().ToList();
                }
            }

            return sequencePoints.FirstOrDefault(x => x.Offset == instruction.Offset);
        }

        private List<InstructionData> GetInstructions()
        {
            var blob = reader
                .GetMethodBody(Definition.RelativeVirtualAddress)
                .GetILReader();

            var codeReader = new CodeReader();

            return codeReader.GetInstructions(metadata, ref blob).ToList();
        }

        private IEnumerable<SequencePointData> GetSequencePoints()
        {
            var symbolReader = new PortableSymbolReader(Assembly);

            return symbolReader.ReadSequencePoints(Handle);
        }
    }
}
