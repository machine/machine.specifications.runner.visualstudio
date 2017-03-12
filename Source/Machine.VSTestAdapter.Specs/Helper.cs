using System;
using System.IO;
using System.Reflection;

namespace Machine.VSTestAdapter.Specs
{
    public class Helper
    {
        public static string GetTestDirectory()
        {
            string testsDirectory =
#if NETSTANDARD
                @"..\..\..\..\testdata\netcoreapp1.1";

#else
                @"..\..\..\..\testdata\net46";
#endif

            // appveyor hack which adds an "Any CPU" directory
            if (!Directory.Exists(testsDirectory)) {
                testsDirectory =

#if NETSTANDARD
                @"..\..\..\..\..\testdata\netcoreapp1.1";

#else
                @"..\..\..\..\..\testdata\net46";
#endif
            }

            return Path.GetFullPath(testsDirectory);
        }
    }
}