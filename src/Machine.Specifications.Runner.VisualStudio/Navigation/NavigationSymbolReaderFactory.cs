using System;
using System.IO;

namespace Machine.VSTestAdapter.Navigation
{
    public class NavigationSymbolReaderFactory : INavigationSymbolReaderFactory
    {
        private const uint PortablePdbSignature = 0x424a5342;

        public INavigationSymbolReader GetReader(string assembly)
        {
            var symbols = Path.ChangeExtension(assembly, "pdb");

            if (!File.Exists(symbols))
            {
                throw new FileNotFoundException($"Symbols file not found: {symbols}");
            }

            if (!IsPortablePdb(symbols))
            {
                throw new FormatException("Only portable PDB files can be used");
            }

            return new PortableNavigationSymbolReader(assembly);
        }

        private bool IsPortablePdb(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return IsPortablePdb(stream);
            }
        }

        private bool IsPortablePdb(Stream stream)
        {
            if (stream.Length < 4)
            {
                return false;
            }

            var reader = new BinaryReader(stream);

            return reader.ReadUInt32() == PortablePdbSignature;
        }
    }
}
