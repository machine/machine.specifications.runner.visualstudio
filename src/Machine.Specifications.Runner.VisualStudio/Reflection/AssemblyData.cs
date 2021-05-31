using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Machine.VSTestAdapter.Reflection
{
    public class AssemblyData : IDisposable
    {
        private readonly PEReader reader;

        private readonly string assembly;

        private readonly MetadataReader metadata;

        private readonly object sync = new object();

        private ReadOnlyCollection<TypeData> types;

        private AssemblyData(PEReader reader, string assembly)
        {
            this.reader = reader;
            this.assembly = assembly;

            metadata = reader.GetMetadataReader();
        }

        public static AssemblyData Read(string assembly)
        {
            var reader = new PEReader(File.OpenRead(assembly));

            return new AssemblyData(reader, assembly);
        }

        public ReadOnlyCollection<TypeData> Types
        {
            get
            {
                if (types != null)
                {
                    return types;
                }

                lock (sync)
                {
                    types = ReadTypes().AsReadOnly();
                }

                return types;
            }
        }

        public void Dispose()
        {
            reader.Dispose();
        }

        private List<TypeData> ReadTypes()
        {
            var values = new List<TypeData>();

            foreach (var typeHandle in metadata.TypeDefinitions)
            {
                ReadType(values, typeHandle);
            }

            return values;
        }

        private void ReadType(List<TypeData> values, TypeDefinitionHandle typeHandle, string namespaceName = null)
        {
            var typeDefinition = metadata.GetTypeDefinition(typeHandle);

            var typeNamespace = string.IsNullOrEmpty(namespaceName)
                ? metadata.GetString(typeDefinition.Namespace)
                : namespaceName;

            var typeName = string.IsNullOrEmpty(namespaceName)
                ? $"{typeNamespace}.{metadata.GetString(typeDefinition.Name)}"
                : $"{typeNamespace}+{metadata.GetString(typeDefinition.Name)}";

            values.Add(new TypeData(assembly, typeName, reader, metadata, typeDefinition, typeHandle));

            foreach (var nestedTypeHandle in typeDefinition.GetNestedTypes())
            {
                ReadType(values, nestedTypeHandle, typeName);
            }
        }
    }
}
