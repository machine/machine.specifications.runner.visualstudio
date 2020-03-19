using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Model;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.VSTestAdapter.Execution
{
    public class SpecificationFilterProvider : ISpecificationFilterProvider
    {
        static readonly TestProperty TagProperty = TestProperty.Register(nameof(Tag), nameof(Tag), typeof(string), typeof(TestCase));
        static readonly TestProperty SubjectProperty = TestProperty.Register(nameof(Subject), nameof(Subject), typeof(string), typeof(TestCase));

        readonly Dictionary<string, TestProperty> testCaseProperties = new Dictionary<string, TestProperty>(StringComparer.OrdinalIgnoreCase)
        {
            [TestCaseProperties.FullyQualifiedName.Id] = TestCaseProperties.FullyQualifiedName,
            [TestCaseProperties.DisplayName.Id] = TestCaseProperties.DisplayName
        };

        readonly Dictionary<string, TestProperty> traitProperties = new Dictionary<string, TestProperty>(StringComparer.OrdinalIgnoreCase)
        {
            [TagProperty.Id] = TagProperty,
            [SubjectProperty.Id] = SubjectProperty
        };

        readonly string[] supportedProperties;

        public SpecificationFilterProvider()
        {
            supportedProperties = testCaseProperties.Keys
                .Concat(traitProperties.Keys)
                .ToArray();
        }


        public IEnumerable<TestCase> FilteredTests(IEnumerable<TestCase> testCases, IRunContext runContext, IFrameworkHandle handle)
        {
            var filterExpression = runContext.GetTestCaseFilter(supportedProperties, propertyName =>
            {
                if (testCaseProperties.TryGetValue(propertyName, out var testProperty))
                {
                    return testProperty;
                }
                if (traitProperties.TryGetValue(propertyName, out var traitProperty))
                {
                    return traitProperty;
                }
                return null;
            });

            var filteredTests = testCases
                .Where(testCase => filterExpression
                    .MatchTestCase(testCase, propertyName =>
                    {
                        var value = GetPropertyValue(propertyName, testCase);
                        handle.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter -Filter property '{propertyName}' for test '{testCase.Id}' returned '{value}'");
                        return value;
                    }));

            return filteredTests;
        }

        object GetPropertyValue(string propertyName, TestCase testCase)
        {
            if (testCaseProperties.TryGetValue(propertyName, out var testProperty))
            {
                if (testCase.Properties.Contains(testProperty))
                {
                    return testCase.GetPropertyValue(testProperty);
                }
            }

            if (traitProperties.TryGetValue(propertyName, out var traitProperty))
            {
                var val = TraitContains(testCase, traitProperty.Id);

                if (val.Length == 1)
                {
                    return val[0];
                }

                if (val.Length > 1)
                {
                    return val;
                }
            }

            return null;
        }

        static string[] TraitContains(TestObject testCase, string traitName)
        {
            return testCase.Traits
                .Where(x => x.Name == traitName)
                .Select(x => x.Value)
                .ToArray();
        }
    }
}
