using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using DwC_A.Meta;
using System.IO;
using DwC_A;
using LINQPad.Extensibility.DataContext;
using DwC_A.Config;

namespace DwC_A_Driver
{
    class ArchiveDbAssemblyBuilder
    {
        private readonly bool capitalize;
        private readonly FileReaderConfiguration fileReaderConfig;
        private readonly RowFactoryConfiguration rowFactoryConfig;

        public ArchiveDbAssemblyBuilder(bool capitalize = false, 
            FileReaderConfiguration fileReaderConfig = null,
            RowFactoryConfiguration rowFactoryConfig = null)
        {
            this.capitalize = capitalize;
            this.fileReaderConfig = fileReaderConfig;
            this.rowFactoryConfig = rowFactoryConfig;
        }

        public void GenerateArchiveDbAssembly(IFileMetaData coreFileMetaData,
            IEnumerable<IFileMetaData> extensionFileMetaData, 
            string assemblyName, string driverFolder)
        {
            var sources = GenerateSourceFiles(coreFileMetaData, extensionFileMetaData);
            sources.Add(GenerateArchiveDb(coreFileMetaData, extensionFileMetaData));
            RoslynCompile(sources.ToArray(), assemblyName, driverFolder);
        }

        private IList<string> GenerateSourceFiles(IFileMetaData coreFileMetaData,
            IEnumerable<IFileMetaData> extensionFileMetaData)
        {
            IList<string> sources = new List<string>();

            var archiveCodeDom = new ArchiveFileCodeDom(capitalize);
            var coreFileCs = archiveCodeDom.GenerateSource(coreFileMetaData.FileName, coreFileMetaData);
            Log(coreFileCs);
            sources.Add(coreFileCs);

            foreach (var extension in extensionFileMetaData)
            {
                var extensionFileCs = archiveCodeDom.GenerateSource(extension.FileName, extension);
                Log(extensionFileCs);
                sources.Add(extensionFileCs);
            }
            return sources;
        }

        private string GenerateArchiveDb(IFileMetaData coreFileMetaData,
            IEnumerable<IFileMetaData> extensionFileMetaData)
        {
            var archiveDbCodeDom = new ArchiveDbCodeDom(capitalize, fileReaderConfig, rowFactoryConfig);
            var archiveDbCs = archiveDbCodeDom.GenerateSource(coreFileMetaData, extensionFileMetaData);
            Log(archiveDbCs);
            return archiveDbCs;
        }

        private void RoslynCompile(string[] sources, string assemblyName, string driverFolder)
        {
            var referencedAssemblies = DataContextDriver.GetCoreFxReferenceAssemblies().Concat(new[]
            {
                typeof(ArchiveReader).Assembly.Location,
            }).ToArray();

            var result = DataContextDriver.CompileSource(new CompilationInput()
            {
                FilePathsToReference = referencedAssemblies,
                SourceCode = sources,
                OutputPath = Path.Combine(driverFolder, assemblyName)
            });

            if(!result.Successful)
            {
                var message = string.Join(' ', result.Errors);
                throw new Exception($"Compilation failed! {message.ToString()}");
            }
        }

        private void Log(string message)
        {
            const string DWC_DEBUG = "DWC_DEBUG";
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(DWC_DEBUG)))
            {
                Debug.WriteLine(message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}
