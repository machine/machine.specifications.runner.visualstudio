using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Diagnostics;

namespace Machine.VSTestAdapter
{
    public static class SpecTestHelper
    {
        public static TestCase GetVSTestCaseFromMSpecTestCase(string source, MSpecTestCase mspecTestCase, Uri uri)
        {
            string specificationName = mspecTestCase.SpecificationName;
            string fullyQualifiedName = string.Format("{0}::{1}", (object)mspecTestCase.ContextFullType, (object)specificationName);
            TestCase testCase = new TestCase(fullyQualifiedName, uri, source)
            {
                DisplayName = mspecTestCase.SpecificationName.Replace("_", " "),
                CodeFilePath = mspecTestCase.CodeFilePath,
                LineNumber = mspecTestCase.LineNumber
            };
            Debug.WriteLine(string.Format("TestCase {0}", (object)testCase.FullyQualifiedName));
            return testCase;
        }
    }
}