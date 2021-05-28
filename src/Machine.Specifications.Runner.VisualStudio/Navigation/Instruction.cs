using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace Machine.VSTestAdapter.Navigation
{
    public class Instruction
    {
        public Instruction(ILOpCode opCode, OperandType operandType, int offset, Instruction previous)
        {
            OpCode = opCode;
            OperandType = operandType;
            Offset = offset;
            Previous = previous;
        }

        public ILOpCode OpCode { get; }

        public OperandType OperandType { get; }

        public int Offset { get; }

        public string Name { get; set; }

        public Instruction Previous { get; }
    }
}
