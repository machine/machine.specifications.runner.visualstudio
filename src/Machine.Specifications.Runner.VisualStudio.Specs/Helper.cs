using System;
using System.IO;
using System.Reflection;

namespace Machine.VSTestAdapter.Specs
{
    public class Helper
    {
        public static string GetTestDirectory()
        {
            return Path.GetDirectoryName(typeof(Helper).Assembly.Location);
        }
    }
}
