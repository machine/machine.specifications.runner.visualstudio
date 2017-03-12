using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.VSTestAdapter.Helpers;

namespace Machine.VSTestAdapter.Discovery.BuiltIn
{

    public class TestDiscoverer
#if !NETSTANDARD
                                : MarshalByRefObject
#endif
    {

        public IEnumerable<MSpecTestCase> DiscoverTests(string assemblyPath)
        {
            AssemblyExplorer assemblyExplorer = new AssemblyExplorer();

            Assembly assembly = AssemblyHelper.Load(assemblyPath);
            IEnumerable<Context> contexts = assemblyExplorer.FindContextsIn(assembly);

            return contexts.SelectMany(context => CreateTestCase(context, assemblyPath)).ToList();
        }

        private IEnumerable<MSpecTestCase> CreateTestCase(Context context, string assemblyPath)
        {

            SourceCodeLocationFinder locationFinder = new SourceCodeLocationFinder(assemblyPath);


            foreach (Specification spec in context.Specifications.ToList())
            {
                MSpecTestCase testCase = new MSpecTestCase();

                testCase.ClassName = context.Type.Name;
                testCase.ContextFullType = context.Type.FullName;
                testCase.ContextDisplayName = GetContextDisplayName(context.Type);

                testCase.SpecificationName = spec.FieldInfo.Name;
                testCase.SpecificationDisplayName = spec.Name;

                string fieldDeclaringType;
                if (spec.FieldInfo.DeclaringType.GetTypeInfo().IsGenericType && !spec.FieldInfo.DeclaringType.GetTypeInfo().IsGenericTypeDefinition)
                    fieldDeclaringType = spec.FieldInfo.DeclaringType.GetGenericTypeDefinition().FullName;
                else
                    fieldDeclaringType = spec.FieldInfo.DeclaringType.FullName;
                    
                SourceCodeLocationInfo locationInfo = locationFinder.GetFieldLocation(fieldDeclaringType, spec.FieldInfo.Name);
                if (locationInfo != null)
                {
                    testCase.CodeFilePath = locationInfo.CodeFilePath;
                    testCase.LineNumber = locationInfo.LineNumber;
                }


                if (context.Tags != null)
                    testCase.Tags = context.Tags.Select(tag => tag.Name).ToArray();

                if (context.Subject != null)
                    testCase.Subject = context.Subject.FullConcern;

                yield return testCase;
            }
        }

        private string GetContextDisplayName(Type contextType)
        {
            var displayName = contextType.Name.Replace("_", " ");

            if (contextType.IsNested)
            {
                return GetContextDisplayName(contextType.DeclaringType) + " " + displayName;
            }

            return displayName;
        }

#if !NETSTANDARD
        public override object InitializeLifetimeService()
        {
            return null;
        }
#endif
    }

}
