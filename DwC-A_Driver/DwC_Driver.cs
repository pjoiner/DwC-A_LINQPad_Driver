using DwC_A;
using DwC_A.Config;
using LINQPad.Extensibility.DataContext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace DwC_A_Driver
{
    public class DwC_Driver : DynamicDataContextDriver
    {
        public override string Name => "Darwin Core Archive Driver";

        public override string Author => "Paul Joiner";

        public override string GetConnectionDescription(IConnectionInfo cxInfo)
        {
            string fileName = Path.GetFileNameWithoutExtension(cxInfo.DriverData.Element("FileName")?.Value);
            return $"DwC-A({fileName})";
        }

        public override bool ShowConnectionDialog(IConnectionInfo cxInfo, ConnectionDialogOptions dialogOptions)
        {
            var folderBrowserDialog = new FolderWindow();
            var result = folderBrowserDialog.ShowDialog();
            if (result.HasValue && result == true)
            {
                var path = (FolderViewModel)folderBrowserDialog.DataContext;
                cxInfo.DriverData.Add(new XElement("FileName", path.Path));
                cxInfo.DriverData.Add(new XElement("Capitalize", path.Capitalize));
                cxInfo.DriverData.Add(new XElement("BufferSize", path.BufferSize));
                cxInfo.DriverData.Add(new XElement("RowStrategy", path.RowStrategy));
                return true;
            }
            return false;
        }

        public override List<ExplorerItem> GetSchemaAndBuildAssembly(IConnectionInfo cxInfo, AssemblyName assemblyToBuild, ref string nameSpace, ref string typeName)
        {
            string fileName = cxInfo.DriverData.Element("FileName")?.Value;
            if(!Boolean.TryParse(cxInfo.DriverData.Element("Capitalize").Value, out bool capitalize))
            {
                capitalize = false;
            }
            if (!int.TryParse(cxInfo.DriverData.Element("BufferSize").Value, out int bufferSize))
            {
                bufferSize = 65536;
            }
            if (!Enum.TryParse(cxInfo.DriverData.Element("RowStrategy").Value, out RowStrategy rowStrategy))
            {
                rowStrategy = RowStrategy.Lazy;
            }
            nameSpace = "DwCArchive";
            typeName = "ArchiveDb";
            var driverFolder = GetDriverFolder();
            using (var archive = new ArchiveReader(fileName))
            {
                var coreFileMetaData = archive.CoreFile.FileMetaData;
                var extensionFileMetaData = archive.Extensions
                    .GetFileReaders()
                    .Select(n => n.FileMetaData);
                var archiveDbSchemaBuilder = new ArchiveDbAssemblyBuilder(capitalize, 
                    new FileReaderConfiguration() { BufferSize = bufferSize },
                    new RowFactoryConfiguration() { Strategy = rowStrategy });
                archiveDbSchemaBuilder.GenerateArchiveDbAssembly(coreFileMetaData, 
                    extensionFileMetaData, assemblyToBuild.CodeBase, driverFolder);
                var linQPadSchemaGenerator = new LINQPadSchemaGenerator(capitalize);
                return linQPadSchemaGenerator.GenerateSchema(fileName, coreFileMetaData, extensionFileMetaData);
            }
        }

        public override ParameterDescriptor[] GetContextConstructorParameters(IConnectionInfo cxInfo)
        {
            var parm = new ParameterDescriptor("fileName", typeof(string).FullName);
            return new[]
            {
                parm
            };
        }

        public override object[] GetContextConstructorArguments(IConnectionInfo cxInfo)
        {
            var fileName = cxInfo.DriverData.Element("FileName")?.Value;
            return new[]
            {
                fileName
            };
        }

        public override IEnumerable<string> GetNamespacesToAdd(IConnectionInfo cxInfo)
        {
            return new[]
            {
                "DwC_A",
                "DwC_A.Terms"
            };
        }

        public override IEnumerable<string> GetAssembliesToAdd(IConnectionInfo cxInfo)
        {
            return new[]
            {
                "DwC-A_dotnet.dll"
            };
        }

        public override void TearDownContext(IConnectionInfo cxInfo, object context, QueryExecutionManager executionManager, object[] constructorArguments)
        {
            (context as IDisposable)?.Dispose();
            base.TearDownContext(cxInfo, context, executionManager, constructorArguments);
        }

    }
}
