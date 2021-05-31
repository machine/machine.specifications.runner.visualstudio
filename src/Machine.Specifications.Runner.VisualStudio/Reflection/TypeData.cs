using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Machine.VSTestAdapter.Reflection
{
    public class TypeData
    {
        private readonly PEReader reader;

        private readonly MetadataReader metadata;

        private readonly object sync = new object();

        private ReadOnlyCollection<MethodData> methods;

        public TypeData(string assembly, string typeName, PEReader reader, MetadataReader metadata, TypeDefinition definition, TypeDefinitionHandle handle)
        {
            this.reader = reader;
            this.metadata = metadata;

            Assembly = assembly;
            TypeName = typeName;
            Definition = definition;
            Handle = handle;
        }

        public string Assembly { get; }

        public string TypeName { get; }

        public TypeDefinition Definition { get; }

        public TypeDefinitionHandle Handle { get; }

        public ReadOnlyCollection<MethodData> Methods
        {
            get
            {
                if (methods != null)
                {
                    return methods;
                }

                lock (sync)
                {
                    methods = GetMethods().AsReadOnly();
                }

                return methods;
            }
        }

        private List<MethodData> GetMethods()
        {
            var values = new List<MethodData>();

            foreach (var methodHandle in Definition.GetMethods())
            {
                var methodDefinition = metadata.GetMethodDefinition(methodHandle);
                var parameters = methodDefinition.GetParameters();

                var methodName = metadata.GetString(methodDefinition.Name);

                if (IsConstructor(methodDefinition, methodName) && methodName.EndsWith(".ctor", StringComparison.Ordinal) && parameters.Count == 0)
                {
                    values.Add(new MethodData(Assembly, TypeName, methodName, reader, metadata, methodHandle));
                }
            }

            return values;
        }

        private bool IsConstructor(MethodDefinition method, string name)
        {
            return method.Attributes.HasFlag(MethodAttributes.RTSpecialName) &&
                   method.Attributes.HasFlag(MethodAttributes.SpecialName) &&
                   (name == ".cctor" || name == ".ctor");
        }
    }
}
