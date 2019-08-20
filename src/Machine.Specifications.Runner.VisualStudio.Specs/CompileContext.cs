using System;
using System.IO;
using System.Linq;
using Machine.Specifications;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Machine.VSTestAdapter.Specs
{
    public class CompileContext : IDisposable
    {
        private readonly string _directory = Path.GetDirectoryName(typeof(CompileContext).Assembly.Location);

        public string Compile(string code)
        {
            var filename = Path.Combine(_directory, Guid.NewGuid() + ".dll");

            var result = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(filename))
                .WithOptions(
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
#if NETCOREAPP
                    GetReference("System.Runtime"),
#endif
                    MetadataReference.CreateFromFile(typeof(Establish).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ShouldExtensionMethods).Assembly.Location))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code))
                .Emit(filename, Path.ChangeExtension(filename, "pdb"));

            if (!result.Success)
                throw new InvalidOperationException();
     
            return filename;
        }

        public void Dispose()
        {
            var files = Directory.GetFiles(_directory, "*.dll")
                .Concat(Directory.GetFiles(_directory, "*.pdb"))
                .Where(x => Guid.TryParse(Path.GetFileNameWithoutExtension(x), out _));

            foreach (var file in files)
            {
                SafeDelete(file);
            }
        }

#if NETCOREAPP
        private MetadataReference GetReference(string name)
        {
            var references = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
                .ToString()
                .Split(Path.PathSeparator);

            return references
                .Where(x => Path.GetFileNameWithoutExtension(x) == name)
                .Select(x => MetadataReference.CreateFromFile(x))
                .First();
        }
#endif

        private void SafeDelete(string filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch
            {
            }
        }
    }
}
