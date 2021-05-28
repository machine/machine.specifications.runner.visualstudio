using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace Machine.VSTestAdapter.Navigation
{
    public class NavigationReader : IDisposable
    {
        private static readonly OperandType[] OperandTypes;

        private static readonly string[] OperandNames = new string[0x11f];

        private readonly INavigationSymbolReaderFactory symbolReaderFactory;

        private readonly string assemblyPath;

        private readonly PEReader peReader;

        private readonly List<NavigationMethod> methods = new List<NavigationMethod>();

        private INavigationSymbolReader symbolReader;

        static NavigationReader()
        {
            OperandTypes = Enumerable.Repeat((OperandType) 0xff, 0x11f).ToArray();

            foreach (var field in typeof(OpCodes).GetFields())
            {
                var opCode = (OpCode) field.GetValue(null);
                var index = (ushort) (((opCode.Value & 0x200) >> 1) | opCode.Value & 0xff);

                OperandTypes[index] = opCode.OperandType;
                OperandNames[index] = opCode.Name;
            }
        }

        public NavigationReader(INavigationSymbolReaderFactory symbolReaderFactory, string assemblyPath)
        {
            this.symbolReaderFactory = symbolReaderFactory;
            this.assemblyPath = assemblyPath;

            peReader = GetReader();
        }

        public void Dispose()
        {
            peReader.Dispose();
        }

        public NavigationData GetNavigationData(string typeName, string fieldName)
        {
            var method = methods.FirstOrDefault(x => x.Type == typeName && x.Method == ".ctor");

            if (method == null)
            {
                return NavigationData.Unknown;
            }

            var instruction = method.Instructions
                .Where(x => x.OperandType == OperandType.InlineField)
                .FirstOrDefault(x => !string.IsNullOrEmpty(x.Name) && x.Name == fieldName);

            while (instruction != null)
            {
                var sequencePoint = method.GetSequencePoint(instruction);

                if (sequencePoint != null && !sequencePoint.IsHidden)
                {
                    return new NavigationData(sequencePoint.FileName, sequencePoint.StartLine);
                }

                instruction = instruction.Previous;
            }

            return null;
        }

        private PEReader GetReader()
        {
            var reader = new PEReader(File.OpenRead(assemblyPath));

            var metadata = reader.GetMetadataReader();

            foreach (var typeHandle in metadata.TypeDefinitions)
            {
                ReadType(reader, metadata, typeHandle);
            }

            return reader;
        }

        private void ReadType(PEReader reader, MetadataReader metadata, TypeDefinitionHandle typeHandle, string namespaceName = null)
        {
            var typeDefinition = metadata.GetTypeDefinition(typeHandle);

            var typeNamespace = string.IsNullOrEmpty(namespaceName)
                ? metadata.GetString(typeDefinition.Namespace)
                : namespaceName;

            var typeName = string.IsNullOrEmpty(namespaceName)
                ? $"{typeNamespace}.{metadata.GetString(typeDefinition.Name)}"
                : $"{typeNamespace}+{metadata.GetString(typeDefinition.Name)}";

            if (typeName.StartsWith("SampleSpecs.Parent"))
            {
                var i = 1;
            }

            foreach (var nestedTypeHandle in typeDefinition.GetNestedTypes())
            {
                ReadType(reader, metadata, nestedTypeHandle, typeName);
            }

            foreach (var methodHandle in typeDefinition.GetMethods())
            {
                var methodDefinition = metadata.GetMethodDefinition(methodHandle);
                var parameters = methodDefinition.GetParameters();

                var methodName = metadata.GetString(methodDefinition.Name);

                if (IsConstructor(methodDefinition, methodName) && methodName.EndsWith(".ctor", StringComparison.Ordinal) && parameters.Count == 0)
                {
                    var method = new NavigationMethod(typeName, methodName, methodHandle);

                    var blob = reader
                        .GetMethodBody(methodDefinition.RelativeVirtualAddress)
                        .GetILReader();

                    GetSequencePoints(method);
                    GetInstructions(metadata, ref blob, method);

                    methods.Add(method);
                }
            }
        }

        private bool IsConstructor(MethodDefinition method, string name)
        {
            return method.Attributes.HasFlag(MethodAttributes.RTSpecialName) &&
                   method.Attributes.HasFlag(MethodAttributes.SpecialName) &&
                   (name == ".cctor" || name == ".ctor");
        }

        private void GetSequencePoints(NavigationMethod method)
        {
            if (symbolReader == null)
            {
                symbolReader = symbolReaderFactory.GetReader(assemblyPath);
            }

            var points = symbolReader
                .ReadSequencePoints(method)
                .ToArray();

            method.AddSequencePoints(points);
        }

        private void GetInstructions(MetadataReader reader, ref BlobReader blob, NavigationMethod method)
        {
            Instruction previous = null;

            while (blob.RemainingBytes > 0)
            {
                var offset = blob.Offset;

                var opCode = DecodeOpCode(ref blob);
                var operandType = GetOperandType(opCode);

                var instruction = new Instruction(opCode, operandType, offset, previous);

                if (operandType != OperandType.InlineNone)
                {
                    instruction.Name = ReadOperand(reader, ref blob, operandType);
                }

                previous = instruction;

                method.Instructions.Add(instruction);
            }
        }

        private string ReadOperand(MetadataReader reader, ref BlobReader blob, OperandType operandType)
        {
            var name = string.Empty;

            switch (operandType)
            {
                case OperandType.InlineI8:
                case OperandType.InlineR:
                    blob.Offset += 8;
                    break;

                case OperandType.InlineBrTarget:
                case OperandType.InlineI:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                    blob.Offset += 4;
                    break;

                case OperandType.InlineField:
                case OperandType.InlineMethod:
                    var handle = MetadataTokens.EntityHandle(blob.ReadInt32());

                    return LookupToken(reader, handle);

                case OperandType.InlineSwitch:
                    var length = blob.ReadInt32();
                    blob.Offset += length * 4;
                    break;

                case OperandType.InlineVar:
                    blob.Offset += 2;
                    break;

                case OperandType.ShortInlineVar:
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                    blob.Offset++;
                    break;
            }

            return name;
        }

        private string LookupToken(MetadataReader reader, EntityHandle handle)
        {
            if (handle.Kind == HandleKind.FieldDefinition)
            {
                var field = reader.GetFieldDefinition((FieldDefinitionHandle) handle);

                return reader.GetString(field.Name);
            }

            if (handle.Kind == HandleKind.MethodDefinition)
            {
                var method = reader.GetMethodDefinition((MethodDefinitionHandle) handle);

                return reader.GetString(method.Name);
            }

            return string.Empty;
        }

        private ILOpCode DecodeOpCode(ref BlobReader blob)
        {
            var opCodeByte = blob.ReadByte();

            var value = opCodeByte == 0xfe
                ? 0xfe00 + blob.ReadByte()
                : opCodeByte;

            return (ILOpCode) value;
        }

        private OperandType GetOperandType(ILOpCode opCode)
        {
            var index = (ushort) ((((int) opCode & 0x200) >> 1) | ((int) opCode & 0xff));

            if (index >= OperandTypes.Length)
            {
                return (OperandType) 0xff;
            }

            return OperandTypes[index];
        }

        private string GetDisplayName(ILOpCode opCode)
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
