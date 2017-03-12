using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Diagnostics;
using Machine.VSTestAdapter.Discovery;

namespace Machine.VSTestAdapter.Helpers
{
    public static class SpecTestHelper
    {
        public static TestCase GetVSTestCaseFromMSpecTestCase(string source, MSpecTestCase mspecTestCase, bool disableFullTestNames, Uri testRunnerUri)
        {
            VisualStudioTestIdentifier vsTest = mspecTestCase.ToVisualStudioTestIdentifier();

            TestCase testCase = new TestCase(vsTest.FullyQualifiedName, testRunnerUri, source)
            {
                DisplayName = disableFullTestNames ? mspecTestCase.SpecificationDisplayName : $"{mspecTestCase.ContextDisplayName} it {mspecTestCase.SpecificationDisplayName}",
                CodeFilePath = mspecTestCase.CodeFilePath,
                LineNumber = mspecTestCase.LineNumber
            };

            Trait classTrait = new Trait(Strings.TRAIT_CLASS, mspecTestCase.ClassName);
            Trait subjectTrait = new Trait(Strings.TRAIT_SUBJECT, string.IsNullOrEmpty(mspecTestCase.Subject) ? Strings.TRAIT_SUBJECT_NOSUBJECT : mspecTestCase.Subject);

            testCase.Traits.Add(classTrait);
            testCase.Traits.Add(subjectTrait);

            if (mspecTestCase.Tags != null)
            {
                foreach (var tag in mspecTestCase.Tags)
                {
                    if (!string.IsNullOrEmpty(tag))
                    {
                        Trait tagTrait = new Trait(Strings.TRAIT_TAG, tag);
                        testCase.Traits.Add(tagTrait);
                    }
                }
            }

            Debug.WriteLine($"TestCase {testCase.FullyQualifiedName}");
            return testCase;
        }
    }

}