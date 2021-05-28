using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;

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

        public override string ToString()
        {
            var value = new StringBuilder();

            AppendLabel(value, this);

            value.Append(": ");
            value.Append(OpCode);

            if (!string.IsNullOrEmpty(Name))
            {
                value.Append(" ");

                if (OperandType == OperandType.InlineString)
                {
                    value.Append("\"");
                    value.Append(Name);
                    value.Append("\"");
                }
                else
                {
                    value.Append(Name);
                }
            }

            return value.ToString();
        }

        private void AppendLabel(StringBuilder value, Instruction instruction)
        {
            value.Append("IL_");
            value.Append(instruction.Offset.ToString("x4"));
        }
    }
}
