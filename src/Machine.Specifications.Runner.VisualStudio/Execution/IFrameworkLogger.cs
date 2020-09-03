using System;

namespace Machine.VSTestAdapter.Execution
{
    public interface IFrameworkLogger
    {
        void SendErrorMessage(string message, Exception exception);
    }
}
