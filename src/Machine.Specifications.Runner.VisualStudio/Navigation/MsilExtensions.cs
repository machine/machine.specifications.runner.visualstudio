using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace Machine.VSTestAdapter.Navigation
{
    public static class MsilExtensions
    {
        private static readonly OperandType[] OperandTypes = Enumerable.Repeat((OperandType) 0xff, 0x11f).ToArray();

        private static readonly string[] OperandNames = new string[0x11f];

        static MsilExtensions()
        {
            foreach (var field in typeof(OpCodes).GetFields())
            {
                var opCode = (OpCode) field.GetValue(null);
                var index = (ushort) (((opCode.Value & 0x200) >> 1) | opCode.Value & 0xff);

                OperandTypes[index] = opCode.OperandType;
                OperandNames[index] = opCode.Name;
            }
        }

        public static ILOpCode ReadOpCode(this ref BlobReader blob)
        {
            var opCodeByte = blob.ReadByte();

            var value = opCodeByte == 0xfe
                ? 0xfe00 + blob.ReadByte()
                : opCodeByte;

            return (ILOpCode) value;
        }

        public static OperandType GetOperandType(this ILOpCode opCode)
        {
            var index = (ushort) ((((int) opCode & 0x200) >> 1) | ((int) opCode & 0xff));

            if (index >= OperandTypes.Length)
            {
                return (OperandType) 0xff;
            }

            return OperandTypes[index];
        }

        public static string GetDisplayName(this ILOpCode opCode)
        {
            var index = (ushort) ((((int) opCode & 0x200) >> 1) | ((int) opCode & 0xff));

            if (index >= OperandNames.Length)
            {
                return string.Empty;
            }

            return OperandNames[index];
        }
    }
}
