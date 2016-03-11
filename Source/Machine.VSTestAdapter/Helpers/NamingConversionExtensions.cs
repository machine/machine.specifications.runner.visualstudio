using System;
using System.Globalization;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;
using Machine.VSTestAdapter.Discovery;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Machine.VSTestAdapter.Helpers
{
    public static class NamingConversionExtensions
    {
        public static VisualStudioTestIdentifier ToVisualStudioTestIdentifier(this SpecificationInfo specification, ContextInfo context, bool displayFullTestName = true)
        {
            return new VisualStudioTestIdentifier(String.Format(CultureInfo.InvariantCulture, "{0}::{1}", context?.TypeName ?? specification.ContainingType, specification.FieldName)) {
                DisplayName = displayFullTestName ? $"{context.TypeName}: {specification.FieldName}" : specification.Name
            };
        }

        public static VisualStudioTestIdentifier ToVisualStudioTestIdentifier(this MSpecTestCase specification, bool displayFullTestName = true)
        {
            return new VisualStudioTestIdentifier(String.Format(CultureInfo.InvariantCulture, "{0}::{1}", specification.ContextFullType, specification.SpecificationName)) {
                DisplayName = displayFullTestName ? $"{specification.ContextFullType} {specification.SpecificationName}" : specification.SpecificationDisplayName
            };
        }
        public static VisualStudioTestIdentifier ToVisualStudioTestIdentifier(this TestCase testCase)
        {
            return new VisualStudioTestIdentifier(testCase.FullyQualifiedName) {
                DisplayName = testCase.DisplayName
            };
        }

        public static VisualStudioTestIdentifier ToVisualStudioTestIdentifier(this Specification specification, Context context, bool displayFullTestName = true)
        {
            return new VisualStudioTestIdentifier(String.Format(CultureInfo.InvariantCulture, "{0}::{1}", context.Type.FullName, specification.FieldInfo.Name)) {
                DisplayName = displayFullTestName ? $"{context.Type.FullName}: {specification.FieldInfo.Name}" : specification.Name
            };
        }
    }
}