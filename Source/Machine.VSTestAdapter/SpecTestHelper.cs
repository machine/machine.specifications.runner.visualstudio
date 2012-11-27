using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Diagnostics;

namespace Machine.VSTestAdapter
{
    public static class SpecTestHelper
    {
        public static TestCase GetVSTestCaseFromMSpecTestCase(string source, MSpecTestCase mspecTestCase, Uri uri, Func<string, string, dynamic> traitCreator)
        {
            string specificationName = mspecTestCase.SpecificationName;
            string fullyQualifiedName = string.Format("{0}::{1}", (object)mspecTestCase.ContextFullType, (object)specificationName);
            TestCase testCase = new TestCase(fullyQualifiedName, uri, source)
            {
                DisplayName = mspecTestCase.SpecificationName.Replace("_", " "),
                CodeFilePath = mspecTestCase.CodeFilePath,
                LineNumber = mspecTestCase.LineNumber
            };

            if (MSpecTestAdapter.UseTraits)
            {
                dynamic dynTestCase = testCase;
                dynamic classTrait = traitCreator(Strings.TRAIT_CLASS, mspecTestCase.ContextType);
                dynamic subjectTrait = traitCreator(Strings.TRAIT_SUBJECT, string.IsNullOrEmpty(mspecTestCase.SubjectName) ? Strings.TRAIT_SUBJECT_NOSUBJECT : mspecTestCase.SubjectName);

                dynTestCase.Traits.Add(classTrait);
                dynTestCase.Traits.Add(subjectTrait);
            }

            Debug.WriteLine(string.Format("TestCase {0}", (object)testCase.FullyQualifiedName));
            return testCase;
        }
    }
}