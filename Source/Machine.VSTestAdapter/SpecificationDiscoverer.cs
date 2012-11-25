using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Machine.VSTestAdapter
{
    public class SpecificationDiscoverer : ISpecificationDiscoverer
    {
        private const int PdbHiddenLine = 0xFEEFEE;

        public string AssemblyFilename { get; set; }

        public SpecificationDiscoverer()
        {
        }

        public IEnumerable<MSpecTestCase> EnumerateSpecs(string assemblyFilePath)
        {
            assemblyFilePath = Path.GetFullPath(assemblyFilePath);
            if (!File.Exists(assemblyFilePath))
            {
                throw new ArgumentException("Could not find file: " + assemblyFilePath);
            }

            this.AssemblyFilename = assemblyFilePath;

            List<MSpecTestCase> list = new List<MSpecTestCase>();
            foreach (TypeDefinition type in AssemblyDefinition.ReadAssembly(this.AssemblyFilename, new ReaderParameters()
            {
                ReadSymbols = true
            }).MainModule.Types)
            {
                foreach (FieldDefinition fieldDefinition in Enumerable.Where<FieldDefinition>((IEnumerable<FieldDefinition>)type.Fields, (Func<FieldDefinition, bool>)(x => x.FieldType.FullName == "Machine.Specifications.It" && !x.Name.Contains("__Cached"))))
                {
                    MSpecTestCase testCase = new MSpecTestCase()
                    {
                        ContextType = type.Name,
                        ContextFullType = type.FullName,
                        SpecificationName = fieldDefinition.Name
                    };
                    this.UpdateTestCaseWithLocation(type, testCase);
                    list.Add(testCase);
                }
            }
            return list.Select(x => x);
        }

        public bool SourceDirectoryContainsMSpec(string assemblyFileName)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(assemblyFileName), "Machine.Specifications.dll"));
        }

        public bool AssemblyContainsMSpecReference(string assemblyFileName)
        {
            AssemblyDefinition asmDef = AssemblyDefinition.ReadAssembly(assemblyFileName);
            foreach (AssemblyNameReference anrRef in asmDef.MainModule.AssemblyReferences)
            {
                if (anrRef.FullName.StartsWith("Machine.Specifications"))
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateTestCaseWithLocation(TypeDefinition type, MSpecTestCase testCase)
        {
            if (!type.HasMethods)
                return;
            string fieldFullName = testCase.SpecificationName.Replace(" ", "_");
            string constructorMethodFullName = string.Format("System.Void {0}::{1}", (object)testCase.ContextFullType, (object)".ctor()");
            MethodDefinition methodDefinition = Enumerable.SingleOrDefault<MethodDefinition>(Enumerable.Where<MethodDefinition>((IEnumerable<MethodDefinition>)type.Methods, (Func<MethodDefinition, bool>)(x => x.FullName == constructorMethodFullName)));
            if (methodDefinition.HasBody)
            {
                if (type.HasCustomAttributes)
                {
                    List<CustomAttribute> list = Enumerable.ToList<CustomAttribute>(Enumerable.Where<CustomAttribute>((IEnumerable<CustomAttribute>)type.CustomAttributes, (Func<CustomAttribute, bool>)(x => x.AttributeType.FullName == "Machine.Specifications.SubjectAttribute")));
                    if (list.Count > 0 && list[0].ConstructorArguments.Count > 0)
                    {
                        testCase.SubjectName = Enumerable.First<CustomAttributeArgument>((IEnumerable<CustomAttributeArgument>)list[0].ConstructorArguments).Value.ToString();
                    }
                }

                Instruction instruction = methodDefinition.Body.Instructions.Where(x => x.Operand != null &&
                                                              x.Operand.GetType().IsAssignableFrom(typeof(FieldDefinition)) &&
                                                              ((MemberReference)x.Operand).Name == fieldFullName).SingleOrDefault();

                while (instruction != null)
                {
                    if (instruction.SequencePoint != null && instruction.SequencePoint.StartLine != PdbHiddenLine)
                    {
                        testCase.CodeFilePath = instruction.SequencePoint.Document.Url;
                        testCase.LineNumber = instruction.SequencePoint.StartLine;
                        break;
                    }
                    instruction = instruction.Previous;
                }

                //for (Instruction instruction = Enumerable.SingleOrDefault<Instruction>(Enumerable.Where<Instruction>(
                //    Enumerable.Where<Instruction>((IEnumerable<Instruction>)methodDefinition.Body.Instructions,
                //    (Func<Instruction, bool>)(x => x.Operand != null && x.Operand.GetType().IsAssignableFrom(typeof(FieldDefinition)))),
                //    (Func<Instruction, bool>)(x => ((MemberReference)x.Operand).Name == fieldFullName)));
                //    instruction != null; instruction = instruction.Previous)
                //{
                //    if (instruction.SequencePoint != null && instruction.SequencePoint.StartLine != 16707566)
                //    {
                //        testCase.CodeFilePath = instruction.SequencePoint.Document.Url;
                //        testCase.LineNumber = instruction.SequencePoint.StartLine;
                //        break;
                //    }
                //}
            }
        }
    }
}