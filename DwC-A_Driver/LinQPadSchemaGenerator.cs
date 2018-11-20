using DwC_A.Meta;
using DwC_A.Terms;
using LINQPad.Extensibility.DataContext;
using System.Collections.Generic;
using System.IO;

namespace DwC_A_Driver
{
    class LinQPadSchemaGenerator
    {
        public List<ExplorerItem> GenerateSchema(string archiveFileName, 
            IFileMetaData coreFileMetaData, 
            IEnumerable<IFileMetaData> extensionFileMetaData)
        {
            List<ExplorerItem> explorerItems = new List<ExplorerItem>();
            var explorerItem = new ExplorerItem(archiveFileName, ExplorerItemKind.Schema,
                ExplorerIcon.LinkedDatabase)
            {
                Children = new List<ExplorerItem>()
            };

            explorerItem.Children.Add(GetFileReaderItem(coreFileMetaData, true));
            foreach(var extension in extensionFileMetaData)
            {
                explorerItem.Children.Add(GetFileReaderItem(extension, false));
            }
            explorerItems.Add(explorerItem);
            return explorerItems;
        }

        private ExplorerItem GetFileReaderItem(IFileMetaData fileMetaData, bool isCoreFile)
        {
            var explorerIcon = isCoreFile ? ExplorerIcon.View : ExplorerIcon.Table;
            var explorerItem = new ExplorerItem(Path.GetFileNameWithoutExtension(fileMetaData.FileName),
                ExplorerItemKind.QueryableObject, explorerIcon)
            {
                Children = new List<ExplorerItem>(),
                IsEnumerable = true
            };
            foreach(var field in fileMetaData.Fields)
            {
                var fieldItem = new ExplorerItem(Terms.ShortName(field.Term),
                    ExplorerItemKind.Property, ExplorerIcon.Column);
                explorerItem.Children.Add(fieldItem);
            }
            return explorerItem;
        }
    }
}
