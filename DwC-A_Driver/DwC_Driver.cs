using DwC_A;
using LINQPad.Extensibility.DataContext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
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

        public override bool ShowConnectionDialog(IConnectionInfo cxInfo, bool isNewConnection)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                cxInfo.DriverData.Add(new XElement("FileName", openFileDialog.FileName));
                return true;
            }
            return false;
        }

        public override List<ExplorerItem> GetSchemaAndBuildAssembly(IConnectionInfo cxInfo, AssemblyName assemblyToBuild, ref string nameSpace, ref string typeName)
        {
            string fileName = cxInfo.DriverData.Element("FileName")?.Value;
            nameSpace = "DwCArchive";
            typeName = "ArchiveDb";
            var driverFolder = GetDriverFolder();
            using (var archive = new ArchiveReader(fileName))
            {
                var coreFileMetaData = archive.CoreFile.FileMetaData;
                var extensionFileMetaData = archive.Extensions.Select(n => n.FileMetaData);
                var archiveDbSchemaBuilder = new ArchiveDbAssemblyBuilder();
                archiveDbSchemaBuilder.GenerateArchiveDbAssembly(coreFileMetaData, 
                    extensionFileMetaData, assemblyToBuild.CodeBase, driverFolder);
                var linQPadSchemaGenerator = new LinQPadSchemaGenerator();
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
