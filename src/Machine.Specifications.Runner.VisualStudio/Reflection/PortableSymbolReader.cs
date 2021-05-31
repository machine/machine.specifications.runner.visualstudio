using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

namespace Machine.VSTestAdapter.Reflection
{
    public class PortableSymbolReader : ISymbolReader
    {
        private readonly MetadataReader reader;

        public PortableSymbolReader(string assembly)
        {
            var symbols = Path.ChangeExtension(assembly, "pdb");

            reader = MetadataReaderProvider
                .FromPortablePdbStream(File.OpenRead(symbols))
                .GetMetadataReader();
        }

        public IEnumerable<SequencePointData> ReadSequencePoints(MethodDefinitionHandle method)
        {
            return reader
                .GetMethodDebugInformation(method)
                .GetSequencePoints()
                .Select(x =>
                {
                    var document = reader.GetDocument(x.Document);
                    var fileName = reader.GetString(document.Name);

                    return new SequencePointData(fileName, x.StartLine, x.EndLine, x.Offset, x.IsHidden);
                });
        }
    }
}
