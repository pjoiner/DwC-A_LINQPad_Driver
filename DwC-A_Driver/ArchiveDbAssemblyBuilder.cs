using DwC_A;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace DwC_A_Driver
{
    class ArchiveDbAssemblyBuilder
    {

        public void GenerateSchemaAssembly(string fileName, string assemblyName, string driverFolder)
        {
            var sources = GenerateExtensionFiles(fileName);
            sources.Add(GenerateMyArchiveDb(fileName));
            CompileUnit(sources.ToArray(), assemblyName, driverFolder);
        }

        private IList<string> GenerateExtensionFiles(string fileName)
        {
            using (var archive = new ArchiveReader(fileName))
            {
                IList<string> sources = new List<string>();

                var coreFileTemplate = new CoreFileTemplate(Path.GetFileNameWithoutExtension(archive.CoreFile.FileMetaData.FileName), archive.CoreFile);
                var coreFileCs = coreFileTemplate.TransformText();
                Debug.WriteLine(coreFileCs);
                sources.Add(coreFileCs);

                foreach (var fileReader in archive.Extensions)
                {
                    var extensionFileTemplate = new CoreFileTemplate(Path.GetFileNameWithoutExtension(fileReader.FileName), fileReader);
                    var extensionFileCs = extensionFileTemplate.TransformText();
                    Debug.WriteLine(extensionFileCs);
                    sources.Add(extensionFileCs);
                }
                return sources;
            }
        }

        private string GenerateMyArchiveDb(string fileName)
        {
            using (var archive = new ArchiveReader(fileName))
            {
                var myArchiveDbTemplate = new MyArchiveDbTemplate(archive);
                var myArchiveDbCs = myArchiveDbTemplate.TransformText();
                Debug.WriteLine(myArchiveDbCs);
                return myArchiveDbCs;
            }
        }

        private void CompileUnit(string[] sources, string assemblyName, string driverFolder)
        {

            CodeDomProvider csc = CodeDomProvider.CreateProvider("CSharp");
            var parameters = new CompilerParameters(new[]
                {
                    "mscorlib.dll",
                    "System.Core.dll",
                    "netstandard.dll",
                    "System.dll",
                    "System.configuration.dll",
                    "DwC-A_dotnet.dll"
                }, assemblyName, true)
            {
                GenerateExecutable = false,
                CoreAssemblyFileName = "netstandard.dll",
                CompilerOptions = $"/lib:\"{driverFolder}\""
            };
            CompilerResults results = csc.CompileAssemblyFromSource(parameters, sources);
            if(results.Errors.HasErrors)
            {
                throw new Exception(string.Join(";", results.Errors.Cast<CompilerError>().ToList()));
            }
        }

    }
}
