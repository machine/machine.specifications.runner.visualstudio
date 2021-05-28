using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

namespace Machine.VSTestAdapter.Navigation
{
    public class PortableNavigationSymbolReader : INavigationSymbolReader
    {
        private readonly MetadataReader reader;

        public PortableNavigationSymbolReader(string assembly)
        {
            var symbols = Path.ChangeExtension(assembly, "pdb");

            reader = MetadataReaderProvider
                .FromPortablePdbStream(File.OpenRead(symbols))
                .GetMetadataReader();
        }

        public IEnumerable<NavigationSequencePoint> ReadSequencePoints(NavigationMethod method)
        {
            return reader
                .GetMethodDebugInformation(method.Handle)
                .GetSequencePoints()
                .Select(x =>
                {
                    var document = reader.GetDocument(x.Document);
                    var fileName = reader.GetString(document.Name);

                    return new NavigationSequencePoint(fileName, x.StartLine, x.EndLine, x.Offset, x.IsHidden);
                });
        }
    }
}
