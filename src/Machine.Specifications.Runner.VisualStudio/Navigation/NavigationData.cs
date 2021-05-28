namespace Machine.VSTestAdapter.Navigation
{
    public class NavigationData
    {
        public static NavigationData Unknown = new NavigationData(null, int.MinValue);

        public NavigationData(string codeFile, int lineNumber)
        {
            CodeFile = codeFile;
            LineNumber = lineNumber;
        }

        public string CodeFile { get; }

        public int LineNumber { get; }
    }
}
