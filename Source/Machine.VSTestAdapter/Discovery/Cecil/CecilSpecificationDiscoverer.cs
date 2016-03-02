using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Machine.VSTestAdapter.Discovery.Cecil
{
    public class CecilSpecificationDiscoverer : ISpecificationDiscoverer
    {
        private const int PdbHiddenLine = 0xFEEFEE;

        public string AssemblyFilename { get; set; }

        public IAssemblyResolver AssemblyResolver { get; set; }

        public ReaderParameters ReaderParameters { get; set; }

        public CecilSpecificationDiscoverer()
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

            // make sure that cecil looks in the assembly path for mspec (+ related assemblies) first
            this.AssemblyResolver = new ScopedAssemblyResolver(Path.GetDirectoryName(assemblyFilePath));
            this.ReaderParameters = new ReaderParameters()
            {
                ReadSymbols = true,
                AssemblyResolver = AssemblyResolver
            };

            List<MSpecTestCase> list = new List<MSpecTestCase>();

            List<IDelegateFieldScanner> fieldScanners = new List<IDelegateFieldScanner>();
            fieldScanners.Add(new ItDelegateFieldScanner());
            fieldScanners.Add(new CustomDelegateFieldScanner());

            // statically inspect the types in the assembly using mono.cecil
            var assembly = AssemblyDefinition.ReadAssembly(this.AssemblyFilename, this.ReaderParameters);
            foreach (TypeDefinition type in GetNestedTypes(assembly.MainModule.Types))
            {
                // if a type is an It delegate generate some test case info for it
                foreach (FieldDefinition fieldDefinition in type.Fields.Where(x => !x.Name.Contains("__Cached")))
                {
                    foreach (IDelegateFieldScanner scanner in fieldScanners)
                    {
                        if (scanner.ProcessFieldDefinition(fieldDefinition))
                        {
                            string typeName = NormalizeCecilTypeName(type.Name);
                            string typeFullName = NormalizeCecilTypeName(type.FullName);

                            MSpecTestCase testCase = new MSpecTestCase()
                            {
                                ContextType = typeName,
                                ContextFullType = typeFullName,
                                SpecificationName = fieldDefinition.Name
                            };

                            // get the source code location for the It delegate from the PDB file using mono.cecil.pdb
                            this.UpdateTestCaseWithLocation(type, testCase);
                            list.Add(testCase);
                            break;
                        }
                    }
                }
            }
            return list.Select(x => x);
        }

        private IEnumerable<TypeDefinition> GetNestedTypes(Collection<TypeDefinition> types)
        {
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];

                if (!type.FullName.EndsWith("/<>c"))
                    yield return type;

                if (!type.HasNestedTypes)
                    continue;

                foreach (var nested in GetNestedTypes(type.NestedTypes))
                    yield return nested;
            }
        }

        private string NormalizeCecilTypeName(string cecilTypeName)
        {
            return cecilTypeName.Replace('/', '+');
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
            {
                return;
            }

            string fieldFullName = testCase.SpecificationName.Replace(" ", "_");
            MethodDefinition methodDefinition = type.Methods.Where(x => x.IsConstructor && x.Parameters.Count == 0 && x.Name.EndsWith(".ctor")).SingleOrDefault();
            if (methodDefinition.HasBody)
            {
                // check if there is a subject attribute
                if (type.HasCustomAttributes)
                {
                    List<CustomAttribute> list = type.CustomAttributes.Where(x => x.AttributeType.FullName == "Machine.Specifications.SubjectAttribute").ToList();
                    if (list.Count > 0 && list[0].ConstructorArguments.Count > 0)
                    {
                        testCase.SubjectName = Enumerable.First<CustomAttributeArgument>((IEnumerable<CustomAttributeArgument>)list[0].ConstructorArguments).Value.ToString();
                    }

                    List<CustomAttribute> tagsList = type.CustomAttributes.Where(x => x.AttributeType.FullName == "Machine.Specifications.TagsAttribute").ToList();
                    if (tagsList.Count > 0 && tagsList[0].ConstructorArguments.Count > 0)
                    {
                        List<string> tags = new List<string>();
                        tags.Add(tagsList[0].ConstructorArguments[0].Value.ToString());
                        if (tagsList[0].ConstructorArguments.Count == 2)
                        {
                            foreach (CustomAttributeArgument additionalTag in ((IEnumerable<CustomAttributeArgument>)tagsList[0].ConstructorArguments[1].Value))
                            {
                                tags.Add(additionalTag.Value.ToString());
                            }
                        }
                        testCase.Tags = tags.ToArray();
                    }
                }

                // now find the source code location
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
            }
        }
    }
}