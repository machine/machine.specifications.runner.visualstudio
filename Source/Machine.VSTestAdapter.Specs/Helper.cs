using System;
using System.IO;
using System.Reflection;

namespace Machine.VSTestAdapter.Specs
{
    public class Helper
    {
        public static string GetTestDebugDirectory()
        {
            Uri assemblyURI = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            return Path.Combine(Path.GetDirectoryName(assemblyURI.LocalPath), @"..\..\..\testdata\");
        }

        public static string GetTestSourceDirectory()
        {
            Uri assemblyURI = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            return Path.Combine(Path.GetDirectoryName(assemblyURI.LocalPath), @"..\..\..\testdata");
        }
    }
}