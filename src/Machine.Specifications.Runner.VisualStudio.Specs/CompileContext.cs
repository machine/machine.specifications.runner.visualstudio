using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Machine.Specifications;
using Microsoft.CodeAnalysis;
using Microsoft.CSharp;

namespace Machine.VSTestAdapter.Specs
{
    public class CompileContext : IDisposable
    {
        private readonly string _directory = Path.GetDirectoryName(typeof(CompileContext).Assembly.Location);

        public string Compile(string code)
        {
            var filename = Path.Combine(_directory, Guid.NewGuid() + ".dll");

#if NETCOREAPP
            var result = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create(Path.GetFileName(filename))
                .WithOptions(
                    new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location),
                    MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                    MetadataReference.CreateFromFile(typeof(Establish).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ShouldExtensionMethods).Assembly.Location))
                .AddSyntaxTrees(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(code))
                .Emit(filename, Path.ChangeExtension(filename, "pdb"));

            if (!result.Success)
                throw new InvalidOperationException();
#else
            var parameters = new CompilerParameters
            {
                OutputAssembly = filename,
                IncludeDebugInformation = true,
                TempFiles = new TempFileCollection("%temp%\\dir")
            };

            parameters.ReferencedAssemblies.AddRange(new []
            {
                "Machine.Specifications.dll",
                "Machine.Specifications.Should.dll"
            });

            var options = new Dictionary<string, string> {{"CompilerVersion", "v4.0"}};

            var provider = new CSharpCodeProvider(options);
            var results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.Count > 0)
                throw new InvalidOperationException();

            results.TempFiles.Delete();
#endif
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
