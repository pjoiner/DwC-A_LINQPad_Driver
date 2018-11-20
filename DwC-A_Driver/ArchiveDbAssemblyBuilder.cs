using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using DwC_A.Meta;

namespace DwC_A_Driver
{
    class ArchiveDbAssemblyBuilder
    {

        public void GenerateArchiveDbAssembly(IFileMetaData coreFileMetaData,
            IEnumerable<IFileMetaData> extensionFileMetaData, 
            string assemblyName, string driverFolder)
        {
            var sources = GenerateSourceFiles(coreFileMetaData, extensionFileMetaData);
            sources.Add(GenerateArchiveDb(coreFileMetaData, extensionFileMetaData));
            CompileUnit(sources.ToArray(), assemblyName, driverFolder);
        }

        private IList<string> GenerateSourceFiles(IFileMetaData coreFileMetaData,
            IEnumerable<IFileMetaData> extensionFileMetaData)
        {
            IList<string> sources = new List<string>();

            var archiveCodeDom = new ArchiveFileCodeDom();
            var coreFileCs = archiveCodeDom.GenerateSource(coreFileMetaData.FileName, coreFileMetaData);
            Debug.WriteLine(coreFileCs);
            sources.Add(coreFileCs);

            foreach (var extension in extensionFileMetaData)
            {
                var extensionFileCs = archiveCodeDom.GenerateSource(extension.FileName, extension);
                Debug.WriteLine(extensionFileCs);
                sources.Add(extensionFileCs);
            }
            return sources;
        }

        private string GenerateArchiveDb(IFileMetaData coreFileMetaData,
            IEnumerable<IFileMetaData> extensionFileMetaData)
        {
            var archiveDbCodeDom = new ArchiveDbCodeDom();
            var archiveDbCs = archiveDbCodeDom.GenerateSource(coreFileMetaData, extensionFileMetaData);
            Debug.WriteLine(archiveDbCs);
            return archiveDbCs;
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
