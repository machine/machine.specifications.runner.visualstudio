using System;
using Machine.Specifications.Runner.VisualStudio.Discovery;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Machine.Specifications.Runner.VisualStudio.Helpers
{
    public static class SpecTestHelper
    {
        public static TestCase GetVSTestCaseFromMSpecTestCase(string source, MSpecTestCase mspecTestCase, bool disableFullTestNames, Uri testRunnerUri)
        {
            var vsTest = mspecTestCase.ToVisualStudioTestIdentifier();

            var testCase = new TestCase(vsTest.FullyQualifiedName, testRunnerUri, source)
            {
                DisplayName = disableFullTestNames ? mspecTestCase.SpecificationDisplayName : $"{mspecTestCase.ContextDisplayName} it {mspecTestCase.SpecificationDisplayName}",
                CodeFilePath = mspecTestCase.CodeFilePath,
                LineNumber = mspecTestCase.LineNumber
            };

            var classTrait = new Trait("ClassName", mspecTestCase.ClassName);
            var subjectTrait = new Trait("Subject", string.IsNullOrEmpty(mspecTestCase.Subject) ? "No Subject" : mspecTestCase.Subject);

            testCase.Traits.Add(classTrait);
            testCase.Traits.Add(subjectTrait);

            if (mspecTestCase.Tags != null)
            {
                foreach (var tag in mspecTestCase.Tags)
                {
                    if (!string.IsNullOrEmpty(tag))
                        testCase.Traits.Add(new Trait("Tag", tag));
                }
            }

            if (!string.IsNullOrEmpty(mspecTestCase.BehaviorFieldName))
                testCase.Traits.Add(new Trait("BehaviorField", mspecTestCase.BehaviorFieldName));

            if (!string.IsNullOrEmpty(mspecTestCase.BehaviorFieldType))
                testCase.Traits.Add(new Trait("BehaviorType", mspecTestCase.BehaviorFieldType));

            return testCase;
        }
    }
}
