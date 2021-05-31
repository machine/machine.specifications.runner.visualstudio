using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace Machine.VSTestAdapter.Reflection
{
    public class InstructionData
    {
        public InstructionData(ILOpCode opCode, OperandType operandType, int offset, InstructionData previous, string name = null)
        {
            OpCode = opCode;
            OperandType = operandType;
            Offset = offset;
            Previous = previous;
            Name = name;
        }

        public ILOpCode OpCode { get; }

        public OperandType OperandType { get; }

        public string Name { get; }

        public int Offset { get; }

        public InstructionData Previous { get; }
    }
}
