using System.IO;

namespace Machine.Specifications.Runner.VisualStudio.Specs
{
    public class Helper
    {
        public static string GetTestDirectory()
        {
            return Path.GetDirectoryName(typeof(Helper).Assembly.Location);
        }
    }
}
