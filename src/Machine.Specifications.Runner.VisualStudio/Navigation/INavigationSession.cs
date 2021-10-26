using System;

namespace Machine.VSTestAdapter.Navigation
{
    public interface INavigationSession : IDisposable
    {
        NavigationData GetNavigationData(string typeName, string fieldName);
    }
}
