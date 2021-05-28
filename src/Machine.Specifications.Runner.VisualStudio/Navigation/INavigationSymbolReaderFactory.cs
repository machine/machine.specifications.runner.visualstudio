namespace Machine.VSTestAdapter.Navigation
{
    public interface INavigationSymbolReaderFactory
    {
        INavigationSymbolReader GetReader(string assembly);
    }
}
