using System.Collections.Generic;

namespace Machine.VSTestAdapter.Navigation
{
    public interface INavigationSymbolReader
    {
        IEnumerable<NavigationSequencePoint> ReadSequencePoints(NavigationMethod method);
    }
}
