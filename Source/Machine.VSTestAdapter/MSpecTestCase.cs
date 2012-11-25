using System;

namespace Machine.VSTestAdapter
{
    public class MSpecTestCase : MarshalByRefObject
    {
        public string SubjectName { get; set; }

        public string ContextFullType { get; set; }

        public string ContextType { get; set; }

        public string SpecificationName { get; set; }

        public string CodeFilePath { get; set; }

        public int LineNumber { get; set; }

        public override object InitializeLifetimeService()
        {
            return (object)null;
        }
    }
}