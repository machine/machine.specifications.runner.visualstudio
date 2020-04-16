using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.VSTestAdapter.Execution
{
    public interface IFrameworkLogger
    {
        void SendMessage(TestMessageLevel level, string message);
    }
}
