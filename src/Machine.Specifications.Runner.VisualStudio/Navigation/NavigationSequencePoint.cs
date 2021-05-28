namespace Machine.VSTestAdapter.Navigation
{
    public class NavigationSequencePoint
    {
        public NavigationSequencePoint(string fileName, int startLine, int endLine, int offset, bool isHidden)
        {
            FileName = fileName;
            StartLine = startLine;
            EndLine = endLine;
            Offset = offset;
            IsHidden = isHidden;
        }

        public string FileName { get; }

        public int StartLine { get; }

        public int EndLine { get; }

        public int Offset { get; }

        public bool IsHidden { get; }
    }
}
