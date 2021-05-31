using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Machine.VSTestAdapter.Reflection
{
    public interface ISymbolReader
    {
        IEnumerable<SequencePointData> ReadSequencePoints(MethodDefinitionHandle method);
    }
}
