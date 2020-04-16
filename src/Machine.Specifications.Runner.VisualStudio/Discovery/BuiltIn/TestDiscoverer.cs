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
#if !NETSTANDARD
        [System.Security.SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }
#endif
        private readonly PropertyInfo behaviorProperty = typeof(BehaviorSpecification).GetProperty("BehaviorFieldInfo");

        public IEnumerable<MSpecTestCase> DiscoverTests(string assemblyPath)
        {
            AssemblyExplorer assemblyExplorer = new AssemblyExplorer();

            Assembly assembly = AssemblyHelper.Load(assemblyPath);
            IEnumerable<Context> contexts = assemblyExplorer.FindContextsIn(assembly);

            using (SourceCodeLocationFinder locationFinder = new SourceCodeLocationFinder(assemblyPath))
            {
                return contexts.SelectMany(context => CreateTestCase(context, locationFinder)).ToList();
            }
        }

        private IEnumerable<MSpecTestCase> CreateTestCase(Context context, SourceCodeLocationFinder locationFinder)
        {
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

                if (spec is BehaviorSpecification behaviorSpec)
                    PopulateBehaviorField(testCase, behaviorSpec);

                if (context.Tags != null)
                    testCase.Tags = context.Tags.Select(tag => tag.Name).ToArray();

                if (context.Subject != null)
                    testCase.Subject = context.Subject.FullConcern;

                yield return testCase;
            }
        }

        private void PopulateBehaviorField(MSpecTestCase testCase, BehaviorSpecification specification)
        {
            if (behaviorProperty?.GetValue(specification) is FieldInfo field)
            {
                testCase.BehaviorFieldName = field.Name;
                testCase.BehaviorFieldType = field.FieldType.GenericTypeArguments.FirstOrDefault()?.FullName;
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
    }

}
